using LG.HCM.Integrador.Model;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using W3.Framework.Servico.Colecao;
using W3.Framework.Servico.Colecao.Classes;

namespace LG.HCM.Integrador.Logical
{
    public class Autenticacao
    {
		public static RestClient Autenticar(EnumModulo enumModulo)
		{
			string nomeClienteSI = HttpContext.Current.Application["NomeCliente"].ToString();
			var chave = W3.Library.Encryption.DataHash.GetHashCliente(nomeClienteSI);
			RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
			var urlIntegracao = RetornaUrlModulo(enumModulo);
			var client = new RestClient($"{urlIntegracao}/api/token");

			var request = new RestRequest(Method.POST);
			request.AddHeader("cache-control", "no-cache");
			request.AddHeader("content-type", "application/x-www-form-urlencoded");
			request.AddParameter("client_credentials", $"grant_type=client_credentials&client_id={ nomeClienteSI }&client_secret={ chave }", "application/text", ParameterType.RequestBody);
			IRestResponse response = client.Execute(request);

			var authToken = deserial.Deserialize<OAuthToken>(response);
			var tokenString = authToken?.access_token ?? string.Empty;

			var auth = new RestClient
			{
				BaseUrl = new System.Uri(urlIntegracao),
				Authenticator = new JwtAuthenticator(tokenString)
			};

			return auth;
		}

		public static string GerarTokenLG(EnumModulo enumModulo) {
			return GerarTokenLG(enumModulo, string.Empty);
		}

		public static string GerarTokenLG(EnumModulo enumModulo, string identificador)
		{
			string nomeClienteSI = HttpContext.Current.Application["NomeCliente"].ToString();
			var chave = W3.Library.Encryption.DataHash.GetHashCliente(nomeClienteSI);
			RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
			
			string urlIntegracao = RetornaUrlModulo(enumModulo);
			string codigoFuncionarioAcesso = (string.IsNullOrEmpty(identificador) ? string.Empty : $"?CodigoFuncionarioAcesso={identificador}");
			var client = new RestClient($"{urlIntegracao}/api/tokenLG{codigoFuncionarioAcesso}");

			var request = new RestRequest(Method.POST);
			request.AddHeader("cache-control", "no-cache");
			request.AddHeader("content-type", "application/x-www-form-urlencoded");
			request.AddParameter("client_credentials", $"grant_type=client_credentials&client_id={ nomeClienteSI }&client_secret={ chave }", "application/text", ParameterType.RequestBody);
			IRestResponse response = client.Execute(request);

			var authToken = deserial.Deserialize<OAuthToken>(response);
			var tokenString = authToken?.access_token ?? string.Empty;

			return tokenString;
		}

		public static string RetornaUrlModulo(EnumModulo enumModulo)
		{
            var codigoModulo = (int)enumModulo;
            var modulo = Colecao<Modulo>.Instance.Itens.Where(p => p.CodModulo == codigoModulo).FirstOrDefault();
            string nomeClienteSI = HttpContext.Current.Application["UrlSI"].ToString();
			string strModulos = (enumModulo == EnumModulo.SistemaIntegrado ? string.Empty : "Modulos/");
            var url = $"{ nomeClienteSI + strModulos + modulo.DscUrl }".Replace("/ASP/", "/");
            switch (enumModulo)
            {
                case EnumModulo.MetasNovo:
					url = url.Replace("W3.Metas", "W3.Metas.Novo");
					break;
                default:
                    break;
            }
            return url;
		}
	}
	
}