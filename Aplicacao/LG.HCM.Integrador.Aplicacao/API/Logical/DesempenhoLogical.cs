using LG.HCM.Integrador.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using W3.Framework.Servico.Colecao;
using W3.Framework.Servico.Colecao.DadosCorporativos;
using W3.Library.DataSql;
using W3.Library.Exceptions;
using W3.Library.Logical.Converter;

namespace LG.HCM.Integrador.Aplicacao.API.Logical {
	/// <summary>
	/// Classe lógica para obtênção do resultado de desempenho do colaborador
	/// </summary>
	public class DesempenhoLogical {
		private const string mensagemSemResultado = "<idioma><pt-BR>Sem Resultado</pt-BR><en-US>No results</en-US><es-ES>No hay resultados</es-ES></idioma>";
		/// <summary>
		/// Método para retorno do resultado de desempenho do colaborador
		/// </summary>
		/// <param name="codigoFuncionario"></param>
		/// <returns></returns>
		public static LG.HCM.Integrador.Api.View.Desempenho RetornarDesempenho(string codigoFuncionario) {
			try {

				//Recupera as informações caso possua plugin configurado
				bool existsCustom = PlugIn.Context.Current.ChecRequestResultadoDesempenho();
				LG.HCM.Integrador.Api.View.DesempenhoArgs args = new LG.HCM.Integrador.Api.View.DesempenhoArgs();
				args.CodigoFuncionario = codigoFuncionario;

				if (existsCustom) {
					PlugIn.Context.Current.OnRequestResultadoDesempenho(args);
					if (args.Desempenho != null) {
						return args.Desempenho;
					}
				}

				List<LG.HCM.Integrador.Api.View.DesempenhoRetorno> resultados = new List<LG.HCM.Integrador.Api.View.DesempenhoRetorno>();
				LG.HCM.Integrador.Api.View.Desempenho desempenho = new LG.HCM.Integrador.Api.View.Desempenho();
				LG.HCM.Integrador.Model.Funcionario funcionario = RetornarFuncionario(codigoFuncionario);

				//Valida se existe o colaborador
				if (funcionario == null) {
					return desempenho;
				}

				LG.HCM.Integrador.Logical.ConfiguracaoApi configuracaoApiLogical = new Integrador.Logical.ConfiguracaoApi();
				LG.HCM.Integrador.Logical.ConfiguracaoApiModelo configuracaoApiModeloLogical = new Integrador.Logical.ConfiguracaoApiModelo();
				var configuracoesPorModulo = configuracaoApiLogical.Listar((int)ApisComConfiguracao.WorkflowMovimentacaoEDesligamento);
				var modelosConfigurados = configuracaoApiModeloLogical.Listar((int)ApisComConfiguracao.WorkflowMovimentacaoEDesligamento);
				var configGeral = configuracoesPorModulo.Where(c => c.CodigoModulo == (int)LG.HCM.Integrador.Logical.EnumModulo.Integrador).FirstOrDefault();
				int quantidadeDiasExpiracaoResultados = int.MaxValue;
				int anoReferencia = (configGeral != null && configGeral.UtilizarAnoDeReferencia && configGeral.AnoDeReferencia.HasValue) ? configGeral.AnoDeReferencia.Value : 0;

				bool existeCompetencias = LG.HCM.Integrador.Logical.Util.ValidaModulo(LG.HCM.Integrador.Logical.EnumModulo.Competencias) && configuracoesPorModulo.Where(c => c.CodigoModulo == (int)LG.HCM.Integrador.Logical.EnumModulo.Competencias).Any();
				bool existeCalibragem = LG.HCM.Integrador.Logical.Util.ValidaModulo(LG.HCM.Integrador.Logical.EnumModulo.Calibragem) && configuracoesPorModulo.Where(c => c.CodigoModulo == (int)LG.HCM.Integrador.Logical.EnumModulo.Calibragem).Any();
				bool existeMetas = LG.HCM.Integrador.Logical.Util.ValidaModulo(LG.HCM.Integrador.Logical.EnumModulo.Metas) && configuracoesPorModulo.Where(c => c.CodigoModulo == (int)LG.HCM.Integrador.Logical.EnumModulo.Metas).Any();

				//Valida se existe os módulos
				if (existeCompetencias) {
					resultados.Add(RetornarResultadoCompetencias(codigoFuncionario, configuracoesPorModulo, modelosConfigurados, anoReferencia));
				}
				if (existeCalibragem) {
					resultados.Add(RetornarResultadoCalibragem(codigoFuncionario, configuracoesPorModulo, anoReferencia));
				}
				if (existeMetas) {
					resultados.Add(RetornarResultadoMetas(codigoFuncionario, configuracoesPorModulo, anoReferencia));
				}

				if (!resultados.Any()) {
					return new LG.HCM.Integrador.Api.View.Desempenho {
						CodigoFuncionario = funcionario.CodigoFuncionario,
						NomeFuncionario = funcionario.NomeFuncionario,
						ResultadoDesempenho = LG.HCM.Integrador.Logical.Util.ObterNomeIdiomizado(mensagemSemResultado)
					};
				}

				if (configGeral != null && configGeral.ExpirarResultadosPorTempo && configGeral.QuantidadeDiasExpiracao.HasValue) {
					quantidadeDiasExpiracaoResultados = configGeral.QuantidadeDiasExpiracao.Value;
				}

				foreach (var resultado in resultados.Where(r => r.DataInicio != null && r.DataFim != null)) {
					resultado.QuantidadeDiasResultado = (int)(DateTime.Now.Date - resultado.DataFim.Value.Date).TotalDays;
				}

				var resultadoMaisRecente = (from rc in resultados
											where rc.DataInicio != null && rc.DataFim != null
												&& rc.QuantidadeDiasResultado <= quantidadeDiasExpiracaoResultados
											orderby rc.DataFim descending, rc.DataInicio descending
											select rc
										).FirstOrDefault();

				desempenho = resultadoMaisRecente;

				if (desempenho != null) {
					if (configGeral != null && configGeral.UtilizarRangeEspecifico 
						&& resultadoMaisRecente.ValorLimiteInferior.HasValue && resultadoMaisRecente.ValorLimiteSuperior.HasValue) {
						decimal notaComNovoRange = RetornarResultadoPorRange((float)desempenho.NotaDesempenho,
							(float)resultadoMaisRecente.ValorLimiteInferior.Value, (float)resultadoMaisRecente.ValorLimiteSuperior.Value,
							(float)configGeral.ValorMinimoRange.Value, (float)configGeral.ValorMaximoRange.Value);

						desempenho.NotaDesempenho = notaComNovoRange;
					}

					return new LG.HCM.Integrador.Api.View.Desempenho {
						CodigoFuncionario = desempenho.CodigoFuncionario,
						NomeFuncionario = desempenho.NomeFuncionario,
						ResultadoDesempenho = desempenho.ResultadoDesempenho,
						NotaDesempenho = desempenho.NotaDesempenho,
						AnoReferencia = desempenho.AnoReferencia
					};

				}
				else {
					return new LG.HCM.Integrador.Api.View.Desempenho {
						CodigoFuncionario = funcionario.CodigoFuncionario,
						NomeFuncionario = funcionario.NomeFuncionario,
						ResultadoDesempenho = LG.HCM.Integrador.Logical.Util.ObterNomeIdiomizado(mensagemSemResultado)
					};
				}
			}
			catch (Exception e) {
				W3.Library.Log.LogErro.WriteLog(e, HttpContext.Current);
				throw new W3ForbiddenException("Erro ao retornar as informações de desempenho do colaborador");
			}
		}

