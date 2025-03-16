using LG.HCM.Integrador.Classes;
using LG.HCM.Integrador.Logical;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using W3.Framework.Servico.Classes.Formulario;
using W3.Framework.Servico.Colecao;
using W3.Framework.Servico.Colecao.Utilitarios;
using W3.Framework.Servico.ServicoColecao;
using W3.Library.DataSql;

namespace LG.HCM.Integrador.Aplicacao.Administracao.ConfiguracoesGerais.APIs.Servicos {
	/// <summary>
	/// Summary description for WorkflowMovimentacaoRescisao
	/// </summary>
	public class WorkflowMovimentacaoRescisao : ServicoBaseCadastro {
		public object RetornarFormularios() {
			var formulario = RetornarFormularioHtml();
			Logical.ConfiguracaoApi configuracaoApi = new Logical.ConfiguracaoApi();
			Logical.ModeloConfiguracaoApi modeloConfiguracaoApi = new Logical.ModeloConfiguracaoApi();
			List<Model.ConfiguracaoApi> configuracaoApis = new List<Model.ConfiguracaoApi>();
			configuracaoApis = configuracaoApi.Listar(Convert.ToInt32(Model.ApisComConfiguracao.WorkflowMovimentacaoEDesligamento));
			Model.ConfiguracaoApi configuracoesIntegrador = configuracaoApis.FirstOrDefault(p => p.CodigoModulo == Convert.ToInt32(Logical.EnumModulo.Integrador));
			Model.ConfiguracaoApi configuracoesCompetencias;
			Model.ConfiguracaoApi configuracoesCalibragem;
			Model.ConfiguracaoApi configuracoesMetas;
			XDocument xDocFormulario = XDocument.Parse(formulario);
			XElement xForm = xDocFormulario.Descendants("div").Where(p => (string)p.Attribute("id") == "formGeral").FirstOrDefault();

			if (LG.HCM.Integrador.Logical.Util.ValidaModulo(Logical.EnumModulo.Competencias)) {
				xDocFormulario.Descendants("div").Where(p => (string)p.Attribute("id") == "Competencias").FirstOrDefault().SetAttributeValue("style", "display: block;");
				configuracoesCompetencias = configuracaoApis.FirstOrDefault(p => p.CodigoModulo == Convert.ToInt32(Logical.EnumModulo.Competencias));
				XElement xSelectNotaDeOrigemCompetencias = xDocFormulario.Descendants("select").FirstOrDefault(p => (string)p.Attribute("id") == "SelNotaDeOrigemCompetencias");

				if (configuracoesCompetencias != null) {
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "CkCompetencias").FirstOrDefault().SetAttributeValue("checked", "checked");

					if (xSelectNotaDeOrigemCompetencias != null) {
						if (!string.IsNullOrEmpty(configuracoesCompetencias.TipoNotaOrigemCompetencias)) {
							xSelectNotaDeOrigemCompetencias.Add(XElement.Parse(string.Format("<option value=\"{0}\" {1}>{2}</option>", (char)Model.OrigemResultadoDesempenho.NotaDoEixoX, configuracoesCompetencias.TipoNotaOrigemCompetencias == ((char)Model.OrigemResultadoDesempenho.NotaDoEixoX).ToString() ? "selected='selected'" : "", Msg("nota_ciclo_origem_eixo_x_competencias"))));
							xSelectNotaDeOrigemCompetencias.Add(XElement.Parse(string.Format("<option value=\"{0}\" {1}>{2}</option>", (char)Model.OrigemResultadoDesempenho.NotaDoEixoY, configuracoesCompetencias.TipoNotaOrigemCompetencias == ((char)Model.OrigemResultadoDesempenho.NotaDoEixoY).ToString() ? "selected='selected'" : "", Msg("nota_ciclo_origem_eixo_y_competencias"))));
							xSelectNotaDeOrigemCompetencias.Add(XElement.Parse(string.Format("<option value=\"{0}\" {1}>{2}</option>", (char)Model.OrigemResultadoDesempenho.NotaDoEixoZ, configuracoesCompetencias.TipoNotaOrigemCompetencias == ((char)Model.OrigemResultadoDesempenho.NotaDoEixoZ).ToString() ? "selected='selected'" : "", Msg("nota_ciclo_origem_eixo_z_competencias"))));
						}
						else {
							xSelectNotaDeOrigemCompetencias.Add(XElement.Parse(string.Format("<option value=\"{0}\" {1}>{2}</option>", (char)Model.OrigemResultadoDesempenho.NotaDoEixoZ, "selected='selected'", Msg("nota_ciclo_origem_eixo_z_competencias"))));
						}
					}

					if (!string.IsNullOrEmpty(configuracoesCompetencias.TipoValidacaoCompetencias)) {
						if (configuracoesCompetencias.TipoValidacaoCompetencias == ((char)Model.TipoValidacaoCompetencias.ResultadoConsolidado).ToString())
							xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "RadioJaExistaResultadoConsolidado").FirstOrDefault().SetAttributeValue("checked", "checked");

						if (configuracoesCompetencias.TipoValidacaoCompetencias == ((char)Model.TipoValidacaoCompetencias.RegraDoVerAvaliacao).ToString())
							xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "RadioRegraVerAvaliacao").FirstOrDefault().SetAttributeValue("checked", "checked");

