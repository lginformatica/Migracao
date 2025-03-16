using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using W3.Library.DataSql;

namespace LG.HCM.Integrador.Logical {
	public class ConfiguracaoApiModelo {
		public List<Model.ConfiguracaoApiModelo> Listar(int codigoApi) {
			List<Model.ConfiguracaoApiModelo> modelosConfigurados = new List<Model.ConfiguracaoApiModelo>();

			var sql = DatabaseUtil.Connector.BindSql(@"
				select cod_api, cod_modulo, cod_modelo 
				from {owner-IN}modelo_configuracao_api 
				where cod_api = ? 
			").ToParam("@codigoApi", codigoApi);

			using (IDataReader reader = sql.AsDataReader()) {
				while (reader.Read()) {
					Model.ConfiguracaoApiModelo configModulo = new Model.ConfiguracaoApiModelo();
					configModulo.CodigoApi = reader.GetValue<int>("cod_api");
					configModulo.CodigoModulo = reader.GetValue<int>("cod_modulo");
					configModulo.CodigoModelo = reader.GetValue<int>("cod_modelo");

					modelosConfigurados.Add(configModulo);
				}
			}

			return modelosConfigurados;
		}

		public void Inserir(int codigoApi, int codigoModulo, int codigoModelo) {

			var sql = DatabaseUtil.Connector.BindSql(@"
				insert into {owner-IN}modelo_configuracao_api (cod_api, cod_modulo, cod_modelo) 
				values (?, ?, ?) 
			").ToParam("@codigoApi", codigoApi)
			  .ToParam("@codigoModulo", codigoModulo)
			  .ToParam("@codModelo", codigoModelo);

			sql.Execute();
		}

		public void Excluir(int codigoApi, int codigoModulo, int codigoModelo) {
			var sql = DatabaseUtil.Connector.BindSql(@"
				delete 
				from {owner-IN}modelo_configuracao_api 
				where cod_api = ? and cod_modulo = ? and cod_modelo = ? 
			").ToParam("@codigoApi", codigoApi)
			  .ToParam("@codigoModulo", codigoModulo)
			  .ToParam("@codigoModelo", codigoModelo);

			sql.Execute();
		}

		public void Excluir(int codigoApi, int codigoModulo) {
			var sql = DatabaseUtil.Connector.BindSql(@"
				delete 
				from {owner-IN}modelo_configuracao_api 
				where cod_api = ? and cod_modulo = ?
			").ToParam("@codigoApi", codigoApi)
			  .ToParam("@codigoModulo", codigoModulo);

			sql.Execute();
		}
	}
}
