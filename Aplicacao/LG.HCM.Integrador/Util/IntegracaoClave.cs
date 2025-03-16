using System;

namespace LG.HCM.Integrador.Util {
	public class IntegracaoClave {
		public static int ObterClienteIntegracao() {
			int codigoClienteClave = 0;
			string query = String.Concat(
				"select top 1 cod_cliente from {owner-SU}integracao_clave");

			using (W3.Library.DataSql.DatabaseConnector conn = new W3.Library.DataSql.DatabaseConnector()) {
				var retornoSql = conn.BindSql(query).AsValue();
				if (retornoSql != null) {
					codigoClienteClave = Convert.ToInt32(retornoSql);
				}
			};

			return codigoClienteClave;
		}
	}
}
