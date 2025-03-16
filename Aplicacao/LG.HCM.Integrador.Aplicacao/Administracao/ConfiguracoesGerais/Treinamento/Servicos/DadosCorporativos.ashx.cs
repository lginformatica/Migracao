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
	public class DadosCorporativos : ServicoBaseCadastro {

		public const string ID_PARAMETRO_CONFIGURACAO_ENVIO_DADOS_CORPORATIVOS = Classes.Parametros.ID_CONFIGURACAO_ENVIO_DADOS_CORPORATIVOS_TREINAMENTO;
		public const string ID_PARAMETRO_CONFIGURACAO_URL_BASE_API = Classes.Parametros.ID_CONFIGURACAO_URL_BASE_API_TREINAMENTO;
		public const string ID_PARAMETRO_CONFIGURACAO_TOKEN_CLIENT_ID = Classes.Parametros.ID_CONFIGURACAO_TOKEN_CLIENT_ID_TREINAMENTO;
		public const string ID_PARAMETRO_CONFIGURACAO_TOKEN_CLIENT_SECRET = Classes.Parametros.ID_CONFIGURACAO_TOKEN_CLIENT_SECRET_TREINAMENTO;

		public const string ID_CAMPO_CONFIGURACAO_ENVIO_DADOS_CORPORATIVOS = "IntegracaoTreinamentoEnvioDadosCorporativos";
		public const string ID_CAMPO_CONFIGURACAO_URL_BASE_API = "IntegracaoTreinamentoUrlBaseApi";
		public const string ID_CAMPO_CONFIGURACAO_TOKEN_CLIENT_ID = "IntegracaoTreinamentoTokenClientId";
		public const string ID_CAMPO_CONFIGURACAO_TOKEN_CLIENT_SECRET = "IntegracaoTreinamentoTokenClientSecret";


		public object RetornarFormularios() {
			var formulario = RetornarFormularioHtml();

			XDocument xDocFormulario = XDocument.Parse(formulario);
			XElement xForm = xDocFormulario.Descendants("div").Where(p => (string)p.Attribute("id") == "formGeral").FirstOrDefault();

			Dictionary<string, object> conteudo = new Dictionary<string, object>();
			var configuracaoDadosCorporativos = Classes.Parametros.Buscar(ID_PARAMETRO_CONFIGURACAO_ENVIO_DADOS_CORPORATIVOS);
			var configuracaoUrlBaseApi = Classes.Parametros.Buscar(ID_PARAMETRO_CONFIGURACAO_URL_BASE_API);
			var configuracaoTokenClientId = Classes.Parametros.Buscar(ID_PARAMETRO_CONFIGURACAO_TOKEN_CLIENT_ID);
			var configuracaoTokenClientSecret = Classes.Parametros.Buscar(ID_PARAMETRO_CONFIGURACAO_TOKEN_CLIENT_SECRET);

			if (configuracaoDadosCorporativos != null) {
				conteudo.Add(ID_CAMPO_CONFIGURACAO_ENVIO_DADOS_CORPORATIVOS, new { value = configuracaoDadosCorporativos.VlrParametro });
			}

			if (configuracaoUrlBaseApi != null) {
				conteudo.Add(ID_CAMPO_CONFIGURACAO_URL_BASE_API, new { value = configuracaoUrlBaseApi.VlrParametro });
			}

			if (configuracaoTokenClientId != null) {
				conteudo.Add(ID_CAMPO_CONFIGURACAO_TOKEN_CLIENT_ID, new { value = configuracaoTokenClientId.VlrParametro });
			}

			if (configuracaoTokenClientSecret != null) {
				conteudo.Add(ID_CAMPO_CONFIGURACAO_TOKEN_CLIENT_SECRET, new { value = "ValorParametro" });
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
				Id = ID_CAMPO_CONFIGURACAO_URL_BASE_API,
				Tipo = EnumTipoCampo.String,
				IndObrigatorio = true
			});

			validador.Add(new ClValidador() {
				Id = ID_CAMPO_CONFIGURACAO_TOKEN_CLIENT_ID,
				Tipo = EnumTipoCampo.String,
				IndObrigatorio = true
			});

			validador.Add(new ClValidador() {
				Id = ID_CAMPO_CONFIGURACAO_TOKEN_CLIENT_SECRET,
				Tipo = EnumTipoCampo.String,
				IndObrigatorio = true
			});

			ClRetornoValidacao validacao = ValidarFormulario(validador, Parametros, mensagensErro);

			return validacao;
		}

		public object SalvarDadosCorporativos() {
			var vlrConfiguracaoDadosCorporativos = Convert.ToString(Parametros[ID_CAMPO_CONFIGURACAO_ENVIO_DADOS_CORPORATIVOS.Replace(".", "")]);

			Validacao validacao = new Validacao();
			ClRetornoValidacao retornoValidacao = this.Validar();
			validacao.Erros = retornoValidacao.Erros;
			validacao.OK = (validacao.Erros.Count == 0);

			if (validacao.Erros.Count == 0) {
				try {
					DatabaseUtil.Connector.BeginTransaction();

					var vlrConfiguracaoUrlBaseApi = Convert.ToString(Parametros[ID_CAMPO_CONFIGURACAO_URL_BASE_API]);
					var vlrConfiguracaoTokenClientId = Convert.ToString(Parametros[ID_CAMPO_CONFIGURACAO_TOKEN_CLIENT_ID]);
					var vlrConfiguracaoTokenClientSecret = Convert.ToString(Parametros[ID_CAMPO_CONFIGURACAO_TOKEN_CLIENT_SECRET]);
					Classes.Parametros.Atualizar(ID_PARAMETRO_CONFIGURACAO_ENVIO_DADOS_CORPORATIVOS, vlrConfiguracaoDadosCorporativos);
					Classes.Parametros.Atualizar(ID_PARAMETRO_CONFIGURACAO_URL_BASE_API, vlrConfiguracaoUrlBaseApi);
					Classes.Parametros.Atualizar(ID_PARAMETRO_CONFIGURACAO_TOKEN_CLIENT_ID, vlrConfiguracaoTokenClientId);

					if (vlrConfiguracaoTokenClientSecret != "ValorParametro") {
						Classes.Parametros.Atualizar(
								ID_PARAMETRO_CONFIGURACAO_TOKEN_CLIENT_SECRET,
								W3.Library.Encryption.DataEncryption.EncryptText(vlrConfiguracaoTokenClientSecret)
							);
					}

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
			}

			return new { OK = validacao.OK, Msg = validacao.Erros.Select(p => new { Nome = p.NomColuna, Msg = p.Msg }).ToList() };
		}
	}

}