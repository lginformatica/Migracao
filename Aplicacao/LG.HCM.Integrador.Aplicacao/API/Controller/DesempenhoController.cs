using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using System.Web.Http;
using LG.HCM.Integrador.Api.View;
using LG.HCM.Integrador.Aplicacao.API;

namespace LG.HCM.Integrador.Aplicacao.API.Controller {
	[RoutePrefix("api/desempenho")]
	public class DesempenhoController : W3.Framework.API.BaseWebApi {
		/// <summary>
		/// Retorna os resultados de desempenho do colaborador
		/// </summary>
		/// <param name="CodigoFuncionario">Código Externo do Colaborador</param>
		/// <remarks>
		/// Esta Api requer token autenticação (protocolo oAuth 2.0).
		/// </remarks>
		[SwaggerResponse(HttpStatusCode.OK, "Get Realizado com sucesso!", typeof(Desempenho))]
		[SwaggerResponse(HttpStatusCode.BadRequest, "Erro na validação dos registros")]
		[SwaggerResponse(HttpStatusCode.Forbidden, "Acesso Negado - Erro no token enviado")]
		[SwaggerResponse(HttpStatusCode.InternalServerError, "Internal Server Error")]
		[SwaggerResponseRemoveDefaults()]
		[Route("{CodigoFuncionario}")]
		[HttpGet]
		[HttpPost]
		[Authorize]
		public IHttpActionResult RetornarHabilidades(string CodigoFuncionario) {

			if (String.IsNullOrEmpty(CodigoFuncionario) || Logical.DesempenhoLogical.RetornarFuncionario(CodigoFuncionario) == null) {
				return NotFound();
			}

			Desempenho desempenho = Logical.DesempenhoLogical.RetornarDesempenho(CodigoFuncionario);

			return Ok(desempenho);
		}
	}
}