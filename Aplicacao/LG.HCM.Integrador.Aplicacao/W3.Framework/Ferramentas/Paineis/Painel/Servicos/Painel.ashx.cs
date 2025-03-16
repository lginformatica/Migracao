using Aspose.Cells;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace W3.Framework.Servico.API.Utilitarios.Paineis.Painel.Servicos {
	/// <summary>
	/// Summary description for Delay
	/// </summary>
	public class Painel : ServicoBaseGrid {

		public object ListarBancos() {
			try {
				DataTable dt = W3.Framework.Servico.Colecao.Utilitarios.Util.RetornarTabela("select DB_NAME() as idBanco");
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

			ClQuery Ret = W3.Framework.Servico.Utilitarios.ExecQuery(Banco, Query);
			Mensagem += Ret.Mensagem;
			if ((Ret.DataSet != null) && (Ret.DataSet.Tables.Count > 0) && (Ret.DataSet.Tables[0].Columns.Count > 0)) {
				NumResultados = Ret.DataSet.Tables.Count;
				bancoCorrente = Ret.BancoCorrente;
			}
			HttpContext.Current.Session["QueryResults"] = Ret;

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
					gridx = new ClGrid(IdGrid, Ret.DataSet.Tables[0], Ret.DataSetReferencia);
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
			workbook.Save("Planilha.xlsx", new OoxmlSaveOptions(SaveFormat.Xlsx));
			Context.Response.Flush();
			Context.Response.End();
		}
	}
}