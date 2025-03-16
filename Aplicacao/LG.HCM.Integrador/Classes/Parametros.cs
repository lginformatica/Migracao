using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using W3.Framework.Servico.Colecao;
using W3.Framework.Servico.Colecao.Classes;
using W3.Library.Data;
using System.Data;
using System.Web;

namespace LG.HCM.Integrador.Classes {
    public class Parametros : BaseItem {
        #region Constantes
        public const string ID_CONFIGURACAO_CHAVE_EGURU = "ChaveHashEguru";
        public const string ID_CONFIGURACAO_URL_EGURU_SSO = "UrlSSOEguru";

        public const string ID_CONFIGURACAO_ENVIO_DADOS_CORPORATIVOS_TREINAMENTO = "IntegracaoTreinamento.EnvioDadosCorporativos";
        public const string ID_CONFIGURACAO_URL_BASE_API_TREINAMENTO = "IntegracaoTreinamento.UrlBaseApi";
        public const string ID_CONFIGURACAO_TOKEN_CLIENT_ID_TREINAMENTO = "IntegracaoTreinamento.TokenClientId";
        public const string ID_CONFIGURACAO_TOKEN_CLIENT_SECRET_TREINAMENTO = "IntegracaoTreinamento.TokenClientSecret";

        public const string ID_CONFIGURACAO_ENVIO_DADOS_CORPORATIVOS_CLAVE = "IntegracaoClave.EnvioDadosCorporativos";
        public const string ID_CONFIGURACAO_URL_BASE_API_CLAVE = "IntegracaoClave.UrlBase";
        public const string ID_CONFIGURACAO_TOKEN_CLIENT_SECRET_CLAVE = "IntegracaoClave.Chave";

        #endregion

        #region Propriedades basicas
        public Int32 CodParametro { get; set; }
        public string NomParametro { get; set; }
        public string VlrParametro { get; set; }

        #endregion

        #region Metodos
        public static Parametros Buscar(string idParametro) {

            List<Parametros> lista = new List<Parametros>();

            Dictionary<string, object> parametros = new Dictionary<string, object>();
            parametros.Add("IdParametro", idParametro);
            lista = Colecao<Parametros>.Instance.RetornarQuery("buscar", parametros)
                    .AsEnumerable()
                    .Select(p => new Parametros() {
                        VlrParametro =
                            p.Field<string>("ind_tipo_valor").Equals("C") ? 
                                 W3.Library.Encryption.DataEncryption.DecryptText(p.Field<string>("vlr_parametro")):
                             p.Field<string>("vlr_parametro"),

                        NomParametro = p.Field<string>("nom_parametro")
                    })
                    .ToList();

            return lista.Count > 0 ? lista[0] : null;
        }

        public static void Atualizar(string idParametro, string Vlrparametro) {
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            parametros.Add("IdParametro", idParametro);
            parametros.Add("Vlrparametro", Vlrparametro);
            Colecoes.Colecao("Parametros").ExecutarQuery("atualizar", parametros);
            Colecoes.Colecao("Parametros").AtualizarCache();
        }
        #endregion
    }
}