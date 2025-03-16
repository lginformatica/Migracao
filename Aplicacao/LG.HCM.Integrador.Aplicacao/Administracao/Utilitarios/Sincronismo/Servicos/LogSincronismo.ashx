<%@ WebHandler Language="C#" Class="LogSincronismo" %>
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

/// <summary>
/// Summary description for Configuracoes
/// </summary>
public class LogSincronismo : ServicoBaseCadastro {
    public override object Listar(string idCadastro, string idGrid) {
        DataTable dt = ColecaoSincronismo.Instance.RetornaLog().AsEnumerable().OrderByDescending(p => p.Field<int>("Sequencia")).CopyToDataTable();
        dt.ExtendedProperties.Add("regra_estilo_linha_01", "avaliar('{Status}' == 'Erro') ==> color: red;");
        ClGrid gridx = new ClGrid(idGrid, dt);
        return new { OK = true, TotalRegistros = gridx.TabelaBase.Rows.Count };
    }

    public override object RetornarToolbar(string idCadastro) {
        return "<toolbar />";
    }
}
