<%@ WebHandler Language="C#" Class="InfoSincronismo" %>
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
public class InfoSincronismo : ServicoBaseCadastro {
    public object RetornarInfo() {
        List<string> lista = ColecaoSincronismo.Instance.RetornaInfo();
        return new { OK = true, Lista = lista };
    }

}
