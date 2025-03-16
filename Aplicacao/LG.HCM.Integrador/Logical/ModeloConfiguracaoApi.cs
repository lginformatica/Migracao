using System.Collections.Generic;
using System.Data;
using W3.Library.DataSql;


namespace LG.HCM.Integrador.Logical {
    public class ModeloConfiguracaoApi {
		public List<Model.ModeloConfiguracaoApi> Listar(int codigoApi) {
			List<Model.ModeloConfiguracaoApi> modeloConfiguracoesApi = new List<Model.ModeloConfiguracaoApi>();

			var sql = DatabaseUtil.Connector.BindSql(@"
				select cod_api, cod_modulo, cod_modelo 
				from {owner-IN}modelo_configuracao_api
				where cod_api = ?
			").ToParam("@codigoApi", codigoApi);

			using (IDataReader reader = sql.AsDataReader()) {
				while (reader.Read()) {
					Model.ModeloConfiguracaoApi modeloConfiguacoes = new Model.ModeloConfiguracaoApi();
					modeloConfiguacoes.CodigoApi = reader.GetValue<int>("cod_api");
					modeloConfiguacoes.CodigoModulo = reader.GetValue<int>("cod_modulo");
					modeloConfiguacoes.CodigoModelo = reader.GetValue<int>("cod_modelo");

					modeloConfiguracoesApi.Add(modeloConfiguacoes);
				}
			}

			return modeloConfiguracoesApi;
		}
	}
}
