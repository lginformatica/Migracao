using System;
using System.Collections.Generic;
using W3.Framework.Servico;
using System.Linq;
using System.Web;

namespace LG.HCM.Integrador.Aplicacao.Principal.Servicos
{
    /// <summary>
    /// Summary description for ConfiguracoesCiclo
    /// </summary>
    public class ConfiguracoesGerais : ServicoBaseAshx
    {
		/// <summary>
		/// Método utilizado para montar a árvore de seleção para configurações de ciclo
		/// </summary>
		/// <param name="pai">Identificador do nó pai (atributo id)</param>
		/// <returns>Objeto com a estrutura de tree para preenchimento do componente DhtmlxTree</returns>
		public object BuscarTree(string pai) {
			try {
				List<object> tree = new List<object>();

				switch (pai) {
					case "0":

						tree.Add(new { id = "sep_portal_autoatendimento", text = Msg("portal_autoatendimento"), im0 = "W3Icons/simulacao_16.png", im1 = "W3Icons/simulacao_16.png", im2 = "W3Icons/simulacao_16.png", child = true });
						tree.Add(new { id = "sep_configracoes_de_apis", text = Msg("configracoes_de_apis"), im0 = "W3Icons/simulacao_16.png", im1 = "W3Icons/simulacao_16.png", im2 = "W3Icons/simulacao_16.png", child = true });
						tree.Add(new { id = "sep_gente_treinamento", text = Msg("gente_treinamento"), im0 = "W3Icons/simulacao_16.png", im1 = "W3Icons/simulacao_16.png", im2 = "W3Icons/simulacao_16.png", child = true });
						tree.Add(new { id = "sep_fit_comportamento_clave", text = Msg("fit_comportamento_clave"), im0 = "W3Icons/simulacao_16.png", im1 = "W3Icons/simulacao_16.png", im2 = "W3Icons/simulacao_16.png", child = true });

						break;
					case "sep_portal_autoatendimento":
						tree.Add(new { id = "autenticacao_integrada", text = Msg("autenticacao_integrada"), im0 = "W3Icons/configuracao.png", im1 = "W3Icons/configuracao.png", im2 = "W3Icons/configuracao.png", userdata = new object[] { new { name = "url", content = "../../Administracao/ConfiguracoesGerais/AutenticacaoIntegrada/ConfigurarAutenticacaoIntegrada.aspx" } } });
						tree.Add(new { id = "configuracao", text = Msg("configuracao"), im0 = "W3Icons/configuracao.png", im1 = "W3Icons/configuracao.png", im2 = "W3Icons/configuracao.png", userdata = new object[] { new { name = "url", content = "../../Administracao/ConfiguracoesGerais/Profile/ConfigurarProfile.aspx" } } });
						tree.Add(new { id = "competencias", text = Msg("competencias"), im0 = "W3Icons/icoTreeBarra3.gif", im1 = "W3Icons/icoTreeBarra3.gif", im2 = "W3Icons/icoTreeBarra3.gif", userdata = new object[] { new { name = "url", content = "../../Administracao/ConfiguracoesGerais/Modulos/ConfigurarCompetencias.aspx" } } });
					break;
					case "sep_configracoes_de_apis":
						tree.Add(new { id = "workflow_de_movimentacao_e_rescicao", text = Msg("workflow_de_movimentacao_e_rescicao"), im0 = "W3Icons/configuracao.png", im1 = "W3Icons/configuracao.png", im2 = "W3Icons/configuracao.png", userdata = new object[] { new { name = "url", content = "../../Administracao/ConfiguracoesGerais/APIs/WorkflowMovimentacaoRescisao.aspx" } } });
						break;
					case "sep_gente_treinamento":
						tree.Add(new { id = "autenticacao_integrada_treinamento", text = Msg("autenticacao_integrada"), im0 = "W3Icons/configuracao.png", im1 = "W3Icons/configuracao.png", im2 = "W3Icons/configuracao.png", userdata = new object[] { new { name = "url", content = "../../Administracao/ConfiguracoesGerais/Treinamento/AutenticacaoIntegrada.aspx" } } });
						tree.Add(new { id = "dados_corporativos_treinamento", text = Msg("dados_corporativos"), im0 = "W3Icons/configuracao.png", im1 = "W3Icons/configuracao.png", im2 = "W3Icons/configuracao.png", userdata = new object[] { new { name = "url", content = "../../Administracao/ConfiguracoesGerais/Treinamento/DadosCorporativos.aspx" } } });
					break;
					case "sep_fit_comportamento_clave":
						tree.Add(new { id = "configuracao_integracao_clave", text = Msg("configuracao_integracao"), im0 = "W3Icons/configuracao.png", im1 = "W3Icons/configuracao.png", im2 = "W3Icons/configuracao.png", userdata = new object[] { new { name = "url", content = "../../Administracao/ConfiguracoesGerais/Clave/ConfiguracaoIntegracao.aspx" } } });
					break;
				}
				return new { id = pai, item = tree, OK = true };
			}
			catch (Exception erro) {
				return new { OK = false, Msg = string.Format("Erro BuscarTree: {0}", erro.Message) };
			}
		}
	}
}