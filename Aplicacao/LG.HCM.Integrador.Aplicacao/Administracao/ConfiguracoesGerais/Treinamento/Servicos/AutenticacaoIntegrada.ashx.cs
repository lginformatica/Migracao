using LG.HCM.Integrador.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;
using W3.Framework.Servico.Classes.Formulario;
using W3.Framework.Servico.Colecao.Utilitarios;
using W3.Framework.Servico.ServicoColecao;
using W3.Library.DataSql;

namespace LG.HCM.Integrador.Aplicacao.Administracao.ConfiguracoesGerais.Treinamento.Servicos {
    /// <summary>
    /// Summary description for ConfigurarProfile
    /// </summary>
    public class AutenticacaoIntegrada : ServicoBaseCadastro {
        public const string ID_CONFIGURACAO_CHAVE_EGURU = Classes.Parametros.ID_CONFIGURACAO_CHAVE_EGURU;
        public const string ID_CONFIGURACAO_URL_EGURU_SSO = Classes.Parametros.ID_CONFIGURACAO_URL_EGURU_SSO;


        public object RetornarFormularios() {
            var formulario = RetornarFormularioHtml();

            XDocument xDocFormulario = XDocument.Parse(formulario);
            XElement xForm = xDocFormulario.Descendants("div").Where(p => (string)p.Attribute("id") == "formGeral").FirstOrDefault();

            Dictionary<string, object> conteudo = new Dictionary<string, object>();
            var configuracaoChaveEguru = Classes.Parametros.Buscar(ID_CONFIGURACAO_CHAVE_EGURU);
            var configuracaoUrlEguruSSO = Classes.Parametros.Buscar(ID_CONFIGURACAO_URL_EGURU_SSO);

            if (configuracaoChaveEguru != null) {
                conteudo.Add(ID_CONFIGURACAO_CHAVE_EGURU, new { value = "ValorParametro" });
            }

            if (configuracaoUrlEguruSSO != null) {
                conteudo.Add(ID_CONFIGURACAO_URL_EGURU_SSO, new { value = configuracaoUrlEguruSSO.VlrParametro });
            }


            xForm = ParseFormulario(xForm, conteudo);

            return new { OK = true, Geral = xForm.ToString() };
        }

        public object SalvarAutenticacaoIntegrada() {
            Validacao validacao = new Validacao();
            validacao.OK = true;

            try {
                DatabaseUtil.Connector.BeginTransaction();

                var vlrConfiguracaoChaveEguru = Convert.ToString(Parametros[ID_CONFIGURACAO_CHAVE_EGURU]);
                var vlrConfiguracaoUrlEguruSSO = Convert.ToString(Parametros[ID_CONFIGURACAO_URL_EGURU_SSO]);

                if (vlrConfiguracaoChaveEguru != "ValorParametro") {
                    Classes.Parametros.Atualizar(ID_CONFIGURACAO_CHAVE_EGURU, W3.Library.Encryption.DataEncryption.EncryptText(vlrConfiguracaoChaveEguru));
                }

                Classes.Parametros.Atualizar(ID_CONFIGURACAO_URL_EGURU_SSO, vlrConfiguracaoUrlEguruSSO);
                DatabaseUtil.Connector.CommitTransaction();

                string urlServico = string.Format("{0}Modulos/LG.HCM.Integrador/W3.Framework/Ferramentas/Parametro/servicos/Parametro.ashx/SetarParametros", HttpContext.Current.Application["UrlSI"].ToString());

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlServico);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            }
            catch (Exception ex) {
                DatabaseUtil.Connector.RollbackTransaction();
                validacao.OK = false;

                validacao.Erros.Add(new ErroValidacao { Msg = ex.Message, NomColuna = "Erro salvar" });

                throw ex;
            }

            return new { OK = validacao.OK, Msg = validacao.Erros.Select(p => new { Nome = p.NomColuna, Msg = p.Msg }).ToList() };
        }
    }

}