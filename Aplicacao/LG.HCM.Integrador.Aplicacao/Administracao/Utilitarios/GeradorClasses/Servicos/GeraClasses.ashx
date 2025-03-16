<%@ WebHandler Language="C#" Class="GeraClasses" %>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Data;
using System.Text;
using System.Collections;
using W3.Framework.Servico;
using W3.Framework.Servico.Colecao;
using W3.Library.DataSql;
using W3.Framework.Servico.Colecao.Utilitarios;
using System.Configuration;
using System.IO;
using System.IO.Compression;

public class GeraClasses : ServicoBaseGrid {

    public object RetornaInfo() {
        string path = Context.Server.MapPath("~").Replace(@"\", "/");
        path = path.Substring(0, path.LastIndexOf("/")) + ConfigurationManager.AppSettings["Colecao.Classes"];
        return new { Namespace = ConfigurationManager.AppSettings["Colecao.Classes"], Path = path };
    }

    public object RetornaOwners() {
        string sql = @"
            select b.name
                from sys.objects a
                inner join sys.schemas b on a.schema_id = b.schema_id
                where a.type = 'U'
                group by b.name
                order by 1;
            ";
        var ret = DatabaseUtil.Connector.BindSql(sql).AsDataTable().AsEnumerable().Select(p => new { text = p.Field<string>("name"), value = ((p.Field<string>("name").ToLower().StartsWith("w3")) ? "{owner-" + p.Field<string>("name").Substring(2, 2) + "}" : p.Field<string>("name")) }).ToList();
        return ret;
    }

    public object RetornaTabelas(string owner) {
        List<string> tabelas = RetornaTabelasFromOwner(owner);
        var ret = tabelas.Select(p => new { text = p, value = (owner.StartsWith("{owner") ? owner : owner + ".") + p }).ToList();
        return ret;
    }

    public List<string> RetornaTabelasFromOwner(string owner) {
        string sql = @"
                select a.name
                    from sys.objects a
                    inner join sys.schemas b on a.schema_id = b.schema_id
                    where a.type = 'U' and b.name = ?
                    order by 1;
            ";

        string ownerDb = owner.StartsWith("{owner-") ? "W3" + owner.Substring(7, 2) : owner;
        List<string> ret = DatabaseUtil.Connector.BindSql(sql).ToParam("Owner", ownerDb).AsDataTable().AsEnumerable().Select(p => p.Field<string>("name")).ToList();
        return ret;
    }

    public object CopiarArquivo(string nomTabela, string indEstatica, string path, string nomespace) {
        path = path.TrimEnd('\\') + "\\";
        var classe = GerarClasse(nomTabela, indEstatica, nomespace);
        string nomClasse = classe.NomClasse;
        string pathFile = path + nomClasse + ".cs";
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        using (StreamWriter outfile = new StreamWriter(pathFile)) {
            outfile.Write(string.Join(Environment.NewLine, classe.Classe));
        }

        string xmlPath = path + "/Configuracao/";
        if (!Directory.Exists(xmlPath)) {
            Directory.CreateDirectory(xmlPath);
        }

        xmlPath += nomClasse + ".xml";
        using (StreamWriter outfile = new StreamWriter(xmlPath)) {
            outfile.Write(string.Join(Environment.NewLine, classe.Configuracao));
        }
        return new { OK = true };
    }

