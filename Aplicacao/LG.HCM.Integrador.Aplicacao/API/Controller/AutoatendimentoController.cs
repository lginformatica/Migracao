using LG.HCM.Integrador.Classes;
using LG.HCM.Integrador.Logical;
using LG.HCM.Integrador.Model;
using RestSharp;
using RestSharp.Authenticators;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Http;
using W3.Framework.API.Filters;
using static LG.HCM.Integrador.Classes.ConfiguracaoIntegracao;

namespace LG.HCM.Integrador.Aplicacao.API.Controller {
    /// <summary>
    /// Autoatendimento
    /// </summary>
    [RoutePrefix("api/autoatendimento")]
    public class AutoatendimentoController : W3.Framework.API.BaseWebApi {

        /// <summary>
        /// A API retornará os ciclos vigentes nos quais a equipe direta do funcionário informado possui participação na avaliação de metas
        /// e tambem nos quais o funcionário informado participa de avaliação de metas 
        /// </summary>
        /// <param name="CodigoFuncionarioAcesso">Código Externo do Funcionário que está acessando os dados. É um valor do tipo string</param>
        /// <param name="CodigoFuncionario">Código Externo do Funcionário do qual os dados serão retornados. É um valor do tipo string</param>
        /// <param name="SglIdioma">Sigla do Idioma do retorno, é um valor do tipo string</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "Get realizado com sucesso!", typeof(LG.Metas.Api.View.Autoatendimento.Ciclo[]))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Erro na validação dos registros")]
        [SwaggerResponse(HttpStatusCode.Forbidden, "Acesso Negado")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error")]
        [SwaggerResponseRemoveDefaults()]
        [HttpGet]
        [Route("ciclosMetas/{CodigoFuncionarioAcesso}/{CodigoFuncionario}/{SglIdioma}")]
        [GravarLogFilter(ServiceName = "autoatendimento/ciclosMetas")]
        [Authorize]
        public IHttpActionResult RetornarCiclosMetas(string CodigoFuncionarioAcesso, string CodigoFuncionario, string SglIdioma) {

            if (!LG.HCM.Integrador.Logical.Util.ValidaModulo(LG.HCM.Integrador.Logical.EnumModulo.Metas)) {
                return BadRequest("Módulo não instalado");
            }

            RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
            var request = new RestRequest(string.Format("api/autoatendimento/ciclosMetas/{0}/{1}/{2}/", CodigoFuncionarioAcesso, CodigoFuncionario, SglIdioma), Method.GET);

            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-type", "application/json");

            var authApi = LG.HCM.Integrador.Logical.Autenticacao.Autenticar(EnumModulo.MetasNovo);
            var integracaoApi = authApi.Execute(request);

            if (integracaoApi.StatusCode == HttpStatusCode.OK) {
                var retorno = deserial.Deserialize<List<LG.Metas.Api.View.Autoatendimento.Ciclo>>(integracaoApi);
                return Ok(retorno);
            }

            return new LG.HCM.Integrador.Logical.Util().RetornaActionResult(integracaoApi);
        }

        /// <summary>
        /// A API retornará o resultado da média da equipe no ciclo, do funcionário informado no parâmetro “CodigoFuncionario”.
		/// e tambem vai retornar o resultado do ciclo, dos subordinados do funcionario informado no parâmetro “CodigoFuncionario”, no mesmo ciclo de Metas
        /// </summary>
        /// <param name="CodigoFuncionarioAcesso">Código Externo do Funcionário que está acessando os dados. É um valor do tipo string</param>
        /// <param name="CodigoFuncionario">Código Externo do Funcionário do qual os dados serão retornados. É um valor do tipo string</param>
        /// <param name="CodigoCiclo">Código do ciclo de competencias. É um valor do tipo inteiro</param>
        /// <param name="SglIdioma">Sigla do Idioma do retorno, é um valor do tipo string</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "Get realizado com sucesso!", typeof(LG.Metas.Api.View.Autoatendimento.MetasMinhaEquipe))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Erro na validação dos registros")]
        [SwaggerResponse(HttpStatusCode.Forbidden, "Acesso Negado")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error")]
        [SwaggerResponseRemoveDefaults()]
        [HttpGet]
        [Route("resultadoMetasMinhaEquipe/{CodigoFuncionarioAcesso}/{CodigoFuncionario}/{CodigoCiclo}/{SglIdioma}")]
        [GravarLogFilter(ServiceName = "autoatendimento/resultadoMetasMinhaEquipe")]
        [Authorize]
        public IHttpActionResult RetornarMetasEquipe(string CodigoFuncionarioAcesso, string CodigoFuncionario, int CodigoCiclo, string SglIdioma) {

            if (!LG.HCM.Integrador.Logical.Util.ValidaModulo(LG.HCM.Integrador.Logical.EnumModulo.Metas)) {
                return BadRequest("Módulo não instalado");
            }

            RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
            var request = new RestRequest(string.Format("api/autoatendimento/resultadoMetasMinhaEquipe/{0}/{1}/{2}/{3}/", CodigoFuncionarioAcesso, CodigoFuncionario, CodigoCiclo, SglIdioma), Method.GET);

            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-type", "application/json");

            var authApi = LG.HCM.Integrador.Logical.Autenticacao.Autenticar(EnumModulo.MetasNovo);
            var integracaoApi = authApi.Execute(request);

            if (integracaoApi.StatusCode == HttpStatusCode.OK) {
                var retorno = deserial.Deserialize<LG.Metas.Api.View.Autoatendimento.MetasMinhaEquipe>(integracaoApi);
                return Ok(retorno);
            }

            return new LG.HCM.Integrador.Logical.Util().RetornaActionResult(integracaoApi);
        }

        /// <summary>
        /// A API retornará o resultado do ciclo, do funcionário informado no parâmetro “CodigoFuncionario”
		/// e tambem vai retornar a lista das metas, do funcionário informado no parâmetro “CodigoFuncionario”
        /// </summary>
        /// <param name="CodigoFuncionarioAcesso">Código Externo do Funcionário que está acessando os dados. É um valor do tipo string</param>
        /// <param name="CodigoFuncionario">Código Externo do Funcionário do qual os dados serão retornados. É um valor do tipo string</param>
        /// <param name="CodigoCiclo">Código do ciclo de competencias. É um valor do tipo inteiro</param>
        /// <param name="SglIdioma">Sigla do Idioma do retorno, é um valor do tipo string</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "Get realizado com sucesso!", typeof(LG.Metas.Api.View.Autoatendimento.MinhasMetas))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Erro na validação dos registros")]
        [SwaggerResponse(HttpStatusCode.Forbidden, "Acesso Negado")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error")]
        [SwaggerResponseRemoveDefaults()]
        [HttpGet]
        [Route("resultadoMinhasMetas/{CodigoFuncionarioAcesso}/{CodigoFuncionario}/{CodigoCiclo}/{SglIdioma}")]
        [GravarLogFilter(ServiceName = "autoatendimento/resultadoMinhasMetas")]
        [Authorize]
        public IHttpActionResult RetornarMinhasMetas(string CodigoFuncionarioAcesso, string CodigoFuncionario, int CodigoCiclo, string SglIdioma) {

            if (!LG.HCM.Integrador.Logical.Util.ValidaModulo(LG.HCM.Integrador.Logical.EnumModulo.Metas)) {
                return BadRequest("Módulo não instalado");
            }

            RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
            var request = new RestRequest(string.Format("api/autoatendimento/resultadoMinhasMetas/{0}/{1}/{2}/{3}/", CodigoFuncionarioAcesso, CodigoFuncionario, CodigoCiclo, SglIdioma), Method.GET);

            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-type", "application/json");

            var authApi = LG.HCM.Integrador.Logical.Autenticacao.Autenticar(EnumModulo.MetasNovo);
            var integracaoApi = authApi.Execute(request);

            if (integracaoApi.StatusCode == HttpStatusCode.OK) {
                var retorno = deserial.Deserialize<LG.Metas.Api.View.Autoatendimento.MinhasMetas>(integracaoApi);
                return Ok(retorno);
            }

            return new LG.HCM.Integrador.Logical.Util().RetornaActionResult(integracaoApi);
        }

        /// <summary>
        /// A API retorna todos os ciclos de Competências onde tiver sido configurado que a exibição dos resultados irá compor os widgets do Auto-Atendimento
        /// </summary>
        /// <param name="CodigoFuncionarioAcesso">Código Externo do Funcionário que está acessando os dados. É um valor do tipo string</param>
        /// <param name="CodigoFuncionario">Código Externo do Funcionário do qual os dados serão retornados. É um valor do tipo string</param>
        /// <param name="SglIdioma">Sigla do Idioma do retorno, é um valor do tipo string</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "Get realizado com sucesso!", typeof(LG.Competence.Api.View.Dashboard.Ciclo[]))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Erro na validação dos registros")]
        [SwaggerResponse(HttpStatusCode.Forbidden, "Acesso Negado")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error")]
        [SwaggerResponseRemoveDefaults()]
        [HttpGet]
        [Route("ciclosCompetencias/{CodigoFuncionarioAcesso}/{CodigoFuncionario}/{SglIdioma}")]
        [GravarLogFilter(ServiceName = "autoatendimento/ciclosCompetencias")]
        [Authorize]
        public IHttpActionResult RetornarCiclosCompetencias(string CodigoFuncionarioAcesso, string CodigoFuncionario, string SglIdioma) {

            if (!LG.HCM.Integrador.Logical.Util.ValidaModulo(LG.HCM.Integrador.Logical.EnumModulo.Competencias)) {
                return BadRequest("Módulo não instalado");
            }

            string ciclosCompetenciasAA = ConfiguracaoIntegracao
                .Buscar(EnumTipoIntegracao.Autoatendimento, ConfiguracaoIntegracao.EnumIDConfiguracaoIntegracao.CiclosCompetenciasAutoatendimento)?.VlrConfiguracaoIntegracao ?? "";

            if (string.IsNullOrEmpty(ciclosCompetenciasAA)) {
                return Ok(new List<LG.Competence.Api.View.Dashboard.Ciclo>());
            }

            RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
            var request = new RestRequest(string.Format(
                "api/autoatendimento/ciclosCompetencias/{0}/{1}/{2}/{3}/", 
                CodigoFuncionarioAcesso, CodigoFuncionario, SglIdioma, ciclosCompetenciasAA), 
                Method.GET);

            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-type", "application/json");

            var authApi = LG.HCM.Integrador.Logical.Autenticacao.Autenticar(EnumModulo.Competencias);
            var integracaoApi = authApi.Execute(request);

            if (integracaoApi.StatusCode == HttpStatusCode.OK) {
                var retorno = deserial.Deserialize<List<LG.Competence.Api.View.Dashboard.Ciclo>>(integracaoApi);
                return Ok(retorno);
            }

            return new LG.HCM.Integrador.Logical.Util().RetornaActionResult(integracaoApi);
        }

        /// <summary>
        /// A API é responsável por apresentar os resultados individuais da equipe direta do funcionário alvo passado como parâmetro
        /// </summary>
        /// <param name="CodigoFuncionarioAcesso">Código Externo do Funcionário que está acessando os dados. É um valor do tipo string</param>
        /// <param name="CodigoFuncionario">Código Externo do Funcionário do qual os dados serão retornados. É um valor do tipo string</param>
        /// <param name="CodigoCiclo">Código do ciclo de competencias. É um valor do tipo inteiro</param>
        /// <param name="SglIdioma">Sigla do Idioma do retorno, é um valor do tipo string</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "Get realizado com sucesso!", typeof(LG.Competence.Api.View.Dashboard.ResultadoEquipe))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Erro na validação dos registros")]
        [SwaggerResponse(HttpStatusCode.Forbidden, "Acesso Negado")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error")]
        [SwaggerResponseRemoveDefaults()]
        [HttpGet]
        [Route("resultadoCompetenciasEquipe/{CodigoFuncionarioAcesso}/{CodigoFuncionario}/{CodigoCiclo}/{SglIdioma}")]
        [GravarLogFilter(ServiceName = "autoatendimento/resultadoCompetenciasEquipe")]
        [Authorize]
        public IHttpActionResult RetornarResultadoEquipeCompetencias(string CodigoFuncionarioAcesso, string CodigoFuncionario, int CodigoCiclo, string SglIdioma) {
            
            if (!LG.HCM.Integrador.Logical.Util.ValidaModulo(LG.HCM.Integrador.Logical.EnumModulo.Competencias)) {
                return BadRequest("Módulo não instalado");
            }

            string ciclosCompetenciasAA = ConfiguracaoIntegracao
                .Buscar(EnumTipoIntegracao.Autoatendimento, EnumIDConfiguracaoIntegracao.CiclosCompetenciasAutoatendimento)?.VlrConfiguracaoIntegracao ?? "";

            if (string.IsNullOrEmpty(ciclosCompetenciasAA)) {
                return Ok(new List<LG.Competence.Api.View.Dashboard.AvaliacaoFuncionario>());
            }

            RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
            var request = new RestRequest(string.Format(
                "api/autoatendimento/resultadoCompetenciasEquipe/{0}/{1}/{2}/{3}/{4}/", 
                CodigoFuncionarioAcesso, CodigoFuncionario, CodigoCiclo, SglIdioma, ciclosCompetenciasAA), 
                Method.GET);

            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-type", "application/json");

            var authApi = LG.HCM.Integrador.Logical.Autenticacao.Autenticar(EnumModulo.Competencias);
            var integracaoApi = authApi.Execute(request);

            if (integracaoApi.StatusCode == HttpStatusCode.OK) {
                var retorno = deserial.Deserialize<LG.Competence.Api.View.Dashboard.ResultadoEquipe>(integracaoApi);
                return Ok(retorno);
            }

            return new LG.HCM.Integrador.Logical.Util().RetornaActionResult(integracaoApi);
        }

        /// <summary>
        /// A API é responsável por apresentar os resultados individuais da equipe direta do funcionário alvo passado como parâmetro
        /// </summary>
        /// <param name="CodigoFuncionarioAcesso">Código Externo do Funcionário que está acessando os dados. É um valor do tipo string</param>
        /// <param name="CodigoFuncionario">Código Externo do Funcionário do qual os dados serão retornados. É um valor do tipo string</param>
        /// <param name="SglIdioma">Sigla do Idioma do retorno, é um valor do tipo string</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, "Get realizado com sucesso!", typeof(LG.Competence.Api.View.Dashboard.AvaliacaoFuncionario[]))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Erro na validação dos registros")]
        [SwaggerResponse(HttpStatusCode.Forbidden, "Acesso Negado")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error")]
        [SwaggerResponseRemoveDefaults()]
        [HttpGet]
        [Route("resultadoCompetenciasIndividual/{CodigoFuncionarioAcesso}/{CodigoFuncionario}/{SglIdioma}")]
        [GravarLogFilter(ServiceName = "autoatendimento/resultadoCompetenciasIndividual")]
        [Authorize]
        public IHttpActionResult RetornarResultadoCompetenciasIndividual(string CodigoFuncionarioAcesso, string CodigoFuncionario, string SglIdioma) {

            if (!LG.HCM.Integrador.Logical.Util.ValidaModulo(LG.HCM.Integrador.Logical.EnumModulo.Competencias)) {
                return BadRequest("Módulo não instalado");
            }

            string ciclosCompetenciasAA = ConfiguracaoIntegracao
                .Buscar(EnumTipoIntegracao.Autoatendimento, EnumIDConfiguracaoIntegracao.CiclosCompetenciasAutoatendimento)?.VlrConfiguracaoIntegracao ?? "";

            if (string.IsNullOrEmpty(ciclosCompetenciasAA)) {
                return Ok(new List<LG.Competence.Api.View.Dashboard.AvaliacaoFuncionario>());
            }

            RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
            var request = new RestRequest(string.Format(
                "api/autoatendimento/resultadoCompetenciasIndividual/{0}/{1}/{2}/{3}/",
                CodigoFuncionarioAcesso, CodigoFuncionario, SglIdioma, ciclosCompetenciasAA),
                Method.GET);

            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-type", "application/json");

            var authApi = LG.HCM.Integrador.Logical.Autenticacao.Autenticar(EnumModulo.Competencias);
            var integracaoApi = authApi.Execute(request);

            if (integracaoApi.StatusCode == HttpStatusCode.OK) {
                var retorno = deserial.Deserialize<List<LG.Competence.Api.View.Dashboard.AvaliacaoFuncionario>>(integracaoApi);
                return Ok(retorno);
            }

            return new LG.HCM.Integrador.Logical.Util().RetornaActionResult(integracaoApi);
        }

       
    }

}