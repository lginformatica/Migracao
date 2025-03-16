using LG.HCM.Integrador.Classes;
using System;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using W3.Framework.Servico.Colecao;
using W3.Framework.Servico.ServicoColecao;

namespace LG.HCM.Integrador.Aplicacao.Integracoes.Eguru.Servicos {
	/// <summary>
	/// Classe de serviço da página "SSO.aspx" com os métodos de acesso a dados e regras de negócio.
	/// </summary>
	public class SSO : ServicoBaseCadastro {
		public object RecuperarUrlSSoEguru(string url) {
			try {
				Parametros UrlSI = Classes.Parametros.Buscar("UrlSI");
				var ChaveHashEguru = Classes.Parametros.Buscar("ChaveHashEguru");
				var UrlSSOEguru = Classes.Parametros.Buscar("UrlSSOEguru");

				var parametros = "login=" + HttpContext.Current.Session["DscLogin"].ToString();
				parametros += "&" + getUrlCompleta(url);
				parametros += "&data-solicitacao=" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				parametros += "&url-erro=" + UrlSI.VlrParametro + "PaginaErro/Erro.aspx?dscErro={0}";
				parametros = AdicionarHashInterno(parametros, ChaveHashEguru.VlrParametro);

				var codigoUsuarioLogado = HttpContext.Current?.Session?["CodUsuario"] != null ? Convert.ToInt32(HttpContext.Current.Session["CodUsuario"]) : 0;
				var usuario = Colecao<W3.Framework.Servico.Colecao.Classes.Usuario>.Instance.Itens.Where(p => p.CodUsuario == codigoUsuarioLogado).FirstOrDefault();
				if (!usuario.CodFuncionario.HasValue) {
					return new { OK = false, Msg = Msg("erroAcessoParaNaoFuncionario"), Titulo = Msg("atencao") };
				}

				var urlRedirect = UrlSSOEguru.VlrParametro;
				//Realiza o carregamento da aplicação antes de ocorrer o redirecionamento. 
				try {
					var request = (HttpWebRequest)WebRequest.Create(urlRedirect);
					request.GetResponse();
				}
				catch { }

				W3.Library.Log.FrameworkTrace.Instance.ForceTrace($"Login SSO LMS # \n\tNome:{usuario.NomUsuario} \n\tEmail:{usuario.DscEmail} \n\tLogon:{usuario.DscLogon} \n\tData:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} \n\tUrl:{urlRedirect}");

				return new { OK = true, urlRedirect = urlRedirect, parametros = parametros };

			}
			catch (Exception ex) {
				W3.Library.Log.FrameworkTrace.Instance.LogarErro(ex.Message, ex);
				return new { OK = false, Msg = Msg("erro_parametros"), Titulo = Msg("atencao") };
			}
		}

		private string getUrlCompleta(string url) {
			var arrUrl = url.Split('?');
			if (arrUrl.Count() > 2) {
				var novaUrl = string.Format("{0}", arrUrl[1]);
				for (int i = 2; i < arrUrl.Count(); i++)
					novaUrl += string.Format("&{0}", arrUrl[i]);
				return novaUrl;
			}
			else {
				return arrUrl[1];
			}
		}

		private static string AdicionarHashInterno(string dados, string chave) {
			var dadosInterpr = HttpUtility.ParseQueryString(dados);
			dadosInterpr.Add("chave", chave);
			var dadosComChave = dadosInterpr.ToString();
			var hashCalculado = CalcularHashSha256(dadosComChave);
			dadosInterpr.Remove("chave");
			dadosInterpr.Add("hash", hashCalculado);
			return dadosInterpr.ToString();

		}
		private static void ValidarHash(NameValueCollection dadosInterpr, string hashEnviado, string chave) {
			dadosInterpr.Remove("hash");
			dadosInterpr.Add("chave", chave);
			var dadosComChave = dadosInterpr.ToString();
			var hashCalculado = CalcularHashSha256(dadosComChave);
			if (hashCalculado != hashEnviado)
				throw new InvalidOperationException("Não foi possível validar a autenticidade da requisição.");
		}
		private static string CalcularHashSha256(string texto) {
			return CalcularHashSha256(texto, System.Text.Encoding.ASCII);
		}
		private static string CalcularHashSha256(string texto, System.Text.Encoding codificacao) {
			var sha256Managed = new System.Security.Cryptography.SHA256Managed();
			var hashArray = sha256Managed.ComputeHash(codificacao.GetBytes(texto), 0,
			codificacao.GetByteCount(texto));
			var stringBuilder = new System.Text.StringBuilder();
			foreach (var b in hashArray)
				stringBuilder.Append(b.ToString("x2"));
			return stringBuilder.ToString();
		}

	}
}