		private static LG.HCM.Integrador.Api.View.DesempenhoRetorno RetornarResultadoCalibragem(string codigoFuncionario, List<Model.ConfiguracaoApi> configuracoes, int anoReferencia) {
			LG.HCM.Integrador.Api.View.DesempenhoRetorno desempenho = new LG.HCM.Integrador.Api.View.DesempenhoRetorno();
			var configRc = configuracoes.Where(c => c.CodigoModulo == (int)LG.HCM.Integrador.Logical.EnumModulo.Calibragem).FirstOrDefault();

			if (configRc != null && !string.IsNullOrEmpty(configRc.TipoNotaOrigemCalibragem)) {
				SqlStatement sql;

				sql = DatabaseUtil.Connector.BindSql("EXEC {owner-RC}PR_RETORNAR_RESULTADO_DESEMPENHO ?, ?, ?").
						ToParam("@codFuncionarioExterno", codigoFuncionario).
						ToParam("@indEixoResultado", configRc.TipoNotaOrigemCalibragem).
						ToParam("@numAnoReferencia", anoReferencia);

				sql.Command.CommandTimeout = 300;
				using (IDataReader reader = sql.AsDataReader()) {
					if (reader.Read()) {
						desempenho.CodigoFuncionario = reader.GetValue<string>("CodigoFuncionario");
						desempenho.NomeFuncionario = reader.GetValue<string>("NomeFuncionario");
						desempenho.ResultadoDesempenho = LG.HCM.Integrador.Logical.Util.ObterNomeIdiomizado(reader.GetValue<string>("ResultadoDesempenho"));
						desempenho.NotaDesempenho = reader.GetValue<decimal?>("NotaDesempenho");
						desempenho.DataInicio = reader.GetValue<DateTime?>("DataInicio");
						desempenho.DataFim = reader.GetValue<DateTime?>("DataFim");
						desempenho.AnoReferencia = reader.GetValue<int?>("AnoReferencia");
						desempenho.ValorLimiteInferior = reader.GetValue<double?>("ValorLimiteInferior");
						desempenho.ValorLimiteSuperior = reader.GetValue<double?>("ValorLimiteSuperior");
					}
				}
			}
			return desempenho;
		}