						if (configuracoesCompetencias.TipoValidacaoCompetencias == ((char)Model.TipoValidacaoCompetencias.RegraEspecifica).ToString()) {
							xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "RadioRegraEspecifica").FirstOrDefault().SetAttributeValue("checked", "checked");
							xDocFormulario.Descendants("select").Where(p => (string)p.Attribute("id") == "SelRegraEspecifica").FirstOrDefault().Attribute("disabled").Remove();
						}
					}
					else {
						xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "RadioJaExistaResultadoConsolidado").FirstOrDefault().SetAttributeValue("checked", "checked");
					}
				}
				else {
					xSelectNotaDeOrigemCompetencias.Add(XElement.Parse(string.Format("<option value=\"{0}\" >{1}</option>", (char)Model.OrigemResultadoDesempenho.NotaDoEixoX, Msg("nota_ciclo_origem_eixo_x_competencias"))));
					xSelectNotaDeOrigemCompetencias.Add(XElement.Parse(string.Format("<option value=\"{0}\" >{1}</option>", (char)Model.OrigemResultadoDesempenho.NotaDoEixoY, Msg("nota_ciclo_origem_eixo_y_competencias"))));
					xSelectNotaDeOrigemCompetencias.Add(XElement.Parse(string.Format("<option value=\"{0}\" {1}>{2}</option>", (char)Model.OrigemResultadoDesempenho.NotaDoEixoZ, "selected='selected'", Msg("nota_ciclo_origem_eixo_z_competencias"))));
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "RadioJaExistaResultadoConsolidado").FirstOrDefault().SetAttributeValue("checked", "checked");
					xDocFormulario.Descendants("select").Where(p => (string)p.Attribute("id") == "SelModelosCompetencias").FirstOrDefault().SetAttributeValue("disabled", "disabled");
					xDocFormulario.Descendants("select").Where(p => (string)p.Attribute("id") == "SelNotaDeOrigemCompetencias").FirstOrDefault().SetAttributeValue("disabled", "disabled");
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "RadioJaExistaResultadoConsolidado").FirstOrDefault().SetAttributeValue("disabled", "disabled");
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "RadioRegraVerAvaliacao").FirstOrDefault().SetAttributeValue("disabled", "disabled");
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "RadioRegraEspecifica").FirstOrDefault().SetAttributeValue("disabled", "disabled");
				}

				XElement xSelectRegraEspecifica = xDocFormulario.Descendants("select").FirstOrDefault(p => (string)p.Attribute("id") == "SelRegraEspecifica");
				if (xSelectRegraEspecifica != null) {
					var regrasVerAvaliacao = Model.Modulos.ListarRegraVerAvaliacao();
					if (regrasVerAvaliacao != null) {
						xSelectRegraEspecifica.Add(XElement.Parse("<option value=\"0\">" + Msg("selecione") + "...</option>"));

						foreach (var regraVerAvaliacao in regrasVerAvaliacao) {
							xSelectRegraEspecifica.Add(XElement.Parse(string.Format("<option value=\"{0}\" {1}>{2}</option>", regraVerAvaliacao.Codigo, regraVerAvaliacao.Codigo == configuracoesCompetencias?.CodigoRegraVerAvaliacao ? "selected='selected'" : "", regraVerAvaliacao.Nome)));
						}
					}
				}
			}

			if (LG.HCM.Integrador.Logical.Util.ValidaModulo(Logical.EnumModulo.Calibragem)) {
				xDocFormulario.Descendants("div").Where(p => (string)p.Attribute("id") == "Calibragem").FirstOrDefault().SetAttributeValue("style", "display: block;");
				configuracoesCalibragem = configuracaoApis.FirstOrDefault(p => p.CodigoModulo == Convert.ToInt32(Logical.EnumModulo.Calibragem));
				XElement xSelectNotaDeOrigemCalibragem = xDocFormulario.Descendants("select").FirstOrDefault(p => (string)p.Attribute("id") == "SelNotaDeOrigemCalibragem");

				if (configuracoesCalibragem != null) {
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "CkCalibragem").FirstOrDefault().SetAttributeValue("checked", "checked");

					if (xSelectNotaDeOrigemCalibragem != null) {
						if (!string.IsNullOrEmpty(configuracoesCalibragem.TipoNotaOrigemCalibragem)) {
							xSelectNotaDeOrigemCalibragem.Add(XElement.Parse(string.Format("<option value=\"{0}\" {1}>{2}</option>", (char)Model.OrigemResultadoDesempenho.NotaDoEixoX, configuracoesCalibragem.TipoNotaOrigemCalibragem == ((char)Model.OrigemResultadoDesempenho.NotaDoEixoX).ToString() ? "selected='selected'" : "", Msg("nota_ciclo_origem_eixo_x_calibragem"))));
							xSelectNotaDeOrigemCalibragem.Add(XElement.Parse(string.Format("<option value=\"{0}\" {1}>{2}</option>", (char)Model.OrigemResultadoDesempenho.NotaDoEixoY, configuracoesCalibragem.TipoNotaOrigemCalibragem == ((char)Model.OrigemResultadoDesempenho.NotaDoEixoY).ToString() ? "selected='selected'" : "", Msg("nota_ciclo_origem_eixo_y_calibragem"))));
						}
						else {
							xSelectNotaDeOrigemCalibragem.Add(XElement.Parse(string.Format("<option value=\"{0}\" {1}>{2}</option>", (char)Model.OrigemResultadoDesempenho.NotaDoEixoX, "selected='selected'", Msg("nota_ciclo_origem_eixo_x_calibragem"))));
							xSelectNotaDeOrigemCalibragem.Add(XElement.Parse(string.Format("<option value=\"{0}\">{1}</option>", (char)Model.OrigemResultadoDesempenho.NotaDoEixoY, Msg("nota_ciclo_origem_eixo_y_calibragem"))));
						}
					}
				}
				else {
					xSelectNotaDeOrigemCalibragem.Add(XElement.Parse(string.Format("<option value=\"{0}\" {1}>{2}</option>", (char)Model.OrigemResultadoDesempenho.NotaDoEixoX, "selected='selected'", Msg("nota_ciclo_origem_eixo_x_calibragem"))));
					xSelectNotaDeOrigemCalibragem.Add(XElement.Parse(string.Format("<option value=\"{0}\">{1}</option>", (char)Model.OrigemResultadoDesempenho.NotaDoEixoY, Msg("nota_ciclo_origem_eixo_y_calibragem"))));
					xDocFormulario.Descendants("select").Where(p => (string)p.Attribute("id") == "SelNotaDeOrigemCalibragem").FirstOrDefault().SetAttributeValue("disabled", "disabled");
				}
			}

			if (LG.HCM.Integrador.Logical.Util.ValidaModulo(Logical.EnumModulo.Metas)) {
				xDocFormulario.Descendants("div").Where(p => (string)p.Attribute("id") == "Metas").FirstOrDefault().SetAttributeValue("style", "display: block;");
				configuracoesMetas = configuracaoApis.FirstOrDefault(p => p.CodigoModulo == Convert.ToInt32(Logical.EnumModulo.Metas));
				XElement xSelectNotaDeOrigemMetas = xDocFormulario.Descendants("select").FirstOrDefault(p => (string)p.Attribute("id") == "SelNotaDeOrigemMetas");

				if (configuracoesMetas != null) {
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "CkMetas").FirstOrDefault().SetAttributeValue("checked", "checked");

					if (xSelectNotaDeOrigemMetas != null) {
						xSelectNotaDeOrigemMetas.Add(XElement.Parse(string.Format("<option value=\"{0}\" {1}>{2}</option>", (char)Model.OrigemResultadoDesempenho.ResultadoCiclo, "selected='selected'", Msg("resultado_do_ciclo"))));
					}
				}
				else {
					xSelectNotaDeOrigemMetas.Add(XElement.Parse(string.Format("<option value=\"{0}\" {1}>{2}</option>", (char)Model.OrigemResultadoDesempenho.ResultadoCiclo, "selected='selected'", Msg("resultado_do_ciclo"))));
					xDocFormulario.Descendants("select").Where(p => (string)p.Attribute("id") == "SelNotaDeOrigemMetas").FirstOrDefault().SetAttributeValue("disabled", "disabled");
				}
			}

			if (configuracoesIntegrador != null) {
				xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "CkConfiguracoesAdicionais").FirstOrDefault().SetAttributeValue("checked", "checked");

				if (configuracoesIntegrador.QuantidadeDiasExpiracao != null) {
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "CkDesconsiderarResultados").FirstOrDefault().Attribute("disabled").Remove();
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "CkDesconsiderarResultados").FirstOrDefault().SetAttributeValue("checked", "checked");
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "Dias").FirstOrDefault().SetAttributeValue("value", configuracoesIntegrador.QuantidadeDiasExpiracao.ToString());
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "Dias").FirstOrDefault().Attribute("disabled").Remove();
				}
				else {
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "CkDesconsiderarResultados").FirstOrDefault().Attribute("disabled").Remove();
				}

				if (configuracoesIntegrador.UtilizarAnoDeReferencia) {
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "CkLimitarResultados").FirstOrDefault().Attribute("disabled").Remove();
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "CkLimitarResultados").FirstOrDefault().SetAttributeValue("checked", "checked");
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "Ano").FirstOrDefault().SetAttributeValue("value", configuracoesIntegrador.AnoDeReferencia.ToString());
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "Ano").FirstOrDefault().Attribute("disabled").Remove();
				}
				else {
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "CkLimitarResultados").FirstOrDefault().Attribute("disabled").Remove();
				}

				if (configuracoesIntegrador.UtilizarRangeEspecifico) {
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "CkConverterResultados").FirstOrDefault().Attribute("disabled").Remove();
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "CkConverterResultados").FirstOrDefault().SetAttributeValue("checked", "checked");
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "Minimo").FirstOrDefault().SetAttributeValue("value", configuracoesIntegrador.ValorMinimoRange.ToString());
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "Maximo").FirstOrDefault().SetAttributeValue("value", configuracoesIntegrador.ValorMaximoRange.ToString());
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "Minimo").FirstOrDefault().Attribute("disabled").Remove();
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "Maximo").FirstOrDefault().Attribute("disabled").Remove();
				}
				else {
					xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "CkConverterResultados").FirstOrDefault().Attribute("disabled").Remove();
				}
			}
			else {
				xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "Minimo").FirstOrDefault().SetAttributeValue("value", "0");
				xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "Maximo").FirstOrDefault().SetAttributeValue("value", "100");
				xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "CkDesconsiderarResultados").FirstOrDefault().SetAttributeValue("disabled", "disabled");
				xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "CkLimitarResultados").FirstOrDefault().SetAttributeValue("disabled", "disabled");
				xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "CkConverterResultados").FirstOrDefault().SetAttributeValue("disabled", "disabled");
				xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "Dias").FirstOrDefault().SetAttributeValue("disabled", "disabled");
				xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "Ano").FirstOrDefault().SetAttributeValue("disabled", "disabled");
				xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "Minimo").FirstOrDefault().SetAttributeValue("disabled", "disabled");
				xDocFormulario.Descendants("input").Where(p => (string)p.Attribute("id") == "Maximo").FirstOrDefault().SetAttributeValue("disabled", "disabled");
			}

			return new { OK = true, Geral = xForm.ToString() };
		}

		public ClRetornoValidacao Validar() {
			List<ClValidador> validador = new List<ClValidador>();
			ClRetornoValidacao validacao = new ClRetornoValidacao();
			ClMensagemErro mensagensErro = new ClMensagemErro {
				PreenchimentoObrigatorio = Msg("campo_preenchimento_obrigatorio")
			};

			bool regraConfigurada = true;
			bool configuracoesAdicionaisObrigatorio = Convert.ToBoolean(Parametros["CkConfiguracoesAdicionais"].ToString());
			bool desconsiderarResultadosObrigatorio = Convert.ToBoolean(Parametros["CkDesconsiderarResultados"].ToString());
			bool limitarResultadosObrigatorio = Convert.ToBoolean(Parametros["CkLimitarResultados"].ToString());
			bool converterResultadosObrigatorio = Convert.ToBoolean(Parametros["CkConverterResultados"].ToString());
			bool configuracoesCompetenciasObrigatorio = Convert.ToBoolean(Parametros["CkCompetencias"].ToString());

			var modelosSelecionados = RecuperarW3Select("SelModelosCompetencias", true);

			Model.ConfiguracaoApi configuracaoCompetencias = new Model.ConfiguracaoApi();
			configuracaoCompetencias.TipoValidacaoCompetencias = Parametros["RegraAcessoResultado"].ToString();
			if (configuracaoCompetencias.TipoValidacaoCompetencias == ((char)Model.TipoValidacaoCompetencias.RegraEspecifica).ToString()) {
				int codigoRegra = Convert.ToInt32(Parametros["SelRegraEspecifica"]);
				regraConfigurada = codigoRegra != 0;
			}

			if (configuracoesAdicionaisObrigatorio) {
				if (desconsiderarResultadosObrigatorio) {
					validador.Add(new ClValidador() {
						Id = "Dias",
						Tipo = EnumTipoCampo.String,
						IndObrigatorio = true
					});
				}

				if (limitarResultadosObrigatorio) {
					validador.Add(new ClValidador() {
						Id = "Ano",
						Tipo = EnumTipoCampo.String,
						IndObrigatorio = true
					});
				}

				if (converterResultadosObrigatorio) {
					validador.Add(new ClValidador() {
						Id = "Minimo",
						Tipo = EnumTipoCampo.String,
						IndObrigatorio = true
					});

					validador.Add(new ClValidador() {
						Id = "Maximo",
						Tipo = EnumTipoCampo.String,
						IndObrigatorio = true
					});
				}
			}

			validacao = ValidarFormulario(validador, Parametros, mensagensErro);

			if (configuracoesCompetenciasObrigatorio) {
				if (modelosSelecionados.Count() == 0) {
					validacao.Erros.Add(new ErroValidacao() { NomColuna = "SelModelosCompetencias", Msg = Msg("campo_preenchimento_obrigatorio") });
				}

				if (!regraConfigurada) {
					validacao.Erros.Add(new ErroValidacao() { NomColuna = "SelRegraEspecifica", Msg = Msg("campo_preenchimento_obrigatorio") });
				}
			}
			return validacao;
		}

		public object CarregarSelect(string idSelect) {
			DataTable tabela = new DataTable();
			string selecionados = "";
			Logical.ModeloConfiguracaoApi modeloConfiguracaoApi = new Logical.ModeloConfiguracaoApi();

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn() { ColumnName = "Codigo", DataType = typeof(int) });
			dt.Columns.Add(new DataColumn() { ColumnName = "Descricao", DataType = typeof(string) });

			try {
				var listaColecaoCompleta = Model.Modulos.ListarModelosCompetencias();
				var modelosConfigurados = modeloConfiguracaoApi.Listar(Convert.ToInt32(Model.ApisComConfiguracao.WorkflowMovimentacaoEDesligamento));
				selecionados = string.Join(",", modelosConfigurados.Select(objeto => objeto.CodigoModelo));

				foreach (var linha in listaColecaoCompleta) {
					int codigo = linha.Codigo;
					string nome = linha.Nome;
					dt.Rows.Add(new object[] { Convert.ToInt32(codigo), nome });
				}

				dt.TableName = "Modelos";
				dt.Columns.Add(new DataColumn() { ColumnName = "checkbox", DataType = typeof(int), DefaultValue = 0 });

				if (dt.Rows.Count > 0)
					InicializarW3Select(dt.AsEnumerable().OrderBy(p => p[2].ToString()).CopyToDataTable(), idSelect);
				else
					InicializarW3Select(dt, idSelect);

				return new { OK = true, vinculosSelecionados = selecionados };
			}
			catch (Exception ex) {
				return new { OK = false, Msg = ex.Message };
			}
		}

		public object SalvarConfiguracao() {
			Validacao validacao = new Validacao();
			Logical.ConfiguracaoApi configuracaoApi = new ConfiguracaoApi();
			Logical.ConfiguracaoApiModelo configuracaoApiModelo = new ConfiguracaoApiModelo();
			ClRetornoValidacao retornoValidacao = this.Validar();
			validacao.Erros = retornoValidacao.Erros;
			validacao.OK = validacao.Erros.Count == 0;

			if (validacao.Erros.Count == 0) {
				try {
					DatabaseUtil.Connector.BeginTransaction();
					bool configuracoesCompetenciasObrigatorio = Convert.ToBoolean(Parametros["CkCompetencias"].ToString());
					bool configuracoesCalibragemObrigatorio = Convert.ToBoolean(Parametros["CkCalibragem"].ToString());
					bool configuracoesMetasObrigatorio = Convert.ToBoolean(Parametros["CkMetas"].ToString());
					bool configuracoesAdicionaisObrigatorio = Convert.ToBoolean(Parametros["CkConfiguracoesAdicionais"].ToString());

					if (configuracoesCompetenciasObrigatorio) {
						Model.ConfiguracaoApi configuracaoCompetencias = new Model.ConfiguracaoApi();
						List<Model.ConfiguracaoApiModelo> modelosCompetencias = new List<Model.ConfiguracaoApiModelo>();

						configuracaoCompetencias.TipoNotaOrigemCompetencias = Parametros["SelNotaDeOrigemCompetencias"].ToString();
						configuracaoCompetencias.TipoValidacaoCompetencias = Parametros["RegraAcessoResultado"].ToString();

						if (configuracaoCompetencias.TipoValidacaoCompetencias == ((char)Model.TipoValidacaoCompetencias.RegraEspecifica).ToString())
							configuracaoCompetencias.CodigoRegraVerAvaliacao = Convert.ToInt32(Parametros["SelRegraEspecifica"]);

						var modelosSelecionados = RecuperarW3Select("SelModelosCompetencias", true);

						configuracaoApiModelo.Excluir(Convert.ToInt32(Model.ApisComConfiguracao.WorkflowMovimentacaoEDesligamento), Convert.ToInt32(Logical.EnumModulo.Competencias));

						for (int i = 0; i <= modelosSelecionados.Count() - 1; i++) {
							int codigoModelo = Convert.ToInt32(modelosSelecionados[i].ItemArray[0]);
							configuracaoApiModelo.Inserir(Convert.ToInt32(Model.ApisComConfiguracao.WorkflowMovimentacaoEDesligamento), Convert.ToInt32(Logical.EnumModulo.Competencias), codigoModelo);
						}

						configuracaoApi.Excluir(Convert.ToInt32(Model.ApisComConfiguracao.WorkflowMovimentacaoEDesligamento), Convert.ToInt32(Logical.EnumModulo.Competencias));
						configuracaoApi.Inserir(Convert.ToInt32(Model.ApisComConfiguracao.WorkflowMovimentacaoEDesligamento), Convert.ToInt32(Logical.EnumModulo.Competencias), configuracaoCompetencias);
					}
					else {
						configuracaoApiModelo.Excluir(Convert.ToInt32(Model.ApisComConfiguracao.WorkflowMovimentacaoEDesligamento), Convert.ToInt32(Logical.EnumModulo.Competencias));
						configuracaoApi.Excluir(Convert.ToInt32(Model.ApisComConfiguracao.WorkflowMovimentacaoEDesligamento), Convert.ToInt32(Logical.EnumModulo.Competencias));
					}

					if (configuracoesCalibragemObrigatorio) {
						Model.ConfiguracaoApi configuracaoCalibragem = new Model.ConfiguracaoApi();
						configuracaoCalibragem.TipoNotaOrigemCalibragem = Parametros["SelNotaDeOrigemCalibragem"].ToString();
						configuracaoApi.Excluir(Convert.ToInt32(Model.ApisComConfiguracao.WorkflowMovimentacaoEDesligamento), Convert.ToInt32(Logical.EnumModulo.Calibragem));
						configuracaoApi.Inserir(Convert.ToInt32(Model.ApisComConfiguracao.WorkflowMovimentacaoEDesligamento), Convert.ToInt32(Logical.EnumModulo.Calibragem), configuracaoCalibragem);
					}
					else {
						configuracaoApi.Excluir(Convert.ToInt32(Model.ApisComConfiguracao.WorkflowMovimentacaoEDesligamento), Convert.ToInt32(Logical.EnumModulo.Calibragem));
					}

					if (configuracoesMetasObrigatorio) {
						Model.ConfiguracaoApi configuracaoMetas = new Model.ConfiguracaoApi();

						configuracaoMetas.TipoNotaOrigemMetas = Parametros["SelNotaDeOrigemMetas"].ToString();
						configuracaoApi.Excluir(Convert.ToInt32(Model.ApisComConfiguracao.WorkflowMovimentacaoEDesligamento), Convert.ToInt32(Logical.EnumModulo.Metas));
						configuracaoApi.Inserir(Convert.ToInt32(Model.ApisComConfiguracao.WorkflowMovimentacaoEDesligamento), Convert.ToInt32(Logical.EnumModulo.Metas), configuracaoMetas);
					}
					else {
						configuracaoApi.Excluir(Convert.ToInt32(Model.ApisComConfiguracao.WorkflowMovimentacaoEDesligamento), Convert.ToInt32(Logical.EnumModulo.Metas));
					}

					if (configuracoesAdicionaisObrigatorio) {
						Model.ConfiguracaoApi configuracoesAdicionais = new Model.ConfiguracaoApi();

						configuracoesAdicionais.ExpirarResultadosPorTempo = Convert.ToBoolean(Parametros["CkDesconsiderarResultados"].ToString());
						configuracoesAdicionais.UtilizarAnoDeReferencia = Convert.ToBoolean(Parametros["CkLimitarResultados"].ToString());
						configuracoesAdicionais.UtilizarRangeEspecifico = Convert.ToBoolean(Parametros["CkConverterResultados"].ToString());

						if (configuracoesAdicionais.ExpirarResultadosPorTempo)
							configuracoesAdicionais.QuantidadeDiasExpiracao = Convert.ToInt32(Parametros["Dias"]);

						if (configuracoesAdicionais.UtilizarAnoDeReferencia)
							configuracoesAdicionais.AnoDeReferencia = Convert.ToInt32(Parametros["Ano"]);

						if (configuracoesAdicionais.UtilizarRangeEspecifico) {
							configuracoesAdicionais.ValorMinimoRange = double.Parse(Parametros["Minimo"].ToString(), CultureInfo.InvariantCulture);
							configuracoesAdicionais.ValorMaximoRange = double.Parse(Parametros["Maximo"].ToString(), CultureInfo.InvariantCulture);
						}

						configuracaoApi.Excluir(Convert.ToInt32(Model.ApisComConfiguracao.WorkflowMovimentacaoEDesligamento), Convert.ToInt32(Logical.EnumModulo.Integrador));
						configuracaoApi.Inserir(Convert.ToInt32(Model.ApisComConfiguracao.WorkflowMovimentacaoEDesligamento), Convert.ToInt32(Logical.EnumModulo.Integrador), configuracoesAdicionais);
					}
					else {
						configuracaoApi.Excluir(Convert.ToInt32(Model.ApisComConfiguracao.WorkflowMovimentacaoEDesligamento), Convert.ToInt32(Logical.EnumModulo.Integrador));
					}

					DatabaseUtil.Connector.CommitTransaction();
				}
				catch (Exception ex) {
					DatabaseUtil.Connector.RollbackTransaction();
					validacao.OK = false;

					validacao.Erros.Add(new ErroValidacao { Msg = ex.Message, NomColuna = "Erro salvar" });

					throw ex;
				}
			}

			return new { OK = validacao.OK, Msg = validacao.Erros.Select(p => new { Nome = p.NomColuna, Msg = p.Msg }).ToList() };
		}
	}
}