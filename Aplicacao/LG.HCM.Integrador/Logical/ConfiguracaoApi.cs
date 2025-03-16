using System;
using System.Collections.Generic;
using System.Data;
using W3.Library.DataSql;

namespace LG.HCM.Integrador.Logical {
	public class ConfiguracaoApi {
		public List<Model.ConfiguracaoApi> Listar(int codigoApi) {
			List<Model.ConfiguracaoApi> configuracoesApi = new List<Model.ConfiguracaoApi>();

			var sql = DatabaseUtil.Connector.BindSql(@"
				select cod_api, cod_modulo, cod_regra_ver_avaliacao, ind_tipo_validacao_ac, 
					ind_nota_origem_ac, ind_nota_origem_rc, ind_nota_origem_me, ind_expiracao_resultados, 
					ind_ano_referencia, ind_range_especifico, qtd_dias_expiracao, num_ano_referencia, 
					vlr_minimo_range, vlr_maximo_range
				from {owner-IN}configuracao_api
				where cod_api = ?
			").ToParam("@codigoApi", codigoApi);

			using (IDataReader reader = sql.AsDataReader()) {
				while (reader.Read()) {
					Model.ConfiguracaoApi configModulo = new Model.ConfiguracaoApi();
					configModulo.CodigoApi = reader.GetValue<int>("cod_api");
					configModulo.CodigoModulo = reader.GetValue<int>("cod_modulo");
					configModulo.CodigoRegraVerAvaliacao = reader.GetValue<int?>("cod_regra_ver_avaliacao");

					configModulo.TipoValidacaoCompetencias = reader.GetValue<string>("ind_tipo_validacao_ac");

					configModulo.TipoNotaOrigemCompetencias = reader.GetValue<string>("ind_nota_origem_ac");
					configModulo.TipoNotaOrigemCalibragem = reader.GetValue<string>("ind_nota_origem_rc");
					configModulo.TipoNotaOrigemMetas = reader.GetValue<string>("ind_nota_origem_me");

					configModulo.ExpirarResultadosPorTempo = reader.GetValue<string>("ind_expiracao_resultados").ToUpper().Equals("S");
					configModulo.UtilizarAnoDeReferencia = reader.GetValue<string>("ind_ano_referencia").ToUpper().Equals("S");
					configModulo.UtilizarRangeEspecifico = reader.GetValue<string>("ind_range_especifico").ToUpper().Equals("S");

					configModulo.QuantidadeDiasExpiracao = reader.GetValue<int?>("qtd_dias_expiracao");
					configModulo.AnoDeReferencia = reader.GetValue<int?>("num_ano_referencia");
					configModulo.ValorMinimoRange = reader.GetValue<double?>("vlr_minimo_range");
					configModulo.ValorMaximoRange = reader.GetValue<double?>("vlr_maximo_range");

					configuracoesApi.Add(configModulo);
				}
			}

			return configuracoesApi;
		}

		public void Inserir(int codigoApi, int codigoModulo, Model.ConfiguracaoApi configuracaoApi) {

			var sql = DatabaseUtil.Connector.BindSql(@"
				insert into {owner-IN}configuracao_api (cod_api, cod_modulo, cod_regra_ver_avaliacao, ind_tipo_validacao_ac, ind_nota_origem_ac, ind_nota_origem_rc, ind_nota_origem_me, ind_expiracao_resultados, ind_ano_referencia, ind_range_especifico, qtd_dias_expiracao, num_ano_referencia, vlr_minimo_range, vlr_maximo_range) 
				values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
			").ToParam("@codigoApi", codigoApi)
			  .ToParam("@codigoModulo", codigoModulo)
			  .ToParam("@codRegraVerAvaliacao", (object)configuracaoApi.CodigoRegraVerAvaliacao ?? DBNull.Value)
			  .ToParam("@indTipoValidacaoAC", (object)configuracaoApi.TipoValidacaoCompetencias ?? DBNull.Value)
			  .ToParam("@indNotaOrigemAC", (object)configuracaoApi.TipoNotaOrigemCompetencias ?? DBNull.Value)
			  .ToParam("@indNotaOrigemRC", (object)configuracaoApi.TipoNotaOrigemCalibragem ?? DBNull.Value)
			  .ToParam("@indNotaOrigemME", (object)configuracaoApi.TipoNotaOrigemMetas ?? DBNull.Value)
			  .ToParam("@indExpiracaoResultados", configuracaoApi.ExpirarResultadosPorTempo ? "S" : "N")
			  .ToParam("@indAnoReferencia", configuracaoApi.UtilizarAnoDeReferencia ? "S" : "N")
			  .ToParam("@indRangeEspecifico", configuracaoApi.UtilizarRangeEspecifico ? "S" : "N")
			  .ToParam("@qtdDiasExpiracao", (object)configuracaoApi.QuantidadeDiasExpiracao ?? DBNull.Value)
			  .ToParam("@numAnoReferencia", (object)configuracaoApi.AnoDeReferencia ?? DBNull.Value)
			  .ToParam("@vlrMinimoRange", (object)configuracaoApi.ValorMinimoRange ?? DBNull.Value)
			  .ToParam("@vlrMaximoRange", (object)configuracaoApi.ValorMaximoRange ?? DBNull.Value);

			sql.Execute();
		}

		public void Excluir(int codigoApi, int codigoModulo) {
			var sql = DatabaseUtil.Connector.BindSql(@"
				delete 
				from {owner-IN}configuracao_api 
				where cod_api = ? and cod_modulo = ? 
			").ToParam("@codigoApi", codigoApi)
			  .ToParam("@codigoModulo", codigoModulo);

			sql.Execute();
		}
	}
}
