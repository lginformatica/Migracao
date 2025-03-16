using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LG.HCM.Integrador.Logical;
using LG.HCM.Integrador.Util;
using W3.Framework.Servico.Colecao;
using W3.Framework.Servico.Colecao.Classes;
using W3.Framework.Servico.Colecao.DadosCorporativos;

namespace LG.HCM.Integrador.Aplicacao.Profile {
    public partial class Default : System.Web.UI.Page {
		private static readonly byte[] IV = { 55, 103, 246, 79, 36, 99, 167, 3, 42, 5, 62, 83, 184, 7, 209, 13, 145, 23, 200, 58, 173, 10, 121, 222, 88, 09, 45, 67, 94, 12, 34, 5 };
		private static List<string> paginas = new List<string> { "CompetenciasEPdi" };
		private static List<string> tiposContainer = new List<string> { "iframe" };

		protected void Page_Load(object sender, EventArgs e) {
			string nomeClienteSI = HttpContext.Current.Application["NomeCliente"].ToString();
			var chave = W3.Library.Encryption.DataHash.GetHashCliente(nomeClienteSI);

			if (String.IsNullOrEmpty(chave)) {
				RedirectErro("Chave LG não configurada.");
				return;
			}

			// Recupera o endereço da página de login do sistema LG do ambiente do cliente.
			// O endereço precisa terminar com a barra "/"
			string UrlSI = Autenticacao.RetornaUrlModulo(Logical.EnumModulo.SistemaIntegrado).Replace(@"Modulos", "");

			if (String.IsNullOrEmpty(UrlSI)) {
				RedirectErro("URL LG não configurada.");
				return;
			}

			// Recupera o timestamp no formato 24h
			string timestamp = DateTime.Now.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
			timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff");

			string data = string.Empty;
			string tokenAutenticacao = string.Empty;
			bool tipoCriptografiaW3 = false;

			// Informe aqui o código do funcionário recebido do sistema LG. Recupere este token do usuário logado.
			//var t = "CodigoFuncionarioAcesso=53083932812|CodigoFuncionario=53083932812|SglIdioma=pt-BR|Pagina=CompetenciasEPdi|TipoContainer=iframe";
			//var teste = W3.Library.Encryption.DataEncryption.Encrypt(t, chave);
			try
			{
				var tipoCriptografia = Request.QueryString["TipoCriptografia"];
				if (!string.IsNullOrEmpty(tipoCriptografia))
				{
					tipoCriptografiaW3 = (Request.QueryString["TipoCriptografia"] == "w3");
				}

				data = Request.QueryString["Data"];
				tokenAutenticacao = Request.QueryString["Token"];

				if (tipoCriptografiaW3)
				{
					data = W3.Library.Encryption.DataEncryption.Decrypt(data, chave);
				} else
                {
					data = IntegracaoAutoAtendimento.Decrypt(data);
				}

				//*** valida o token
				if (!ValidaToken(tokenAutenticacao))
                {
					return;
                }

			} catch (Exception ex)
            {
				W3.Library.Log.LogErro.WriteLog(new Exception("API Profile - Erro ao tentar descriptografar os dados passados como parâmetro: " + ex.Message + " - StackTrace: " + ex.StackTrace), HttpContext.Current);
				RedirectErro("Não foi possível reconhecer os parâmetros enviados.");
				return;
			}

			Dictionary<string, string> parametros = new Dictionary<string, string>();
			if (!string.IsNullOrEmpty(data)) {
                foreach (var parametroData in data.Split('|')) {
					if (parametroData.Contains("=")) {
						var itemData = parametroData.Split('=');
						parametros.Add(itemData[0], itemData[1]);
					}
                }
			}

			string codigoFuncionarioAcesso = string.Empty;
			string codigoFuncionario = string.Empty;
			string sglIdioma = string.Empty;
			string nomePagina = string.Empty;
			string tipoContainer = string.Empty;

			codigoFuncionarioAcesso = parametros.ContainsKey("CodigoFuncionarioAcesso") ? parametros["CodigoFuncionarioAcesso"] : string.Empty;
			codigoFuncionario = parametros.ContainsKey("CodigoFuncionario") ? parametros["CodigoFuncionario"] : string.Empty;
			sglIdioma = parametros.ContainsKey("SglIdioma") ? parametros["SglIdioma"] : string.Empty;
			nomePagina = parametros.ContainsKey("Pagina") ? parametros["Pagina"] : string.Empty;
			tipoContainer = parametros.ContainsKey("TipoContainer") ? parametros["TipoContainer"] : string.Empty;

			//codigoFuncionarioAcesso = Request.QueryString["CodigoFuncionarioAcesso"];
			//codigoFuncionario = Request.QueryString["CodigoFuncionario"];
			//sglIdioma = Request.QueryString["SglIdioma"];
			//nomePagina = "CompetenciasEPdi";
			//tipoContainer = "iframe";

			if (Colecao<VwFuncionario>.Instance.Itens.Where(x => x.CodFuncionarioExterno == codigoFuncionario).Count() == 0)
			{
				W3.Library.Log.LogErro.WriteLog(new Exception("API Profile - Código externo do colaborador não encontrado:" + Convert.ToString(codigoFuncionario)), HttpContext.Current);
				RedirectErro("Os parâmetros informados não são válidos.");
				return;
			}

			if (Colecao<VwFuncionario>.Instance.Itens.Where(x => x.CodFuncionarioExterno == codigoFuncionarioAcesso).Count() == 0)
			{
				W3.Library.Log.LogErro.WriteLog(new Exception("API Profile - Código externo do colaborador acesso não encontrado:" + Convert.ToString(codigoFuncionarioAcesso)), HttpContext.Current);
				RedirectErro("Os parâmetros informados não são válidos.");
				return;
			}

			if (Colecao<Idioma>.Instance.Itens.Where(x => x.SglIdioma.Equals(sglIdioma)).Count() == 0)
			{
				W3.Library.Log.LogErro.WriteLog(new Exception("API Profile - Idioma informado não encontrado:" + Convert.ToString(sglIdioma)), HttpContext.Current);
				RedirectErro("Os parâmetros informados não são válidos.");
				return;
			}

			if (!paginas.Contains(nomePagina))
            {
				W3.Library.Log.LogErro.WriteLog(new Exception("API Profile - Página informada não encontrada:" + Convert.ToString(nomePagina)), HttpContext.Current);
				RedirectErro("Os parâmetros informados não são válidos.");
				return;
			}

			if (!tiposContainer.Contains(tipoContainer))
			{
				W3.Library.Log.LogErro.WriteLog(new Exception("API Profile - Tipo de container informado não encontrado:" + Convert.ToString(tipoContainer)), HttpContext.Current);
				RedirectErro("Os parâmetros informados não são válidos.");
				return;
			}

			var token = W3.Library.Encryption.DataEncryption.Encrypt(codigoFuncionarioAcesso + "|" + timestamp, chave);
			var tokenTimestamp = W3.Library.Encryption.DataEncryption.Encrypt(timestamp, chave);
			token = System.Net.WebUtility.UrlEncode(token);
			tokenTimestamp = System.Net.WebUtility.UrlEncode(tokenTimestamp);

			Response.Redirect(String.Format("{0}Modulos/W3.Competence/profile/#/Profile/{1}/{2}/{3}?token={4}&tokenTimestamp={5}", UrlSI, codigoFuncionarioAcesso, codigoFuncionario, sglIdioma, token, tokenTimestamp), false);
		}

