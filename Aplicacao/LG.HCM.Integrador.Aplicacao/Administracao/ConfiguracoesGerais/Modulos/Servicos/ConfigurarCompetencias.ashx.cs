using LG.HCM.Integrador.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using W3.Framework.Servico.ServicoColecao;
using W3.Library.DataSql;
using static LG.HCM.Integrador.Classes.ConfiguracaoIntegracao;
namespace LG.HCM.Integrador.Aplicacao.Administracao.ConfiguracoesGerais.Modulos.Competencias.Servicos {
	/// <summary>
	/// Summary description for ConfigurarCompetencias
	/// </summary>
	public class ConfigurarCompetencias : ServicoBaseCadastro {

		/// <summary>
		/// Retorna o formulário de ciclos para configuração do usuário
		/// </summary>
		/// <returns></returns>
		public object RetornarFormularios() {
			var formulario = RetornarFormularioHtml();

			XDocument xDocFormulario = XDocument.Parse(formulario);

			XElement xForm = xDocFormulario.Descendants("div").Where(p => (string)p.Attribute("id") == "formGeral").FirstOrDefault();

			return new { OK = true, Geral = xForm.ToString() };
		}


		/// <summary>
		/// Salva as alterações realizadas pelo usuário
		/// </summary>
		/// <returns>Status da alteração</returns>
		public object SalvarConfiguracaoIntegracao() {
			try {
				List<string> listaCiclosSelecionados = new List<string>();

				foreach (var key in Parametros.Keys) {
					if (key.ToString().Equals("cb_TodosCiclos")) {
						continue;
					}

					bool selected = false;

					string id = key.Substring("cbCiclo_".Length);

					if (bool.TryParse(Parametros[key].ToString(), out selected)) {

						if (selected) {
							listaCiclosSelecionados.Add(id);
						}
					}
				}

				DatabaseUtil.Connector.BeginTransaction();

				ConfiguracaoIntegracao.Atualizar(EnumTipoIntegracao.Autoatendimento, EnumIDConfiguracaoIntegracao.CiclosCompetenciasAutoatendimento, string.Join<string>(";", listaCiclosSelecionados));

				DatabaseUtil.Connector.CommitTransaction();
			}
			catch (Exception ex) {
				DatabaseUtil.Connector.RollbackTransaction();
				throw ex;
			}

			return new { OK = true, Msg = Msg("dados_salvos_com_sucesso") };
		}

		/// <summary>
		/// Monta a lista de ciclos cadastrados, verificando quais estão selecionados
		/// para retornar informações para o portal de autoatendimento
		/// </summary>
		/// <returns>Lista de Ciclos</returns>
		public List<object> RetornaListaCiclos() {
			List<string> listaCodigosMarcados = new List<string>();

			List<Model.Ciclo> lista = LG.HCM.Integrador.Model.Modulos.ListarCiclosCompetencias()
				.OrderByDescending(o => o.DataInicio)
				.ThenByDescending(o => o.DataFim)
				.ThenByDescending(o => o.CodCiclo)
				.ToList();

			ConfiguracaoIntegracao confIntegracao = ConfiguracaoIntegracao.Buscar(EnumTipoIntegracao.Autoatendimento, EnumIDConfiguracaoIntegracao.CiclosCompetenciasAutoatendimento);

			if (confIntegracao != null) {
				listaCodigosMarcados = confIntegracao.VlrConfiguracaoIntegracao.Split(';').ToList();
			}

			List<object> objForms = (from c in lista
									 select new {
										 CodCiclo = c.CodCiclo,
										 NomCiclo = WebUtility.HtmlEncode(c.NomeCiclo),
										 IndMarcado = listaCodigosMarcados.Contains(c.CodCiclo.ToString()) ? "S" : "N",
										 DataInicio = c.DataInicio.ToShortDateString(),
										 DataFim = c.DataFim.ToShortDateString()
									 }).ToList<object>();

			return objForms;
		}
	}
}