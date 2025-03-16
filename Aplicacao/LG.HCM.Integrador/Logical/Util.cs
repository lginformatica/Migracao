using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using LG.HCM.Integrador.Model;
using RestSharp;
using W3.Framework.Servico.Colecao;
using W3.Framework.Servico.Colecao.Classes;
using W3.Library.Logical.Converter;

namespace LG.HCM.Integrador.Logical
{
    public class Util
    {
		public static bool ValidaModulo(LG.HCM.Integrador.Logical.EnumModulo Modulo) {
            int codigoModulo = (int)Modulo;

            Modulo modulo = Colecao<Modulo>.Instance.Itens.Where(p => p.CodModulo == codigoModulo).FirstOrDefault();

			return modulo != null;
		}

        public IHttpActionResult RetornaActionResult(IRestResponse response) {
            RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
            var message = "";
            try {
                message = deserial.Deserialize<BadRequestContent>(response).Message;
            }
            catch (Exception) {
                message = "Ocorreu um erro interno.";
            }
            return new HttpActionResult(response.StatusCode, message);
        }

        public static string RecuperaToken(HttpRequestMessage request)
        {
            string token = string.Empty;

            if (request.Headers.Contains("Authorization"))
            {
                token = request.Headers.Authorization.Parameter;
            }
            
            return token;
        }

        public static LG.HCM.Integrador.Api.View.NomeIdiomizado ObterNomeIdiomizado(string nome)
        {
            LG.HCM.Integrador.Api.View.NomeIdiomizado retorno = new LG.HCM.Integrador.Api.View.NomeIdiomizado("", "", "");

            if (!String.IsNullOrEmpty(nome))
            {
                retorno.ptBR = ConverterUtilIdiomizada<XmlToStringConverter>.ConvertToOutput(nome, typeof(string), "pt-br").ToString();
                retorno.enUS = ConverterUtilIdiomizada<XmlToStringConverter>.ConvertToOutput(nome, typeof(string), "en-us").ToString();
                retorno.esES = ConverterUtilIdiomizada<XmlToStringConverter>.ConvertToOutput(nome, typeof(string), "es-es").ToString();
            }

            return retorno;
        }
    }
	
}