		private static LG.HCM.Integrador.Api.View.DesempenhoRetorno RetornarResultadoCompetencias(string codigoFuncionario, List<Model.ConfiguracaoApi> configuracoes, List<Model.ConfiguracaoApiModelo> modelos, int anoReferencia) {
			LG.HCM.Integrador.Api.View.DesempenhoRetorno desempenho = new LG.HCM.Integrador.Api.View.DesempenhoRetorno();
			string modelosConfigurados = string.Empty;

			var configAc = configuracoes.Where(c => c.CodigoModulo == (int)LG.HCM.Integrador.Logical.EnumModulo.Competencias).FirstOrDefault();
			modelosConfigurados = modelos.Any() ? string.Join(",", modelos.Select(m => m.CodigoModelo).ToArray()) : "0";

			if (configAc != null && !string.IsNullOrEmpty(configAc.TipoNotaOrigemCompetencias) && !string.IsNullOrEmpty(configAc.TipoValidacaoCompetencias) && modelos.Any()) {
				DatabaseUtil.Connector.SetEncryptionInfo();
				SqlStatement sql;

				sql = DatabaseUtil.Connector.BindSql("EXEC {owner-AC}PR_RETORNAR_RESULTADO_DESEMPENHO ?, ?, ?, ?, ?, ? ").
					ToParam("@codFuncionarioExterno", codigoFuncionario).
					ToParam("@indEixoResultado", configAc.TipoNotaOrigemCompetencias).
					ToParam("@indTipoRegraLiberacao", configAc.TipoValidacaoCompetencias).
					ToParam("@codigoRegraLiberacao", configAc.CodigoRegraVerAvaliacao ?? 0).
					ToParam("@modelosConsiderados", modelosConfigurados).
					ToParam("@numAnoReferencia", anoReferencia);

				sql.Command.CommandTimeout = 300;
				using (IDataReader reader = sql.AsDataReader()) {
					if (reader.Read()) {
						desempenho.CodigoFuncionario = reader.GetValue<string>("CodigoFuncionario");
						desempenho.NomeFuncionario = reader.GetValue<string>("NomeFuncionario");
						desempenho.ResultadoDesempenho = LG.HCM.Integrador.Logical.Util.ObterNomeIdiomizado(reader.GetValue<string>("ResultadoDesempenho"));
						desempenho.NotaDesempenho = reader.GetValue<decimal?>("NotaDesempenho");
						desempenho.DataInicio = reader.GetValue<DateTime?>("DataInicio");
						desempenho.DataFim = reader.GetValue<DateTime?>("DataFim");
						desempenho.AnoReferencia = reader.GetValue<int?>("AnoReferencia");
						desempenho.ValorLimiteInferior = reader.GetValue<double?>("ValorLimiteInferior");
						desempenho.ValorLimiteSuperior = reader.GetValue<double?>("ValorLimiteSuperior");
					}
				}
				DatabaseUtil.Connector.ClearEncryptionInfo();
			}

			return desempenho;
		}

