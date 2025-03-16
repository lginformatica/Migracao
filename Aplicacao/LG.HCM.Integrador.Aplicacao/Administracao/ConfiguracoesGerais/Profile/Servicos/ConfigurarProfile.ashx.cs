using LG.HCM.Integrador.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using W3.Framework.Servico.Classes.Formulario;
using W3.Framework.Servico.Colecao.Utilitarios;
using W3.Framework.Servico.ServicoColecao;
using W3.Library.DataSql;
using static LG.HCM.Integrador.Classes.ConfiguracaoIntegracao;

namespace LG.HCM.Integrador.Aplicacao.Administracao.ConfiguracoesGerais.Profile.Servicos
{
    /// <summary>
    /// Summary description for ConfigurarProfile
    /// </summary>
    public class ConfigurarProfile : ServicoBaseCadastro {

		public object RetornarFormularios() {
			var formulario = RetornarFormularioHtml();

			XDocument xDocFormulario = XDocument.Parse(formulario);
			XElement xForm = xDocFormulario.Descendants("div").Where(p => (string)p.Attribute("id") == "formGeral").FirstOrDefault();

			Dictionary<string, object> conteudo = new Dictionary<string, object>();
			var configuracaoIntegracao = ConfiguracaoIntegracao.Buscar(EnumTipoIntegracao.Autoatendimento, EnumIDConfiguracaoIntegracao.UrlIntegracaoSA3);

			if (configuracaoIntegracao != null)
				conteudo.Add(EnumIDConfiguracaoIntegracao.UrlIntegracaoSA3.ToString(), new { value = configuracaoIntegracao.VlrConfiguracaoIntegracao });

			xForm = ParseFormulario(xForm, conteudo);

			return new { OK = true, Geral = xForm.ToString() };
		}

        public ClRetornoValidacao Validar()
        {
            List<ClValidador> validador = new List<ClValidador>();

            ClMensagemErro mensagensErro = new ClMensagemErro
            {
                PreenchimentoObrigatorio = Msg("campo_preenchimento_obrigatorio")
            };

            var vlrConfiguracaoIntegracao = Convert.ToString(Parametros[EnumIDConfiguracaoIntegracao.UrlIntegracaoSA3.ToString()]);
            validador.Add(new ClValidador()
            {
                Id = EnumIDConfiguracaoIntegracao.UrlIntegracaoSA3.ToString(),
                Tipo = EnumTipoCampo.String,
                IndObrigatorio = true
            });

            ClRetornoValidacao validacao = ValidarFormulario(validador, Parametros, mensagensErro);

            return validacao;
        }

        public object SalvarConfiguracaoIntegracao()
        {
            Validacao validacao = new Validacao();

            ClRetornoValidacao retornoValidacao = this.Validar();
            validacao.Erros = retornoValidacao.Erros;
            validacao.OK = (validacao.Erros.Count == 0);

            if (validacao.Erros.Count == 0)
            {
                try
                {
                    DatabaseUtil.Connector.BeginTransaction();

                    var vlrConfiguracaoIntegracao = Convert.ToString(Parametros[EnumIDConfiguracaoIntegracao.UrlIntegracaoSA3.ToString()]);
                    ConfiguracaoIntegracao.Atualizar(EnumTipoIntegracao.Autoatendimento, EnumIDConfiguracaoIntegracao.UrlIntegracaoSA3, vlrConfiguracaoIntegracao);

                    DatabaseUtil.Connector.CommitTransaction();
                }
                catch (Exception ex)
                {
                    DatabaseUtil.Connector.RollbackTransaction();
                    throw ex;
                }
            }

            return new { OK = validacao.OK, Msg = validacao.Erros.Select(p => new { Nome = p.NomColuna, Msg = p.Msg }).ToList() };
        }
    }

}