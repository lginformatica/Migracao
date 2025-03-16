<%@ WebHandler Language="C#" Class="Painel" %>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.IO;
using System.Web.Script.Serialization;
using System.Reflection;
using W3.Framework.Servico.Interface;
using System.Data;
using W3.Framework.Servico;
using Aspose.Cells;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Configuration;
using W3.Framework.Servico.Colecao.Utilitarios;

/// <summary>
/// Summary description for Delay
/// </summary>
public class Painel : ServicoBaseGrid {

    public object ListarBancos() {
        try {
            //string stringConexaoCliente = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            //string banco = stringConexaoCliente.Substring(stringConexaoCliente.ToUpper().IndexOf("INITIAL CATALOG"), stringConexaoCliente.Substring(stringConexaoCliente.ToUpper().IndexOf("INITIAL CATALOG")).IndexOf(";"));
            //banco = banco.Substring(16, banco.Length - 16);
            //DataTable dt = Util.RetornarTabela("select name from sys.databases");
            //var itens = dt.AsEnumerable().Select(p => new { Texto = p.Field<string>("name").ToLower(), Valor = p.Field<string>("name").ToLower() }).ToList();

            DataTable dt = Util.RetornarTabela("select DB_NAME() as idBanco");
            string idBanco = dt.Rows[0][0].ToString();
            var itens = dt.AsEnumerable().Select(p => new { Texto = p.Field<string>("idBanco").ToLower(), Valor = p.Field<string>("idBanco").ToLower() }).ToList();

            return new { OK = true, Itens = itens, Banco = idBanco };
        }
        catch (Exception ex) {
            return new { OK = false, Msg = ex.Message };
        }
    }

    public object ExecutarQuery(string Banco, string Query) {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        ClGrid.RemoverGrids();

        Query = HttpUtility.UrlDecode(Query);
        Query = Query.Replace("[[", "'");
        string novaQuery = Query;
        string Mensagem = string.Empty;
        int NumResultados = 0;
        string bancoCorrente = Banco;

        ClQuery Ret = Utilitarios.ExecQuery(Banco, Query);
        Mensagem += Ret.Mensagem;
        if ((Ret.DataSet != null) && (Ret.DataSet.Tables.Count > 0) && (Ret.DataSet.Tables[0].Columns.Count > 0)) {
            NumResultados = Ret.DataSet.Tables.Count;
            bancoCorrente = Ret.BancoCorrente;
        }
        HttpContext.Current.Session["QueryResults"] = Ret;
        for (var i = 1; i <= NumResultados; i++) {
            HttpContext.Current.Cache.Remove("ClGrid_resultado_" + i.ToString() + "_" + HttpContext.Current.Session.SessionID);
        }

        stopwatch.Stop();
        TimeSpan ts = stopwatch.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        return new { Query = Query, Mensagem = Mensagem, NumResultados = NumResultados, BancoCorrente = bancoCorrente, ElapsedTime = elapsedTime };
    }

    public object PrepararDadosGrid(string IdGrid) {
        string msg = string.Empty;
        bool ok = false;
        ClGrid gridx;
        try {
            gridx = ClGrid.RetornaGrid(IdGrid);
            if (gridx == null) {
                ClQuery Ret = (ClQuery)Context.Session["QueryResults"];
                int numTabela = Convert.ToInt32(IdGrid.Split(new char[] { '_' })[1]) - 1;
                gridx = new ClGrid(IdGrid, Ret.DataSet.Tables[numTabela], Ret.DataSetReferencia);
                //gridx.DatasetReferencia = Ret.DataSetReferencia;
                //gridx.Banco = Ret.BancoCorrente;
            }
            msg = "Grid criada / recuperada com " + gridx.TabelaBase.Columns.Count + " colunas e " + gridx.TabelaBase.Rows.Count + " linhas.";
            ok = true;
        }
        catch (Exception ex) {
            msg = ex.Message;
            ok = false;
        }
        return new { OK = ok, Mensagem = msg };
    }

    [ServicoWeb(Ajax = false)]
    public void GerarExcelCompleto() {
        Workbook workbook = new Workbook();
        workbook.Worksheets.RemoveAt(0);
        ClQuery Ret = (ClQuery)Context.Session["QueryResults"];
        int i = 0;
        foreach (DataTable dt in Ret.DataSet.Tables) {
            workbook.Worksheets.Add("Resultado " + (i + 1).ToString());
            Worksheet worksheet = workbook.Worksheets[i];
            worksheet.Cells.ImportDataTable(dt, true, "A1");
            if (dt.Columns.Count > 0) {
                FormatarPlanilha(worksheet, dt, dt.Rows.Count, dt.Columns.Count - 1);
            }
            i++;
        }

        Context.Response.ContentType = "application/excel";
        workbook.Save(HttpContext.Current.Response, "Planilha.xlsx", ContentDisposition.Attachment, new OoxmlSaveOptions(SaveFormat.Xlsx));
        Context.Response.Flush();
        Context.Response.End();
    }
}