    public ClRetorno GerarClasse(string nomTabela, string indEstatica, string nomespace) {
        string sql = @"
                        SELECT t.name, a.object_id AS TableId, s.name, a.name AS TableName, c.column_id AS ColumnId, c.name AS ColumnName, t.name AS TypeName, c.precision AS NumericPrecision, c.scale AS NumericScale, c.is_identity as IsIdentity, c.is_nullable AS IsNullable,
                            (SELECT COUNT(column_name) FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE 
                                WHERE TABLE_NAME = a.name AND TABLE_SCHEMA = s.name AND CONSTRAINT_NAME =
                                        (SELECT constraint_name FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                                            WHERE TABLE_NAME = a.name AND constraint_type = 'PRIMARY KEY' AND CONSTRAINT_SCHEMA = s.name and  COLUMN_NAME = c.name
                                        )
                            ) AS IsPrimaryKey,
	                        convert(int,c.max_length) as CharMaxLength,
	                        case when ck.object_id is not null then 1 else 0 end as temConstraint
                        FROM sys.tables a
                        inner join  sys.columns c on a.object_id = c.object_id
                        inner join sys.types t on c.system_type_id = t.system_type_id and t.user_type_id = c.user_type_id
                        inner join sys.schemas s on a.schema_id = s.schema_id 
                        left join sys.check_constraints ck on a.object_id = ck.parent_object_id and c.column_id = ck.parent_column_id
                        WHERE
                            s.name = ? and   a.name = ?
                        ORDER BY column_id;
            ";
        string[] spl = Util.ParseQuery(nomTabela).Split('.');
        string owner = (spl.Length > 1) ? spl[0] : "dbo";

        string nomTabelaDb = (spl.Length > 1) ? spl[1] : nomTabela;

        //string aux = nomTabela.Replace("{owner-", "w3").Replace("}",".");
        //aux = aux.Split('.')[0];
        //aux = aux.ToLower() + ".";
        //string k = ConfigurationManager.AppSettings.AllKeys.Where(p => p.StartsWith("TableOwner")).Where(p => ConfigurationManager.AppSettings[p].ToLower() == aux).Select(p => p).FirstOrDefault();
        //k = k.Replace("TableOwner", "{owner-") + "}";

        DataTable dt = DatabaseUtil.Connector.BindSql(sql).ToParam("Owner", owner).ToParam("Tabela", nomTabelaDb).AsDataTable();
        var linhas = dt.AsEnumerable().Select(p => new {
            NomColuna = p.Field<string>("ColumnName"),
            NomPropriedade = Util.CapitalizeDbName(p.Field<string>("ColumnName")),
            Tipo = TypeExtensions.ConvertFromSql(p.Field<string>("TypeName")),
            IndPermitirNulo = p.Field<bool>("IsNullable"),
            IndChave = (p.Field<int>("IsPrimarykey") == 1),
            IndIdentity = p.Field<bool>("IsIdentity"),
            TamanhoMaximo = p.Field<int>("CharMaxLength"),
            TemConstraint = (p.Field<int>("temConstraint") == 1)
        }).ToList();

        //*** foreign keys
        sql = @"
                    SELECT FK_Owner = FK.TABLE_SCHEMA, FK_Table = FK.TABLE_NAME, FK_Column = CU.COLUMN_NAME, PK_Owner= PK.TABLE_SCHEMA, PK_Table = PK.TABLE_NAME, PK_Column = PT.COLUMN_NAME, Constraint_Name = C.CONSTRAINT_NAME
	                    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C
	                    INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
	                    INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME
	                    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
	                    INNER JOIN (
		                    SELECT i1.TABLE_NAME, i2.COLUMN_NAME
		                    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1
		                    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME
		                    WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY'
	                    ) PT ON PT.TABLE_NAME = PK.TABLE_NAME
	                    WHERE  Fk.TABLE_SCHEMA = ? and  FK.TABLE_NAME = ?;
                    ";
        DataTable dtReferencia = DatabaseUtil.Connector.BindSql(sql).ToParam("Owner", owner).ToParam("Tabela", nomTabelaDb).AsDataTable();
        var referencias = dtReferencia.AsEnumerable().Select(p => new { NomColuna = p.Field<string>("FK_COLUMN"), NomPropriedade = Util.CapitalizeDbName(p.Field<string>("FK_COLUMN")), NomReferencia = Util.CapitalizeDbName(p.Field<string>("PK_TABLE")) }).ToList();


        #region classe
        string nomClasse = Util.CapitalizeDbName(nomTabelaDb);
        List<string> classe = new List<string>();
        if (indEstatica.ToUpper() == "S") {
            classe.Add("using System;");
            classe.Add("using System.Collections.Generic;");
            classe.Add("using System.Linq;");
            classe.Add("using System.Text;");
            classe.Add("using System.Threading.Tasks;");
            classe.Add("using W3.Framework.Servico.Colecao;");
            classe.Add("using W3.Framework.Servico.Colecao.Classes;");
            classe.Add("using W3.Library.Data;");
            classe.Add("");
            string sNamespace = "namespace " + nomespace + " {";
            classe.Add(sNamespace);
            classe.Add("    public class " + nomClasse + ": BaseItem {");
            classe.Add("        #region Propriedades basicas");
            foreach (var linha in linhas) {
                classe.Add("        public " + linha.Tipo.Name + " " + linha.NomPropriedade + " { get; set; }");
            }
            classe.Add("        #endregion");
            classe.Add("    }");
            classe.Add("}");
        }
        #endregion

        //*** Configuracao
        List<string> configuracao = new List<string>();
        configuracao.Add("<?xml version='1.0' encoding='Windows-1252' ?>");
        configuracao.Add("<cadastro xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xsi:noNamespaceSchemaLocation='../../Config.xsd' id='" + nomClasse + "' dbName='" + nomTabela + "'>");

        #region Colunas
        configuracao.Add("  <colunas>");
        string propDescricao = linhas.Where(p => !p.IndPermitirNulo).Where(p => p.TamanhoMaximo == -1 || p.TamanhoMaximo > 10).Where(p => p.Tipo.Name == "String" || p.Tipo.Name == "TIdioma").Select(p => p.NomPropriedade).FirstOrDefault();
        foreach (var linha in linhas) {
            string indChave = (linha.IndChave) ? " indChave='true' " : "";
            string indIdentity = (linha.IndIdentity) ? " indIdentity='true' " : "";
            string indDescricao = (linha.NomPropriedade == propDescricao) ? " indDescricao='true' " : "";
            string indSequencia = ((linha.Tipo == typeof(int)) && ((linha.NomPropriedade == "NumOrdem") || (linha.NomPropriedade == "NumSequencia"))) ? " indSequencia='true' " : "";
            string tipoEstatica = (indEstatica.ToUpper() == "S") ? "" : " tipo='" + ((linha.Tipo.Name == "TIdioma") ? "TIdioma" : linha.Tipo.FullName) + "'";

            configuracao.Add("      <coluna id='" + linha.NomPropriedade + "'" + indChave + indIdentity + indDescricao + indSequencia + tipoEstatica + ">");
            if (linha.IndPermitirNulo || linha.Tipo == typeof(string)) {
                configuracao.Add("          <validacao " + (linha.IndPermitirNulo ? " indPermitirNulo ='true' " : "") + ((linha.Tipo == typeof(string)) ? " tamanhoMaximo='" + ((linha.TamanhoMaximo == -1) ? 8000 : linha.TamanhoMaximo) + "' " : "") + " />");
            }
            //*** referencia
            var refer = referencias.Where(p => p.NomColuna == linha.NomColuna).FirstOrDefault();
            if (refer != null) {
                configuracao.Add("          <referencia tipo='Colecao' nome='" + refer.NomReferencia + "' />");
            }
            else {
                if (linha.TemConstraint) {
                    configuracao.Add("          <referencia tipo='Lista' nome='Lista" + linha.NomPropriedade + "' />");
                }
            }


            configuracao.Add("      </coluna>");
        }
        configuracao.Add("  </colunas>");
        #endregion

        #region Formulario
        configuracao.Add("  <formulario largura='800'  altura='600'>");
        configuracao.Add("      <items>");
        configuracao.Add("          <item type='settings' position='label-top' offsetLeft='10' offsetTop='10' />");
        foreach (var linha in linhas) {
            if (linha.IndIdentity) {
                configuracao.Add("            <item type='hidden' name='" + linha.NomPropriedade + "' value='0' />");
            }
            else {
                var refer = referencias.Where(p => p.NomColuna == linha.NomColuna).FirstOrDefault();
                if (refer != null) {
                    configuracao.Add("            <item type='select' name='" + refer.NomPropriedade + "' />");
                }
                else {
                    if (linha.TemConstraint) {
                        configuracao.Add("            <item type='w3radioList' name='" + linha.NomPropriedade + "' />");
                    }
                    else {
                        int tam = (linha.TamanhoMaximo > 50) ? 300 : (linha.TamanhoMaximo == -1) ? 300 : (linha.TamanhoMaximo < 30) ? 30 : linha.TamanhoMaximo;
                        switch (linha.Tipo.Name) {
                            case "TIdioma":
                                configuracao.Add("            <item type='input' name='" + linha.NomPropriedade + "' inputWidth='" + tam + "'>");
                                configuracao.Add("              <userdata name='idiomizado'>true</userdata>");
                                configuracao.Add("            </item>");
                                break;
                            case "String":
                                configuracao.Add("            <item type='input' name='" + linha.NomPropriedade + "' inputWidth='" + tam + "' />");
                                break;
                            case "DateTime":
                                configuracao.Add("            <item type='calendar' dateFormat='%d/%m/%Y' name='" + linha.NomPropriedade + "' calendarPosition='right' />");
                                break;
                            default:
                                configuracao.Add("            <item type='input' inputWidth='" + tam + "' name='" + linha.NomPropriedade + "' />");
                                break;
                        }
                    }
                }
            }
        }
        configuracao.Add("      </items>");
        configuracao.Add("  </formulario>");
        #endregion

        #region Mensagens
        configuracao.Add("  <Mensagens>");
        foreach (var linha in linhas) {
            configuracao.Add("      <Mensagem chave='" + linha.NomPropriedade + "'>");
            configuracao.Add("          <pt-BR>" + linha.NomPropriedade + "</pt-BR>");
            configuracao.Add("          <en-US>" + linha.NomPropriedade + "</en-US>");
            configuracao.Add("          <es-ES>" + linha.NomPropriedade + "</es-ES>");
            configuracao.Add("          <fr-FR>" + linha.NomPropriedade + "</fr-FR>");
            configuracao.Add("      </Mensagem>");
        }
        configuracao.Add("  </Mensagens>");
        #endregion

        configuracao.Add("</cadastro>");

        return new ClRetorno() { Classe = classe, NomClasse = nomClasse, Configuracao = configuracao };
    }

