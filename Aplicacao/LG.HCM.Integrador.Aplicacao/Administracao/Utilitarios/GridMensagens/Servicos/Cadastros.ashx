<%@ WebHandler Language="C#" Class="Cadastros" %>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Xml.Linq;
using Aspose.Cells;
using W3.Framework.Servico;
using W3.Framework.Servico.ServicoColecao;
using W3.Framework.Servico.Colecao;
using W3.Framework.Servico.Colecao.Classes;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Reflection;

/// <summary>
/// Summary description for Configuracoes
/// </summary>
public class Cadastros : ServicoBaseCadastro {
    public override object Listar(string idCadastro, string idGrid) {
        try {
            IColecao colecao = Colecoes.Colecao(idCadastro);
            DataSet ds;
            ds = colecao.RetornarDataSetGrid();
            DataTable dt = ds.Tables[0].Copy();

            foreach (var key in ds.Tables[0].ExtendedProperties.Keys) {
                if (!dt.ExtendedProperties.ContainsKey(key)) {
                    dt.ExtendedProperties.Add(key, ds.Tables[0].ExtendedProperties[key]);
                }
            }

            if (Parametros.ContainsKey("FiltroBusca")) {
                int coluna = 5; //*** parametrziar isto
                var linhas = dt.AsEnumerable().Where(p => p.Field<string>(coluna).StartsWith(Parametros["FiltroBusca"].ToString())).ToList();
                dt = (linhas.Count > 0) ? linhas.CopyToDataTable() : dt.Clone();
            }

            DataSet dsNovo = new DataSet();
            dsNovo.Tables.Add(dt);
            for (int x = 1; x < ds.Tables.Count; x++) {
                dsNovo.Tables.Add(ds.Tables[x].Copy());
            }
            ds = dsNovo;
            //}
            var tipoSel = Context.Session["TipoSeletor_" + idGrid];
            if (tipoSel != null) {
                ds.Tables[0].Columns[0].ColumnName = tipoSel.ToString();
            }
            ClGrid gridx = new ClGrid(idGrid, ds);

            return new { OK = true, TotalRegistros = gridx.TabelaBase.Rows.Count };
        }
        catch (Exception ex) {
            return new { OK = false, Msg = ex.Message };
        }
    }

    public override object Salvar(string idCadastro) {
        string dscMensagem = "";
        foreach (var idioma in Colecao<Idioma>.Instance.Itens.OrderBy(p => p.CodIdioma)) {
            if (Parametros.ContainsKey(idioma.SglIdioma)) {
                dscMensagem += "<" + idioma.SglIdioma + "><![CDATA[" + Parametros[idioma.SglIdioma] + "]]></" + idioma.SglIdioma + ">";
            }
        }
        Parametros["DscMensagem"] = "<idioma>" + dscMensagem + "</idioma>";
        return base.Salvar(idCadastro);
    }

    public object RetornaSelecionados(string idGrid) {
        ClGrid gridx = ClGrid.RetornaGrid(idGrid);
        if (gridx == null) {
            return new { OK = false, GridNula = true };
        }
        return new { OK = true, TotalRegistros = gridx.ListarRegistrosSelecionados().Count };
    }

    public object RemoverTodasCustomizacoes() {
        W3.Framework.Servico.Colecao.Classes.Mensagem.ExcluirTodasCustomizacoes();
        return new { OK = true };
    }

    public object RemoverCustomizacoesSelecionadas(string idGrid) {
        ClGrid gridx = ClGrid.RetornaGrid(idGrid);
        if (gridx == null) {
            return new { OK = false, GridNula = true };
        }

        int[] codigos = gridx.ListarRegistrosSelecionados().Select(p => p.Field<int>(BaseItem.IdChavePrimaria)).ToArray();
        Colecao<MensagemFramework>.Instance.Excluir(codigos);

        return new { OK = true };
    }

