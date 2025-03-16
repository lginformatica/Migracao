using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using W3.Framework.Servico.Colecao;
using W3.Framework.Servico.Colecao.Classes;
using W3.Library.Data;

namespace LG.HCM.Integrador.Classes {
    public class ConfiguracaoIntegracao : BaseItem {

        #region Propriedades basicas
        public Int32 CodConfiguracaoIntegracao { get; set; }
        public Int32 CodTipoIntegracao { get; set; }
        public String IdConfiguracaoIntegracao { get; set; }
        public String VlrConfiguracaoIntegracao { get; set; }
        public DateTime DatAlteracao { get; set; }
        #endregion

        #region Metodos
        public static List<ConfiguracaoIntegracao> Listar() {
            var configuracaoIntegracao = Colecao<ConfiguracaoIntegracao>.Instance.Itens.ToList();
            return configuracaoIntegracao;
        }

        public static List<ConfiguracaoIntegracao> Buscar(EnumTipoIntegracao tipoIntegracao) {
            var configuracaoIntegracao = Colecao<ConfiguracaoIntegracao>.Instance.Itens.Where(p => p.CodTipoIntegracao == ((int)tipoIntegracao)).ToList();
            return configuracaoIntegracao;
        }

        public static ConfiguracaoIntegracao Buscar(EnumTipoIntegracao tipoIntegracao, EnumIDConfiguracaoIntegracao idConfiguracaoIntegracao) {
            var configuracaoIntegracao = Colecao<ConfiguracaoIntegracao>.Instance.Itens.Where(p => p.CodTipoIntegracao == ((int)tipoIntegracao) && p.IdConfiguracaoIntegracao == idConfiguracaoIntegracao.ToString()).FirstOrDefault();
            return configuracaoIntegracao;
        }

        public static ConfiguracaoIntegracao BuscarConfiguracaoIntegracao(EnumTipoIntegracao tipoIntegracao, EnumIDConfiguracaoIntegracao idConfiguracaoIntegracao) {

            DataTable dados =  DatabaseUtil.Connector.BindSql(
                @"select ci.cod_configuracao_integracao, ci.cod_tipo_integracao, ci.id_configuracao_integracao, ci.vlr_configuracao_integracao, ci.dat_alteracao 
                from {owner-IN}configuracao_integracao ci
                inner join {owner-IN}tipo_integracao ti on ti.cod_tipo_integracao = ci.cod_tipo_integracao
                where ti.cod_tipo_integracao = ? and ci.id_configuracao_integracao = ? ")
                .ToParam("@tipoIntegracao", (int)tipoIntegracao)
                .ToParam("@idConfiguracaoIntegracao", idConfiguracaoIntegracao.ToString())
                .AsDataTable();

            ConfiguracaoIntegracao configuracao = new ConfiguracaoIntegracao();

            if (dados.Rows.Count > 0) {
                DataRow row = dados.Rows[0];
                configuracao.CodConfiguracaoIntegracao = Convert.ToInt32(row["cod_configuracao_integracao"]);
                configuracao.CodTipoIntegracao = Convert.ToInt32(row["cod_tipo_integracao"]);
                configuracao.IdConfiguracaoIntegracao = Convert.ToString(row["id_configuracao_integracao"]);
                configuracao.VlrConfiguracaoIntegracao = Convert.ToString(row["vlr_configuracao_integracao"]);
                configuracao.DatAlteracao = Convert.ToDateTime(row["dat_alteracao"]);
            }

            return configuracao;
        }

        public static void Atualizar(EnumTipoIntegracao tipoIntegracao, EnumIDConfiguracaoIntegracao idConfiguracaoIntegracao, string vlrConfiguracaoIntegracao) {
            var configuracaoIntegracao = Colecao<ConfiguracaoIntegracao>.Instance.Itens.Where(p => p.CodTipoIntegracao == ((int)tipoIntegracao) && p.IdConfiguracaoIntegracao == idConfiguracaoIntegracao.ToString()).FirstOrDefault();
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            parametros.Add("CodTipoIntegracao", (int)tipoIntegracao);
            parametros.Add("IdConfiguracaoIntegracao", idConfiguracaoIntegracao);
            parametros.Add("VlrConfiguracaoIntegracao", vlrConfiguracaoIntegracao);
            Colecoes.Colecao("ConfiguracaoIntegracao").ExecutarQuery("atualizar", parametros);
        }
        #endregion

        #region Tipo Configuração
        public enum EnumIDConfiguracaoIntegracao {
            CiclosCompetenciasAutoatendimento,
            UrlIntegracaoSA3,
            UrlBase,
            ChavePublica,
            ChavePrivada
        }
        #endregion

    }
}