namespace LG.HCM.Integrador.Model {
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using W3.Library.DataSql;

    public class Modulos {
        /// <summary>
        /// Lista de Ciclos de competências
        /// </summary>
        /// <returns>Listagem de Ciclos de competências cadastrados</returns>
        public static List<Ciclo> ListarCiclosCompetencias() {
            List<Ciclo> listaCiclos = new List<Ciclo>();

            string sql = @"
             SELECT COD_CICLO_AVALIATIVO, {owner-SI}FN_RECUPERAR_TEXTO_IDIOMA(NOM_CICLO_AVALIATIVO, ?) AS NOM_CICLO_AVALIATIVO, DAT_INICIO, DAT_FIM 
             FROM {owner-AC}CICLO_AVALIATIVO ";

            listaCiclos = DatabaseUtil.Connector.BindSql(sql)
                .ToParam("@idioma", W3.Framework.Servico.Colecao.Utilitarios.Util.RetornarIdiomaCorrente())
                .AsDataTable()
                .AsEnumerable()
                .Select(x => new Ciclo {
                    CodCiclo = x.Field<int>("COD_CICLO_AVALIATIVO"),
                    NomeCiclo = String.IsNullOrEmpty(x.Field<string>("NOM_CICLO_AVALIATIVO")) ? String.Empty : x.Field<string>("NOM_CICLO_AVALIATIVO"),
                    DataInicio = x.Field<DateTime>("DAT_INICIO"),
                    DataFim = x.Field<DateTime>("DAT_FIM")

                }).ToList();

            return listaCiclos;
        }

        public static List<Modelo> ListarModelosCompetencias() {
            List<Modelo> modelos = new List<Modelo>();

            var sql = DatabaseUtil.Connector.BindSql(@"
				SELECT COD_MODELO, {owner-SI}FN_RECUPERAR_TEXTO_IDIOMA(NOM_MODELO, ?) AS NOM_MODELO
				FROM {owner-AC}MODELO 
				ORDER BY DAT_INICIO DESC, COD_MODELO DESC 
			").ToParam("@idioma", W3.Framework.Servico.Colecao.Utilitarios.Util.RetornarIdiomaCorrente());

            using (IDataReader reader = sql.AsDataReader()) {
                while (reader.Read()) {
                    Modelo modelo = new Modelo();
                    modelo.Codigo = reader.GetValue<int>("COD_MODELO");
                    modelo.Nome = reader.GetValue<string>("NOM_MODELO");

                    modelos.Add(modelo);
                }
            }

            return modelos;
        }

        public static List<RegraVerAvaliacao> ListarRegraVerAvaliacao() {
            List<RegraVerAvaliacao> regrasVerAvaliacao = new List<RegraVerAvaliacao>();

            var sql = DatabaseUtil.Connector.BindSql(@"
				SELECT COD_REGRA_VER_AVALIACAO, W3SI.FN_RECUPERAR_TEXTO_IDIOMA(NOM_REGRA_VER_AVALIACAO, ?) AS NOM_REGRA_VER_AVALIACAO 
				FROM W3AC.REGRA_VER_AVALIACAO 
				WHERE IND_TIPO_REGRA_VER_AVALIACAO = 'A' 
			").ToParam("@idioma", W3.Framework.Servico.Colecao.Utilitarios.Util.RetornarIdiomaCorrente());

            using (IDataReader reader = sql.AsDataReader()) {
                while (reader.Read()) {
                    RegraVerAvaliacao regraVerAvaliacao = new RegraVerAvaliacao();
                    regraVerAvaliacao.Codigo = reader.GetValue<int>("COD_REGRA_VER_AVALIACAO");
                    regraVerAvaliacao.Nome = reader.GetValue<string>("NOM_REGRA_VER_AVALIACAO");

                    regrasVerAvaliacao.Add(regraVerAvaliacao);
                }
            }

            return regrasVerAvaliacao;
        }
    }
}
