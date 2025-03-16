using LG.HCM.Integrador.Model.Integracoes.Clave;
using LG.HCM.Integrador.Util;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Linq;
using System.Web;
using W3.Framework.Servico.Colecao;
using W3.Framework.Servico.ServicoColecao;

namespace LG.HCM.Integrador.Aplicacao.Integracoes.Clave.Servicos {
	/// <summary>
	/// Classe de serviço da página "SSO.aspx" com os métodos de acesso a dados e regras de negócio.
	/// </summary>
	public class SSO : ServicoBaseCadastro {
		const double MINUTOS_ADICIONAIS = 0.00;

		public object RecuperarUrlSSoClave() {
			try {

				string urlSI = Classes.Parametros.Buscar("UrlSI").VlrParametro;
				string urlBase = Classes.Parametros.Buscar("IntegracaoClave.UrlBase").VlrParametro;
				string chave = Classes.Parametros.Buscar("IntegracaoClave.Chave").VlrParametro;

				var codigoUsuarioLogado = HttpContext.Current?.Session?["CodUsuario"] != null ? Convert.ToInt32(HttpContext.Current.Session["CodUsuario"]) : 0;
				var usuario = Colecao<W3.Framework.Servico.Colecao.Classes.Usuario>.Instance.Itens.Where(p => p.CodUsuario == codigoUsuarioLogado).FirstOrDefault();

				if (usuario == null) {
					return new { OK = false, Msg = Msg("erro_parametros") };
				}

				if (!usuario.CodFuncionario.HasValue) {
					return new { OK = false, Msg = Msg("erroAcessoParaNaoFuncionario"), Titulo = Msg("atencao") };
				}

				string nome = usuario.NomUsuario;
				string email = usuario.DscEmail;
				int codigoCliente = IntegracaoClave.ObterClienteIntegracao();

				string data = DateTime.Now.AddMinutes(MINUTOS_ADICIONAIS).ToString("dd/MM/yyyy HH:mm");
				LoginSSO loginSSO = new LoginSSO(nome, email, codigoCliente, chave, data);

				string jsonDados = JsonConvert.SerializeObject(loginSSO);
				string hash = CalcularHashSha256(jsonDados, System.Text.Encoding.UTF8);

				string endPointSSO = String.Format("/#/home/login?{0}%7C{1}%7C{2}%7C{3}", nome, email, codigoCliente, hash);
				urlBase = (urlBase.Substring(urlBase.Length - 1, 1).Equals("/") ? urlBase.Substring(0, urlBase.Length - 1) : urlBase);
				string urlSSO = urlBase + endPointSSO;

				W3.Library.Log.FrameworkTrace.Instance.ForceTrace($"Login SSO Clave # \n\tNome:{nome} \n\tEmail:{email} \n\tEmail:{codigoCliente} \n\tData:{data} \n\tUrl:{urlSSO}");
				return new { OK = true, urlRedirect = urlSSO, parametros = jsonDados };
			}
			catch (Exception ex) {
				W3.Library.Log.FrameworkTrace.Instance.LogarErro(ex.Message, ex);
				return new { OK = false, Msg = Msg("erro_parametros"), Titulo = Msg("atencao") };
			}
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