		private static LG.HCM.Integrador.Api.View.DesempenhoRetorno RetornarResultadoMetas(string codigoFuncionario, List<Model.ConfiguracaoApi> configuracoes, int anoReferencia) {
			LG.HCM.Integrador.Api.View.DesempenhoRetorno desempenho = new LG.HCM.Integrador.Api.View.DesempenhoRetorno();
			var configMe = configuracoes.Where(c => c.CodigoModulo == (int)LG.HCM.Integrador.Logical.EnumModulo.Metas).FirstOrDefault();

			if (configMe != null) {
				SqlStatement sql;

				sql = DatabaseUtil.Connector.BindSql("EXEC {owner-ME}PR_RETORNAR_RESULTADO_DESEMPENHO ?, ? ").
					ToParam("@codFuncionarioExterno", codigoFuncionario).
					ToParam("@numAnoReferencia", anoReferencia);

				sql.Command.CommandTimeout = 300;

				using (IDataReader reader = sql.AsDataReader()) {
					if (reader.Read()) {
						desempenho.CodigoFuncionario = reader.GetValue<string>("CodigoFuncionario");
						desempenho.NomeFuncionario = reader.GetValue<string>("NomeFuncionario");
						desempenho.ResultadoDesempenho = LG.HCM.Integrador.Logical.Util.ObterNomeIdiomizado(reader.GetValue<string>("ResultadoDesempenho"));
						desempenho.NotaDesempenho = reader.GetValue<decimal?>("NotaDesempenho");
						desempenho.DataInicio = reader.GetValue<DateTime?>("DataInicio");
						desempenho.DataFim = reader.GetValue<DateTime?>("DataFim");
						desempenho.AnoReferencia = reader.GetValue<int?>("AnoReferencia");
						desempenho.ValorLimiteInferior = reader.GetValue<double?>("ValorLimiteInferior");
						desempenho.ValorLimiteSuperior = reader.GetValue<double?>("ValorLimiteSuperior");
					}
				}
			}

			return desempenho;
		}

		private static decimal RetornarResultadoPorRange(float valor, float rangeMinimoOriginal, float rangeMaximoOriginal, float rangeMinimoConversao, float rangeMaximoConversao) {
			decimal valorConvertido = Convert.ToDecimal(valor);
			SqlStatement sql;

			sql = DatabaseUtil.Connector.BindSql("SELECT {owner-IN}FN_CALCULAR_MEDIA_RELATIVA(?, ?, ?, ?, ?) as vlr_resultado_desempenho ").
				ToParam("@valor", (double)valor).
				ToParam("@rangeMinimoOriginal", (double)rangeMinimoOriginal).
				ToParam("@rangeMaximoOriginal", (double)rangeMaximoOriginal).
				ToParam("@rangeMinimoConversao", (double)rangeMinimoConversao).
				ToParam("@rangeMaximoConversao", (double)rangeMaximoConversao);

			using (IDataReader reader = sql.AsDataReader()) {
				if (reader.Read()) {
					valorConvertido = reader.GetValue<decimal>("vlr_resultado_desempenho");
				}
			}
			return valorConvertido;
		}

		public static Model.Funcionario RetornarFuncionario(string codigoFuncionario) {
			Model.Funcionario funcionario = null;

			var dadosFuncionario = Colecao<VwFuncionario>.Instance.Itens.Where(p => p.CodFuncionarioExterno == codigoFuncionario).FirstOrDefault();
			if (dadosFuncionario != null)
			{
				funcionario = new Model.Funcionario();
				funcionario.CodigoFuncionario = dadosFuncionario.CodFuncionarioExterno;
				funcionario.NomeFuncionario = dadosFuncionario.NomFuncionario;
			}

			return funcionario;
		}
	}
}