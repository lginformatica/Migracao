using LG.HCM.Integrador.Api.View;
using LG.HCM.Integrador.Logical;
using RestSharp;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using W3.Framework.API.Filters;

namespace LG.HCM.Integrador.Aplicacao.API.Controller {
    /// <summary>
    /// Profile
    /// </summary>
    [RoutePrefix("api")]
    public class ProfileController : W3.Framework.API.BaseWebApi {
        /// <summary>
        /// A API retornará o histórico de ações de PDI, referentes á processos livres dos últimos 4 ciclos do qual o colaborador tenha participado
        /// Serão retornados somente ciclos no qual o colaborador tenha tido pelo menos uma ação de PDI registrada por ele ou pelo Gestor 
        /// </summary>
        /// <param name="CodigoFuncionarioAcesso">Código Externo do Funcionário que está acessando os dados. É um valor do tipo string</param>
        /// <param name="CodigoFuncionario">Código Externo do Funcionário do qual os dados serão retornados. É um valor do tipo string</param>
        /// <param name="SglIdioma">Sigla do Idioma do retorno, é um valor do tipo string</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "Get realizado com sucesso!", typeof(LG.PDI.Api.View.HistoricoResultadoPDI[]))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Erro na validação dos registros")]
        [SwaggerResponse(HttpStatusCode.Forbidden, "Acesso Negado")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error")]
        [SwaggerResponseRemoveDefaults()]
        [HttpGet]
        [Route("profile/historicoPdi/{CodigoFuncionarioAcesso}/{CodigoFuncionario}/{SglIdioma}")]
        [GravarLogFilter(ServiceName = "profile/historicoPDI")]
        [Authorize]
        public IHttpActionResult RetornarHistoricoPDI(string CodigoFuncionarioAcesso, string CodigoFuncionario, string SglIdioma) {

            if (!LG.HCM.Integrador.Logical.Util.ValidaModulo(LG.HCM.Integrador.Logical.EnumModulo.Pdi)) {
                return BadRequest("Módulo não instalado");
            }

            RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
            var request = new RestRequest(string.Format("api/profile/{0}/{1}/{2}/", CodigoFuncionarioAcesso, CodigoFuncionario, SglIdioma), Method.GET);

            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-type", "application/json");

            var authApi = LG.HCM.Integrador.Logical.Autenticacao.Autenticar(EnumModulo.Pdi);
            var integracaoApi = authApi.Execute(request);

            if (integracaoApi.StatusCode == HttpStatusCode.OK) {
                var retorno = deserial.Deserialize<List<LG.PDI.Api.View.HistoricoResultadoPDI>>(integracaoApi);
                return Ok(retorno);
            }

            return new LG.HCM.Integrador.Logical.Util().RetornaActionResult(integracaoApi);
        }


        /// <summary>
        /// A API retornará o histórico de resultados de avaliações de Competência, referentes á aos últimos 4 ciclos do qual o colaborador tenha participado
        /// Serão retornados somente ciclos no qual a avaliação do colaborador já possua resultado consolidado 
        /// </summary>
        /// <param name="CodigoFuncionarioAcesso">Código Externo do Funcionário que está acessando os dados. É um valor do tipo string</param>
        /// <param name="CodigoFuncionario">Código Externo do Funcionário do qual os dados serão retornados. É um valor do tipo string</param>
        /// <param name="SglIdioma">Sigla do Idioma do retorno, é um valor do tipo string</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "Get realizado com sucesso!", typeof(LG.Competence.Api.View.ResultadoProfile[]))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Erro na validação dos registros")]
        [SwaggerResponse(HttpStatusCode.Forbidden, "Acesso Negado")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error")]
        [SwaggerResponseRemoveDefaults()]
        [HttpGet]
        [Route("profile/historicoCompetencia/{CodigoFuncionarioAcesso}/{CodigoFuncionario}/{SglIdioma}")]
        [GravarLogFilter(ServiceName = "profile/historicoCompetencia")]
        [Authorize]
        public IHttpActionResult RetornarHistoricoCompetencia(string CodigoFuncionarioAcesso, string CodigoFuncionario, string SglIdioma) {

            if (!LG.HCM.Integrador.Logical.Util.ValidaModulo(LG.HCM.Integrador.Logical.EnumModulo.Competencias)) {
                return BadRequest("Módulo não instalado");
            }

            RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
            var request = new RestRequest(string.Format("api/profile/{0}/{1}/{2}/", CodigoFuncionarioAcesso, CodigoFuncionario, SglIdioma), Method.GET);

            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-type", "application/json");

            var authApi = LG.HCM.Integrador.Logical.Autenticacao.Autenticar(EnumModulo.Competencias);
            var integracaoApi = authApi.Execute(request);

            if (integracaoApi.StatusCode == HttpStatusCode.OK) {
                var retorno = deserial.Deserialize<List<LG.Competence.Api.View.ResultadoProfile>>(integracaoApi);
                return Ok(retorno);
            }

            return new LG.HCM.Integrador.Logical.Util().RetornaActionResult(integracaoApi);
        }


        /// <summary>
        /// A API retornará o histórico de resultados de avaliações de Metas, referentes á aos últimos 4 ciclos do qual o colaborador tenha participado
        /// Serão retornados somente ciclos no qual a avaliação do colaborador já possua resultado final 
        /// </summary>
        /// <param name="CodigoFuncionarioAcesso">Código Externo do Funcionário que está acessando os dados. É um valor do tipo string</param>
        /// <param name="CodigoFuncionario">Código Externo do Funcionário do qual os dados serão retornados. É um valor do tipo string</param>
        /// <param name="SglIdioma">Sigla do Idioma do retorno, é um valor do tipo string</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "Get realizado com sucesso!", typeof(LG.Metas.Api.View.HistoricoResultadoMetas[]))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Erro na validação dos registros")]
        [SwaggerResponse(HttpStatusCode.Forbidden, "Acesso Negado")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error")]
        [SwaggerResponseRemoveDefaults()]
        [HttpGet]
        [Route("profile/historicoMetas/{CodigoFuncionarioAcesso}/{CodigoFuncionario}/{SglIdioma}")]
        [GravarLogFilter(ServiceName = "profile/historicoMetas")]
        [Authorize]
        public IHttpActionResult RetornarHistoricoMetas(string CodigoFuncionarioAcesso, string CodigoFuncionario, string SglIdioma) {

            if (!LG.HCM.Integrador.Logical.Util.ValidaModulo(LG.HCM.Integrador.Logical.EnumModulo.Metas)) {
                return BadRequest("Módulo não instalado");
            }

            RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
            var request = new RestRequest(string.Format("api/profile/{0}/{1}/{2}/", CodigoFuncionarioAcesso, CodigoFuncionario, SglIdioma), Method.GET);

            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-type", "application/json");

            var authApi = LG.HCM.Integrador.Logical.Autenticacao.Autenticar(EnumModulo.MetasNovo);
            var integracaoApi = authApi.Execute(request);

            if (integracaoApi.StatusCode == HttpStatusCode.OK) {
                var retorno = deserial.Deserialize<List<LG.Metas.Api.View.HistoricoResultadoMetas>>(integracaoApi);
                return Ok(retorno);
            }

            return new LG.HCM.Integrador.Logical.Util().RetornaActionResult(integracaoApi);
        }

        /// <summary>
        /// A API retornará o histórico de resultados de avaliações de Calibragem, referentes á aos últimos 4 ciclos do qual o colaborador tenha participado
        /// Serão listados somente turmas que estejam concluídas e nas quais o colaborador enviado no parâmetro “CodigoFuncionarioAcesso” tenha acesso de acordo com a validação de acesso “heathcheck” atualmente aplicado no módulo de Calibragem
        /// </summary>
        /// <param name="CodigoFuncionarioAcesso">Código Externo do Funcionário que está acessando os dados. É um valor do tipo string</param>
        /// <param name="CodigoFuncionario">Código Externo do Funcionário do qual os dados serão retornados. É um valor do tipo string</param>
        /// <param name="SglIdioma">Sigla do Idioma do retorno, é um valor do tipo string</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "Get realizado com sucesso!", typeof(LG.Calibragem.Api.View.ResultadoProfile[]))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Erro na validação dos registros")]
        [SwaggerResponse(HttpStatusCode.Forbidden, "Acesso Negado")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error")]
        [SwaggerResponseRemoveDefaults()]
        [HttpGet]
        [Route("profile/historicoCalibragem/{CodigoFuncionarioAcesso}/{CodigoFuncionario}/{SglIdioma}")]
        [GravarLogFilter(ServiceName = "profile/historicoCalibragem")]
        [Authorize]
        public IHttpActionResult RetornarHistoricoCalibragem(string CodigoFuncionarioAcesso, string CodigoFuncionario, string SglIdioma) {

            if (!LG.HCM.Integrador.Logical.Util.ValidaModulo(LG.HCM.Integrador.Logical.EnumModulo.Calibragem)) {
                return BadRequest("Módulo não instalado");
            }

            RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
            var request = new RestRequest(string.Format("api/profile/{0}/{1}/{2}/", CodigoFuncionarioAcesso, CodigoFuncionario, SglIdioma), Method.GET);

            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-type", "application/json");

            var authApi = LG.HCM.Integrador.Logical.Autenticacao.Autenticar(EnumModulo.Calibragem);
            var integracaoApi = authApi.Execute(request);

            if (integracaoApi.StatusCode == HttpStatusCode.OK) {
                var retorno = deserial.Deserialize<List<LG.Calibragem.Api.View.ResultadoProfile>>(integracaoApi);
                return Ok(retorno);
            }

            return new LG.HCM.Integrador.Logical.Util().RetornaActionResult(integracaoApi);
        }

        /// <summary>
        /// A API retornará a situação do funcionário no último ciclo de Sucessão
        /// </summary>
        /// <param name="CodigoFuncionarioAcesso">Código Externo do Funcionário que está acessando os dados. É um valor do tipo string</param>
        /// <param name="CodigoFuncionario">Código Externo do Funcionário do qual os dados serão retornados. É um valor do tipo string</param>
        /// <param name="SglIdioma">Sigla do Idioma do retorno, é um valor do tipo string</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "Get realizado com sucesso!", typeof(LG.Sucessao.Api.View.ResultadoProfile))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Erro na validação dos registros")]
        [SwaggerResponse(HttpStatusCode.Forbidden, "Acesso Negado")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error")]
        [SwaggerResponseRemoveDefaults()]
        [HttpGet]
        [Route("profile/sucessao/{CodigoFuncionarioAcesso}/{CodigoFuncionario}/{SglIdioma}")]
        [GravarLogFilter(ServiceName = "profile/sucessao")]
        [Authorize]
        public IHttpActionResult RetornarHistoricoSucessao(string CodigoFuncionarioAcesso, string CodigoFuncionario, string SglIdioma) {

            if (!LG.HCM.Integrador.Logical.Util.ValidaModulo(LG.HCM.Integrador.Logical.EnumModulo.Sucessao)) {
                return BadRequest("Módulo não instalado");
            }

            RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
            var request = new RestRequest(string.Format("api/profile/{0}/{1}/{2}/", CodigoFuncionarioAcesso, CodigoFuncionario, SglIdioma), Method.GET);

            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-type", "application/json");

            var authApi = LG.HCM.Integrador.Logical.Autenticacao.Autenticar(EnumModulo.Sucessao);
            var integracaoApi = authApi.Execute(request);

            if (integracaoApi.StatusCode == HttpStatusCode.OK) {
                if (integracaoApi.Content == "null")
                    return Ok();
                else
                    return Ok(deserial.Deserialize<LG.Sucessao.Api.View.ResultadoProfile>(integracaoApi));

            }

            return new LG.HCM.Integrador.Logical.Util().RetornaActionResult(integracaoApi);
        }

        /// <summary>
        /// API para listagem dos resultados integrados, por ciclo, de um determinado colaborador. Como resultado integrado, entende-se os resultados de Competências, Metas e Calibragem do colaborador
        /// </summary>
        /// <param name="CodigoFuncionario">Código Externo do Funcionário do qual os dados serão retornados. É um valor do tipo string</param>
        /// <param name="SglIdioma">Sigla do Idioma do retorno, é um valor do tipo string</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "Get realizado com sucesso!", typeof(LG.HCM.Integrador.Api.View.HistoricoResultadoIntegrado))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Erro na validação dos registros")]
        [SwaggerResponse(HttpStatusCode.Forbidden, "Acesso Negado")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error")]
        [SwaggerResponseRemoveDefaults()]
        [HttpGet]
        [Route("v1/profile/historico/{CodigoFuncionario}/{SglIdioma}")]
        [Authorize]
        public IHttpActionResult BuscarHistoricoResultadoIntegrado(string CodigoFuncionario, string SglIdioma) {
            string token = LG.HCM.Integrador.Logical.Util.RecuperaToken(Request);
            string codigoFuncionarioAcesso = new W3.Framework.API.Classes.W3Autenticacao().RecuperarTokenAcessoApi(token);

            if (!Logical.Profile.ValidarCodigoFuncionario(codigoFuncionarioAcesso)) {
                return BadRequest("Parâmetros inválidos! CodigoFuncionarioAcesso");
            }

            if (!Logical.Profile.ValidarCodigoFuncionario(CodigoFuncionario)) {
                return BadRequest("Parâmetros inválidos! CodigoFuncionario");
            }

            if (!Logical.Profile.ValidarIdioma(SglIdioma)) {
                return BadRequest("Parâmetros inválidos! SglIdioma");
            }

            LG.HCM.Integrador.Logical.CicloIntegrado _cicloIntegrado = new Integrador.Logical.CicloIntegrado();
            var ciclosIntegrados = _cicloIntegrado.ListarCiclosIntegrados(SglIdioma);
            List<LG.HCM.Integrador.Api.View.CicloIntegrado> listaCiclosIntegradosFiltrada = new List<LG.HCM.Integrador.Api.View.CicloIntegrado>();
            List<HistoricoResultadoIntegrado> resultados = new List<HistoricoResultadoIntegrado>();
            List<HistoricoResultadoIntegrado> listaFinal = new List<HistoricoResultadoIntegrado>();

            foreach (LG.HCM.Integrador.Api.View.CicloIntegrado ciclo in ciclosIntegrados) {
                if (ciclo.compoeResultado == true) {
                    listaCiclosIntegradosFiltrada.Add(ciclo);
                }
            }

            List<LG.HCM.Integrador.Api.View.CicloIntegrado> listaCiclosIntegrados = listaCiclosIntegradosFiltrada
                .OrderByDescending(x => x.dataInicio)
                .ThenByDescending(x => x.dataFim)
                .ThenByDescending(x => x.codigoCiclo)
                .Take(4)
                .ToList();

            List<int> codigosCicloMetas = new List<int>();
            List<int> codigosCicloCompetencias = new List<int>();
            List<int> codigosCicloCalibragem = new List<int>();

            foreach (LG.HCM.Integrador.Api.View.CicloIntegrado cicloIntegrado in listaCiclosIntegrados) {
                if (cicloIntegrado.ciclosIntegrados.Where(p => p.codigoModulo == (int)EnumModulo.Metas).Any()) {
                    codigosCicloMetas.Add(cicloIntegrado.ciclosIntegrados.Where(p => p.codigoModulo == (int)EnumModulo.Metas).Select(p => p.codigoCicloModulo).Distinct().First());
                }

                if (cicloIntegrado.ciclosIntegrados.Where(p => p.codigoModulo == (int)EnumModulo.Competencias).Any()) {
                    codigosCicloCompetencias.Add(cicloIntegrado.ciclosIntegrados.Where(p => p.codigoModulo == (int)EnumModulo.Competencias).Select(p => p.codigoCicloModulo).Distinct().First());
                }

                if (cicloIntegrado.ciclosIntegrados.Where(p => p.codigoModulo == (int)EnumModulo.Calibragem).Any()) {
                    codigosCicloCalibragem.Add(cicloIntegrado.ciclosIntegrados.Where(p => p.codigoModulo == (int)EnumModulo.Calibragem).Select(p => p.codigoCicloModulo).Distinct().First());
                }
            }

            List<LG.Metas.Api.View.ResultadoProfileMetas> resultadoMetas = new List<Metas.Api.View.ResultadoProfileMetas>();
            List<LG.Competence.Api.View.HistoricoAvaliacoesIntegrado> resultadoCompetencias = new List<Competence.Api.View.HistoricoAvaliacoesIntegrado>();
            List<LG.Calibragem.Api.View.ResultadoIntegrado> resultadoCalibragem = new List<Calibragem.Api.View.ResultadoIntegrado>();

            if (codigosCicloMetas.Any()) {
                string ciclos = string.Join(",", codigosCicloMetas);
                resultadoMetas = Logical.Profile.RecuperarResultadosCiclosMetas(codigoFuncionarioAcesso, CodigoFuncionario, SglIdioma, ciclos);
            }

            if (codigosCicloCompetencias.Any()) {
                string ciclos = string.Join(",", codigosCicloCompetencias);
                resultadoCompetencias = Logical.Profile.RecuperarResultadosCiclosCompetencias(codigoFuncionarioAcesso, CodigoFuncionario, SglIdioma, ciclos);
            }

            if (codigosCicloCalibragem.Any()) {
                string ciclos = string.Join(",", codigosCicloCalibragem);
                resultadoCalibragem = Logical.Profile.RecuperarResultadosCiclosCalibragem(codigoFuncionarioAcesso, CodigoFuncionario, SglIdioma, ciclos);
            }

            foreach (Api.View.CicloIntegrado cicloIntegrado in listaCiclosIntegrados) {
                var cicloMetas = cicloIntegrado.ciclosIntegrados.Where(p => p.codigoModulo == (int)EnumModulo.Metas).FirstOrDefault();
                var cicloCompetencias = cicloIntegrado.ciclosIntegrados.Where(p => p.codigoModulo == (int)EnumModulo.Competencias).FirstOrDefault();
                var cicloCalibragem = cicloIntegrado.ciclosIntegrados.Where(p => p.codigoModulo == (int)EnumModulo.Calibragem).FirstOrDefault();

                if ((cicloMetas == null || !resultadoMetas.Where(p => p.CodigoCiclo == (cicloMetas?.codigoCicloModulo ?? 0)).Any())
                    && (cicloCompetencias == null || !resultadoCompetencias.Where(p => p.CodigoCiclo == (cicloCompetencias?.codigoCicloModulo ?? 0)).Any())
                    && (cicloCalibragem == null || !resultadoCalibragem.Where(p => p.CodigoCiclo == (cicloCalibragem?.codigoCicloModulo ?? 0)).Any())) {
                    continue;
                }

                Api.View.HistoricoResultadoIntegrado cicloIntegradoResultado = new Api.View.HistoricoResultadoIntegrado();

                cicloIntegradoResultado.Ciclo = cicloIntegrado.nomeCiclo;
                cicloIntegradoResultado.CodigoCiclo = cicloIntegrado.codigoCiclo;
                cicloIntegradoResultado.cicloAtual = cicloIntegrado.cicloAtual;

                if (cicloMetas != null) {
                    cicloIntegradoResultado.resultadoMetas = resultadoMetas.Where(p => p.CodigoCiclo == cicloMetas.codigoCicloModulo).FirstOrDefault();
                }

                if (cicloCompetencias != null) {
                    cicloIntegradoResultado.resultadoCompetencias = resultadoCompetencias.Where(p => p.CodigoCiclo == cicloCompetencias.codigoCicloModulo).FirstOrDefault();
                }

                if (cicloCalibragem != null) {
                    cicloIntegradoResultado.resultadoCalibragem = resultadoCalibragem.Where(p => p.CodigoCiclo == cicloCalibragem.codigoCicloModulo).FirstOrDefault();
                }

                resultados.Add(cicloIntegradoResultado);
            }

            return Ok(resultados);
        }

    }

}