    public virtual object PrepararExcelCompleto(string idCadastro) {
        bool ok = true;
        string msg = string.Empty;
        Workbook workbook = new Workbook();
        try {
            Worksheet worksheet = workbook.Worksheets[0];
            IColecao colecao = Colecoes.Colecao(idCadastro);
            DataTable dt = colecao.TabelaBase.Copy();

            #region Verifica Idiomizacao
            foreach (var col in colecao.Colunas.Where(p => p.Tipo == typeof(TIdioma))) {
                DataColumn colIdioma = dt.Columns[col.Id];
                int ix = colIdioma.Ordinal + 1;
                foreach (var idioma in Colecao<Idioma>.Instance.Itens.OrderBy(p => p.CodIdioma)) {
                    DataColumn nCol = dt.Columns.Add(col.Id + "_" + idioma.SglIdioma);
                    nCol.SetOrdinal(ix++);
                }
            }
            foreach (DataRow dr in dt.Rows) {
                foreach (var col in colecao.Colunas.Where(p => p.Tipo == typeof(TIdioma))) {
                    foreach (var idioma in Colecao<Idioma>.Instance.Itens.OrderBy(p => p.CodIdioma)) {
                        dr[col.Id + "_" + idioma.SglIdioma] = (new TIdioma(dr.Field<string>(col.Id), true)).ToString(idioma.SglIdioma);
                    }
                }
            }
            foreach (var col in colecao.Colunas.Where(p => p.Tipo == typeof(TIdioma))) {
                DataColumn colIdioma = dt.Columns[col.Id];
                dt.Columns.Remove(colIdioma);
            }

            #endregion

            #region Coluna Eliminadas
            //*** elimina colunas seletoras
            if ((dt.Columns[0].ColumnName == "radiobox") || (dt.Columns[0].ColumnName == "checkbox")) {
                dt.Columns.RemoveAt(0);
            }
            //*** elimina coluna chave 
            if (dt.Columns.Contains(colecao.IdChavePrimaria)) {
                dt.Columns.Remove(colecao.IdChavePrimaria);
            }
            #endregion

            #region formatacao
            worksheet.Cells.ImportDataTable(dt, true, "A1");
            FormatarPlanilha(worksheet, dt, dt.Rows.Count, dt.Columns.Count - 1);

            //*** coloca sombra na coluna chave primaria
            Style style = worksheet.Workbook.Styles[worksheet.Workbook.Styles.Add()];
            style.ForegroundColor = System.Drawing.Color.LightGray;
            style.Pattern = BackgroundType.Solid;
            style.HorizontalAlignment = TextAlignmentType.Center;
            StyleFlag flag = new StyleFlag();
            flag.All = true;
            foreach (var col in colecao.ColunasChave) {
                Range range = worksheet.Cells.CreateRange(1, dt.Columns[col].Ordinal, dt.Rows.Count, 1);
                range.ApplyStyle(style, flag);
            }
            #endregion

            Context.Session["TabelaExportacaoCompleta"] = workbook;
        }
        catch (Exception ex) {
            ok = false;
            msg = ex.Message;
        }
        return new { OK = ok, Msg = msg };
    }



    [ServicoWeb(Ajax = false)]
    public void GerarPacote() {
        Dictionary<string, XDocument> docs = new Dictionary<string, XDocument>();
        foreach (var mensagem in Colecao<Mensagem>.Instance.Itens.Where(p => p.IndCustomizada == "S").Distinct()) {
            string path;
            XDocument xDoc;
            if (mensagem.DscEndereco.ToLower().EndsWith(".msg")) {
                xDoc = XDocument.Load(HttpContext.Current.Server.MapPath("~") + @"\" + mensagem.DscEndereco);
                path = @"Aplicacao\" + mensagem.DscEndereco;
            }
            else {
                if (mensagem.DscEndereco.StartsWith("W3.Framework.Servico.Mensagens.")) {
                    string nomApp = "W3.Framework.Servico";
                    Assembly assembly = typeof(W3.Framework.Servico.Colecao.BaseColecao).Assembly;
                    string s = assembly.GetManifestResourceNames().Where(p => p.ToLower().StartsWith(mensagem.DscEndereco.ToLower())).FirstOrDefault();
                    Stream st = assembly.GetManifestResourceStream(s);
                    xDoc = XDocument.Load(st);
                    path = nomApp + @"\" + mensagem.DscEndereco.Substring(nomApp.Length + 1).Replace(".", @"\");
                }
                else {
                    IColecao colecao = Colecoes.Colecao(mensagem.DscEndereco);
                    xDoc = colecao.Xml;
                    string nomApp = AppDomain.CurrentDomain.GetAssemblies().Where(p => mensagem.DscEndereco.StartsWith(p.GetName().Name)).FirstOrDefault().GetName().Name;
                    path = nomApp + @"\" + mensagem.DscEndereco.Substring(nomApp.Length + 1).Replace(".", @"\");
					int px = path.LastIndexOf(@"\");
					path = path.Substring(0, px) + @"\Configuracao" + path.Substring(px);
                }
                path += ".Xml";
            }
            if (!docs.ContainsKey(path))
                docs.Add(path, xDoc);
            xDoc = docs[path];
            XElement xMensagens = xDoc.Descendants("Mensagens").FirstOrDefault();
            XElement xMsg = xMensagens.Descendants("Mensagem").Where(p => p.Attribute("chave").Value == mensagem.NomChave).FirstOrDefault();
            xMsg.Nodes().Remove();
            XElement xEl = XElement.Parse(mensagem.DscMensagem.ToString());
            xMsg.Add(xEl.Nodes());
        }


        MemoryStream ZipInMemory = new MemoryStream();
        using (ZipArchive UpdateArchive = new ZipArchive(ZipInMemory, ZipArchiveMode.Update)) {
            foreach (var path in docs.Keys) {
                ZipArchiveEntry entry = UpdateArchive.CreateEntry(path);
                using (StreamWriter writer = new StreamWriter(entry.Open())) {
                    docs[path].Save(writer);
                }
            }
        }

        byte[] buffer = ZipInMemory.GetBuffer();
        Context.Response.AppendHeader("content-disposition", "attachment; filename=Mensagens.zip");
        Context.Response.AppendHeader("content-length", buffer.Length.ToString());
        Context.Response.ContentType = "application/x-compressed";
        Context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        Context.ApplicationInstance.CompleteRequest();
    }
}
