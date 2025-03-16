<%@ WebHandler Language="C#" CodeBehind="Calcula.ashx.cs" Class="Calcula" %>
using Aspose.Cells;
using NCalc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using W3.Framework.Servico;
using W3.Framework.Servico.Colecao;
using W3.Framework.Servico.Colecao.Classes;
using W3.Framework.Servico.Colecao.Utilitarios;
using W3.Framework.Servico.ServicoColecao;
using W3.Library.DataSql;

public class Calcula : ServicoBaseAshx {
    public object ProcessarFormula(string formula) {
        try {
            Expression e = new Expression(formula);
            object result = e.Evaluate();
            return new { OK = true, Resultado = result.ToString() };
        }
        catch (Exception ex) {
            return new { OK = false, Msg = ex.Message };
        }
    }
}
