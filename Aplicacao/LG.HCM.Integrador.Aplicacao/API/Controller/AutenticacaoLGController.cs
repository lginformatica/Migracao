using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using LG.HCM.Integrador.Logical;
using LG.HCM.Integrador.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using Swashbuckle.Swagger.Annotations;
using W3.Framework.API.Filters;
using System.Web.Http.Cors;
using System.Web;
using LG.HCM.Integrador.Classes;
using static LG.HCM.Integrador.Classes.ConfiguracaoIntegracao;

namespace LG.HCM.Integrador.Aplicacao.API.Controller {
    /// <summary>
    /// API para tratar a geração de tokens para integração com os módulos da LG Nuvem - Nova Geração
    /// </summary>
    [RoutePrefix("api/autenticacaolg")]
    //[EnableCors(origins: "http://op.w3n.com.br", headers: "*", methods: "*")]
    public class AutenticacaoLGController : W3.Framework.API.BaseWebApi {
        /// <summary>
        /// Método para retorno do token de integração com o autoatendimento
        /// </summary>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "Post realizado com sucesso!", typeof(string))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Erro na validação dos parametros")]
        [SwaggerResponse(HttpStatusCode.Forbidden, "Acesso Negado")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error")]
        [SwaggerResponseRemoveDefaults()]
        [HttpPost]
        [Route("autoatendimento/token")]
        [AllowAnonymous()]
        public async Task<IHttpActionResult> AutenticacaoAutoatendimento() {
            // TODO:: Substituir pela url de validação do token da LG. Cadastro de configuração do profile
            // api.autoatendimento/token
            var configuracaoIntegracao = ConfiguracaoIntegracao.Buscar(EnumTipoIntegracao.Autoatendimento, EnumIDConfiguracaoIntegracao.UrlIntegracaoSA3);
            var urlValidacao = configuracaoIntegracao.VlrConfiguracaoIntegracao;
            var baseAddress = new Uri(urlValidacao);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            using (var handler = new HttpClientHandler()) {
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress }) {

                    if (Request.Headers.Contains("Authorization")) {
                        var header = Request.Headers.Authorization;
                        client.DefaultRequestHeaders.Add("Authorization", header.ToString());
                        try {
                            var resultado = await client.PostAsync(urlValidacao, null);

                            if (resultado.IsSuccessStatusCode) {
                                try {
                                    var jsonSettings = new JsonSerializerSettings() {
                                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                                    };

                                    var jsonResult = await resultado.Content.ReadAsStringAsync();
                                    var retornoValidacao = JsonConvert.DeserializeObject<RetornoIntegracao>(jsonResult, jsonSettings);

                                    if (retornoValidacao != null && !string.IsNullOrEmpty(retornoValidacao.Dados)) {
                                        var retornoAA = JsonConvert.DeserializeObject<ClientAndSecretResult>(IntegracaoAutoAtendimento.Decrypt(retornoValidacao.Dados), jsonSettings);
                                        //identificador, secretId, clientId
                                        string token = LG.HCM.Integrador.Logical.Autenticacao.GerarTokenLG(EnumModulo.Integrador, retornoAA.Identificador);

                                        return Json(new {
                                            token
                                        });
                                    }
                                }
                                catch (Exception ex) {
                                    return BadRequest(ex.Message);
                                }
                            }
                        }
                        catch (Exception ex) {
                            return BadRequest(ex.Message);
                        }

                    }
                }
            }

            return Unauthorized();
        }

        /// <summary>
        /// Método para retorno do token de integração com o autoatendimento
        /// </summary>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "Post realizado com sucesso!", typeof(string))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Erro na validação dos parametros")]
        [SwaggerResponse(HttpStatusCode.Forbidden, "Acesso Negado")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error")]
        [SwaggerResponseRemoveDefaults()]
        [HttpGet]
        [Route("autoatendimento/validatoken")]
        [Authorize()]
        public async Task<IHttpActionResult> ValidacaoToken()
        {
            return Ok();
        }
    }


    public class RetornoIntegracao {
        public string Dados { get; set; }
    }

    public class ClientAndSecretResult {
        public string ClientId { get; set; }
        public string SecretId { get; set; }
        public string Identificador { get; set; }
    }
}