		private void RedirectErro(string mensagemErro)
    {
			string UrlSI = Autenticacao.RetornaUrlModulo(Logical.EnumModulo.SistemaIntegrado).Replace(@"Modulos", "");
			Response.Redirect(String.Format("{0}Modulos/W3.Competence/profile/#/400?ErrorMsg={1}", UrlSI, mensagemErro), false);
		}

		private void RedirectErroAcesso(string mensagemErro)
		{
			string UrlSI = Autenticacao.RetornaUrlModulo(Logical.EnumModulo.SistemaIntegrado).Replace(@"Modulos", "");
			Response.Redirect(String.Format("{0}Modulos/W3.Competence/profile/#/401?ErrorMsg={1}", UrlSI, mensagemErro), false);
		}

		private bool ValidaToken(string tokenAutenticacao)
        {
			string urlSI = HttpContext.Current.Application["UrlSI"].ToString();
			var url = $"{ urlSI }Modulos/LG.HCM.Integrador";
			var client = new RestSharp.RestClient($"{url}/api/autenticacaolg/autoatendimento/validatoken");

			var request = new RestSharp.RestRequest(RestSharp.Method.GET);
			request.AddHeader("cache-control", "no-cache");
			request.AddHeader("content-type", "application/x-www-form-urlencoded");
			request.AddHeader("content-type", "application/x-www-form-urlencoded");
			request.AddHeader("Authorization", $"Bearer {tokenAutenticacao}");
			RestSharp.IRestResponse response = client.Execute(request);
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				W3.Library.Log.LogErro.WriteLog(new Exception("API Profile - acesso negado. o token enviado não é válido."), HttpContext.Current);
				RedirectErroAcesso("Acesso negado. O tempo de acesso pode ter expirado, tente atualizar a página novamente.");
				return false;
			}
			return true;
		}
	}
}