using LG.HCM.Integrador.Classes;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using W3.Framework.Servico.Colecao;
using W3.Framework.Servico.ServicoColecao;

namespace LG.HCM.Integrador.Aplicacao.Integracoes.Feedback.Servicos {
	/// <summary>
	/// Classe de serviço da página "SSO.aspx" com os métodos de acesso a dados e regras de negócio.
	/// </summary>
	public class Default : ServicoBaseCadastro {
		public object RecuperarUrlFeedback(string url) {
			string mensagemErro = String.Empty;

			try {
				mensagemErro = Msg("erroAcessoFeedback");

				string[] data;
				string ctrlUrl;
				data = url.Split(new char[] { '?' }, StringSplitOptions.RemoveEmptyEntries);

				if (data.Length == 0) {
					ctrlUrl = url;
				}
				else {
					ctrlUrl = data[0];
				}

				if (data.Length > 1) {
					string pdata = data[1];
					try {
						pdata = W3.Library.Encryption.DataEncryption.DecodeQueryString(String.Join(String.Empty, data, 1, data.Length - 1));
						data[1] = pdata;
					}
					catch (Exception) {
                    }

					url = String.Concat(ctrlUrl, "?", pdata);

					List<ParamData> paramList = ParseQueryString(pdata);

					if (paramList.Where(l => l.PropertyName == "visualizacaoIndividual" && Convert.ToString(l.Value).ToUpper() == "TRUE").ToList().Count > 0) {
						ParamData idPessoa = paramList.Where(l => Convert.ToString(l.PropertyName).ToUpper() == "IDPESSOA" && !String.IsNullOrEmpty(l.Value)).FirstOrDefault();
						if(String.IsNullOrEmpty(idPessoa.Value)) {
							mensagemErro = Msg("erroColaboradorID");
							throw new Exception(Msg("erroColaboradorID"));
						}
					}
				}

				string urlAcesso = string.Empty;
				string urlCDN = ConfigurationManager.AppSettings["urlCDNFeedback"];
				bool acessoFeedback = W3.Framework.Servico.Colecao.Classes.Sessao.VerificaAcessoFeedbackLG();
				string urlAutoAtendimentoLG = W3.Framework.Servico.Colecao.Classes.Sessao.RetornarUrlAutoAtendimentoLG();

				//Verifica o acesso ao Feedback e recupera a Url do Autotendimento
				if (acessoFeedback) {
					if (!String.IsNullOrEmpty(urlAutoAtendimentoLG)) {
						var uri = new Uri(urlAutoAtendimentoLG);
						urlAcesso = uri.GetLeftPart(System.UriPartial.Authority);
					}
					else {
						mensagemErro = Msg("acessoFeedbackUsuario");
						throw new Exception("Não foi possível recuperar a Url do Autoatendimento."); }
				}
				else {
					mensagemErro = Msg("acessoFeedbackUsuario");
					throw new Exception("Usuário não possui acesso do Feedback."); 
				}
				
				//Adiciona os parâmetros passados a URL do CDN
                try {
					urlCDN = String.Concat(urlCDN,"?",getUrlCompleta(url));
				}
                catch (Exception) { }				
				
				return new { OK = true, urlRedirect = urlAcesso, urlCDN = urlCDN };

			}
			catch (Exception ex) {
				W3.Library.Log.FrameworkTrace.Instance.LogarErro(ex.Message, ex);
				return new { OK = false, Msg = mensagemErro, Titulo = Msg("atencao") };
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

		private static List<ParamData> ParseQueryString(string queryString) {
			List<ParamData> paramList = new List<ParamData>();

			if (!string.IsNullOrWhiteSpace(queryString)) {
				var parsedParams = HttpUtility.ParseQueryString(queryString);

				foreach (string key in parsedParams.AllKeys) {
					paramList.Add(new ParamData {
						PropertyName = key,
						Value = parsedParams[key]
					});
				}
			}

			return paramList;
		}
		private struct ParamData {
			public string PropertyName { get; set; }
			public string Value { get; set; }
		}

	}
}