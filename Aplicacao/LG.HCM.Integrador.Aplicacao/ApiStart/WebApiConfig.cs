using System;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using W3.Framework.API.Documentacao;
using Swashbuckle.Application;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using System.Net.Http.Formatting;
using System.Web.Http.Cors;
using System.Web;

[assembly: OwinStartup("OwinConfiguration", typeof(W3.Framework.API.ApiStart.WebApiConfig))]
namespace W3.Framework.API.ApiStart {
    /// <summary>
    /// Start Web API da aplicação que faz uso do framework API.
    /// </summary>    
    public class WebApiConfig : W3.Framework.API.Start.WebApiStartConfig {
        /// <summary>
        /// Construtor padrão da configurador inicial das web api's.
        /// </summary>        
        public WebApiConfig() : base("Suíte LG HCM - API") { }
        /// <summary>
        /// Método para aplicar configurações especificadas desta web api
        /// </summary>
        protected override void ConfiguracoesAdicionais() {
            // TODO:: Substituir pela url "domínio" que derevá ser liberado no CORS. Cadastro de configuração do profile
            //var cors = new EnableCorsAttribute(origins: "http://op.w3n.com.br", headers: "*", methods: "*");
            //var cors = new EnableCorsAttribute(origins: "https://projetos.w3net.com.br", headers: "*", methods: "*");
            //var cors = new EnableCorsAttribute(origins: "https://tst-aa.lgcloud.com.br,http://op.w3n.com.br", headers: "*", methods: "*");
            //var cors = new EnableCorsAttribute(origins: IntegracaoAutoAtendimento.GetUrlBase(), headers: "*", methods: "*");
            var cors = new EnableCorsAttribute(origins: "*", headers: "*", methods: "*");
            this.Config.EnableCors(cors);

            ConfiguraJsonResult(this.Config);
            this.SwaggerConfig.AddApplicationComments($@"{System.AppDomain.CurrentDomain.BaseDirectory}\bin\LG.HCM.Integrador.Aplicacao.xml");
        }

        private static void ConfiguraJsonResult(HttpConfiguration config) {
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            var defaultSettings = new JsonSerializerSettings {
                Formatting = Formatting.None,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter> {
                    new StringEnumConverter{ CamelCaseText = true },
                }
            };

            JsonConvert.DefaultSettings = () => { return defaultSettings; };
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.JsonFormatter.SerializerSettings = defaultSettings;
        }
    }
}