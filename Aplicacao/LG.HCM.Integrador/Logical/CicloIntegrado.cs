using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using W3.Framework.Servico.Colecao;
using W3.Framework.Servico.Colecao.Classes;
using W3.Library.DataSql;

namespace LG.HCM.Integrador.Logical {
    public class CicloIntegrado {
        public static bool ValidarParametros(string sglIdioma) {
            if (Colecao<Idioma>.Instance.Itens.Where(x => x.SglIdioma.Equals(sglIdioma)).Count() == 0) {
                return false;
            }

            return true;
        }

        public static DataTable BuscarIdiomizado(string sglIdioma) {
            return DatabaseUtil.Connector.BindSql(
                "select cod_ciclo, {owner-SI}fn_recuperar_texto_idioma(nom_ciclo_avaliativo, ?) as nom_ciclo_avaliativo, num_ordem, dat_inicio, dat_fim, ind_utilizar_resultado from {owner-SI}ciclo")
                .ToParam("@SglIdioma", sglIdioma)
                .AsDataTable();
        }

        public DataTable BuscarCicloModuloIntegrado(int codCiclo) {
            return DatabaseUtil.Connector.BindSql(
                "select c.cod_ciclo, c.cod_modulo, c.cod_ciclo_modulo from {owner-SI}ciclo_modulo as c where c.cod_ciclo = ?")
                .ToParam("@CodCiclo", codCiclo)
                .AsDataTable();
        }

        public List<LG.HCM.Integrador.Api.View.CicloIntegrado> ListarCiclosIntegrados(string sglIdioma) {
            DataTable dt = BuscarIdiomizado(sglIdioma);

            List<LG.HCM.Integrador.Api.View.CicloIntegrado> lista = new List<LG.HCM.Integrador.Api.View.CicloIntegrado>();
            foreach (DataRow row in dt.Rows) {
                LG.HCM.Integrador.Api.View.CicloIntegrado cicloIntegrado = new LG.HCM.Integrador.Api.View.CicloIntegrado();

                cicloIntegrado.codigoCiclo = Convert.ToInt32(row["COD_CICLO"]);
                cicloIntegrado.nomeCiclo = row["NOM_CICLO_AVALIATIVO"].ToString();
                cicloIntegrado.numeroOrdem = Convert.ToInt32(row["NUM_ORDEM"]);
                cicloIntegrado.dataInicio = DateTime.Parse(row["DAT_INICIO"].ToString());
                cicloIntegrado.dataFim = DateTime.Parse(row["DAT_FIM"].ToString());

                if (row["IND_UTILIZAR_RESULTADO"].ToString() == "S") {
                    cicloIntegrado.compoeResultado = true;
                }
                else if (row["IND_UTILIZAR_RESULTADO"].ToString() == "N") {
                    cicloIntegrado.compoeResultado = false;
                }

                DataTable dt_integrado = BuscarCicloModuloIntegrado(cicloIntegrado.codigoCiclo);

                List<LG.HCM.Integrador.Api.View.CicloIntegradoModulo> ciclosIntegrados = new List<LG.HCM.Integrador.Api.View.CicloIntegradoModulo>();

                foreach (DataRow rowIntegrado in dt_integrado.Rows) {
                    LG.HCM.Integrador.Api.View.CicloIntegradoModulo cicloIntegradoModulo = new LG.HCM.Integrador.Api.View.CicloIntegradoModulo();

                    cicloIntegradoModulo.codigoCicloModulo = Convert.ToInt32(rowIntegrado["COD_CICLO_MODULO"]);
                    cicloIntegradoModulo.codigoModulo = Convert.ToInt32(rowIntegrado["COD_MODULO"]);
                    ciclosIntegrados.Add(cicloIntegradoModulo);
                }
                cicloIntegrado.ciclosIntegrados = ciclosIntegrados;

                lista.Add(cicloIntegrado);
            }

            DateTime dataAtual = DateTime.Now;

            var cicloAtual = lista
                .Where(ciclo => dataAtual >= ciclo.dataInicio && dataAtual <= ciclo.dataFim)
                .OrderByDescending(ciclo => ciclo.dataInicio)
                .ThenByDescending(ciclo => ciclo.dataFim)
                .ThenByDescending(ciclo => ciclo.codigoCiclo)
                .FirstOrDefault();

            if (cicloAtual != null) {
                cicloAtual.cicloAtual = true;
            }

            return lista;
        }
    }
}