    public object RetornaZipCompleto() {
        return new { Completo = (bool)Context.Application["ZipCompleto"] };
    }

    [ServicoWeb(Ajax = false)]
    public void GerarZip() {
        Context.Application["ZipCompleto"] = false;
        string owner = Context.Request.QueryString["owner"];
        string indEstatica = Context.Request.QueryString["indEstatica"];
        string nomespace = Context.Request.QueryString["nomespace"];

        Dictionary<string, List<string>> docs = new Dictionary<string, List<string>>();
        var linhas = RetornaTabelasFromOwner(owner);
        foreach (var tabela in linhas) {
            string nomTabela = owner.StartsWith("{owner-") ? owner + tabela : "dbo." + tabela;
            string sTabela = Util.CapitalizeDbName(tabela);
            var ret = (ClRetorno)GerarClasse(nomTabela, indEstatica, nomespace);

            if (indEstatica.ToUpper() == "S") {
                docs.Add("Classes/" + sTabela + ".cs", ret.Classe);
                docs.Add("Classes/Configuracao/" + sTabela + ".xml", ret.Configuracao);
            }
            else {
                docs.Add("Customizacao/ColecaoDinamica/" + sTabela + ".xml", ret.Configuracao);
            }
        }

        using (var outputStream = new PositionWrapperStream(Context.Response.OutputStream))
        using (var ZipArchive = new ZipArchive(outputStream, ZipArchiveMode.Create, false)) {
            foreach (var path in docs.Keys) {
                ZipArchiveEntry entry = ZipArchive.CreateEntry(path);
                using (StreamWriter writer = new StreamWriter(entry.Open())) {
                    writer.Write(string.Join(Environment.NewLine, docs[path].ToArray()));
                    writer.Close();
                }
            }

        }
        Context.Application["ZipCompleto"] = true;

    }


}

public class ClRetorno {
    public string NomClasse { get; set; }
    public List<string> Classe { get; set; }
    public List<string> Configuracao { get; set; }
}

//** para suportar o streaming de zip de tamanhos maiores
public class PositionWrapperStream : Stream {
    private readonly Stream wrapped;

    private int pos = 0;

    public PositionWrapperStream(Stream wrapped) {
        this.wrapped = wrapped;
    }

    public override bool CanSeek { get { return false; } }

    public override bool CanWrite { get { return true; } }

    public override long Position {
        get { return pos; }
        set { throw new NotSupportedException(); }
    }

    public override void Write(byte[] buffer, int offset, int count) {
        pos += count;
        wrapped.Write(buffer, offset, count);
    }

    public override void Flush() {
        wrapped.Flush();
    }

    protected override void Dispose(bool disposing) {
        wrapped.Dispose();
        base.Dispose(disposing);
    }

    // all the other required methods can throw NotSupportedException
    public override bool CanRead {
        get { return false; }
    }

    public override int Read(byte[] buffer, int offset, int count) {
        throw new NotImplementedException();
    }
    public override void SetLength(long value) {
        throw new NotImplementedException();
    }
    public override long Seek(long offset, SeekOrigin origin) {
        throw new NotImplementedException();
    }

    public override long Length {
        get { throw new NotImplementedException(); }
    }

}