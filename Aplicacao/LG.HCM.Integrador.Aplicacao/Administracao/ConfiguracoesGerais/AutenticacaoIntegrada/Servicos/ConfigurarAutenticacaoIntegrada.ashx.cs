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
using static LG.HCM.Integrador.Classes.ConfiguracaoIntegracao;

namespace LG.HCM.Integrador.Aplicacao.Administracao.ConfiguracoesGerais.AutenticacaoIntegrada.Servicos {

    public class ConfigurarAutenticacaoIntegrada : ServicoBaseCadastro {

        public object RetornarFormularios() {
            var formulario = RetornarFormularioHtml();

            XDocument xDocFormulario = XDocument.Parse(formulario);
            XElement xForm = xDocFormulario.Descendants("div").Where(p => (string)p.Attribute("id") == "formGeral").FirstOrDefault();

            Dictionary<string, object> conteudo = new Dictionary<string, object>();
            var autenticacaoIntegracao = ConfiguracaoIntegracao.Buscar(EnumTipoIntegracao.AutenticacaoIntegrada, EnumIDConfiguracaoIntegracao.UrlBase);
            var chavePublica = ConfiguracaoIntegracao.Buscar(EnumTipoIntegracao.AutenticacaoIntegrada, EnumIDConfiguracaoIntegracao.ChavePublica);
            var chavePrivada = ConfiguracaoIntegracao.Buscar(EnumTipoIntegracao.AutenticacaoIntegrada, EnumIDConfiguracaoIntegracao.ChavePrivada);

            if (autenticacaoIntegracao != null) {
                conteudo.Add(EnumIDConfiguracaoIntegracao.UrlBase.ToString(), new { value = autenticacaoIntegracao.VlrConfiguracaoIntegracao });
            }

            if (chavePublica != null) {
                conteudo.Add(EnumIDConfiguracaoIntegracao.ChavePublica.ToString(), new { value = "ValorParametro" });
            }

            if (chavePrivada != null) {
                conteudo.Add(EnumIDConfiguracaoIntegracao.ChavePrivada.ToString(), new { value = "ValorParametro" });
            }

            xForm = ParseFormulario(xForm, conteudo);

            return new { OK = true, Geral = xForm.ToString() };
        }

        public ClRetornoValidacao Validar() {
            List<ClValidador> validador = new List<ClValidador>();

            ClMensagemErro mensagensErro = new ClMensagemErro {
                PreenchimentoObrigatorio = Msg("campo_preenchimento_obrigatorio")
            };

            validador.Add(new ClValidador() {
                Id = EnumIDConfiguracaoIntegracao.UrlBase.ToString(),
                Tipo = EnumTipoCampo.String,
                IndObrigatorio = true
            });

            validador.Add(new ClValidador() {
                Id = EnumIDConfiguracaoIntegracao.ChavePublica.ToString(),
                Tipo = EnumTipoCampo.String,
                IndObrigatorio = true
            });

            validador.Add(new ClValidador() {
                Id = EnumIDConfiguracaoIntegracao.ChavePrivada.ToString(),
                Tipo = EnumTipoCampo.String,
                IndObrigatorio = true
            });

            ClRetornoValidacao validacao = ValidarFormulario(validador, Parametros, mensagensErro);

            return validacao;
        }

        public object SalvarConfigurarAutenticacaoIntegrada() {
            Validacao validacao = new Validacao();

            ClRetornoValidacao retornoValidacao = this.Validar();
            validacao.Erros = retornoValidacao.Erros;
            validacao.OK = (validacao.Erros.Count == 0);

            if (validacao.Erros.Count == 0) {
                try {
                    DatabaseUtil.Connector.BeginTransaction();

                    var vlrUrlBase = Convert.ToString(Parametros[EnumIDConfiguracaoIntegracao.UrlBase.ToString()]);
                    ConfiguracaoIntegracao.Atualizar(EnumTipoIntegracao.AutenticacaoIntegrada, EnumIDConfiguracaoIntegracao.UrlBase, vlrUrlBase);

                    var vlrChavePublica = Convert.ToString(Parametros[EnumIDConfiguracaoIntegracao.ChavePublica.ToString()]);
                    
                    if (vlrChavePublica != "ValorParametro") {
                        ConfiguracaoIntegracao.Atualizar(
                                EnumTipoIntegracao.AutenticacaoIntegrada, 
                                EnumIDConfiguracaoIntegracao.ChavePublica,
                                W3.Library.Encryption.DataEncryption.EncryptText(vlrChavePublica)
                            );
                    }

                    var vlrChavePrivada = Convert.ToString(Parametros[EnumIDConfiguracaoIntegracao.ChavePrivada.ToString()]);

                    if (vlrChavePrivada != "ValorParametro") {
                        ConfiguracaoIntegracao.Atualizar(
                                EnumTipoIntegracao.AutenticacaoIntegrada,
                                EnumIDConfiguracaoIntegracao.ChavePrivada,
                                W3.Library.Encryption.DataEncryption.EncryptText(vlrChavePrivada)
                            );
                    }

                    DatabaseUtil.Connector.CommitTransaction();
                }
                catch (Exception ex) {
                    DatabaseUtil.Connector.RollbackTransaction();
                    throw ex;
                }
            }

            return new { OK = validacao.OK, Msg = validacao.Erros.Select(p => new { Nome = p.NomColuna, Msg = p.Msg }).ToList() };
        }
    }

}