<%@ Page Language="C#" AutoEventWireup="true" Inherits="LG.HCM.Integrador.Aplicacao.BasePage" %>

<%@ Import Namespace="W3.Framework.Servico" %>
<% = LibUtil.docType %>

<html class="no-ie" xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
	<title>Configurações de integração</title>
	<%
		LibUtil.ValidarPerfil(new Perfil[] { Perfil.AdministradorW3net, Perfil.Administrador });
		var util = new LibUtil(EnumSkin.Azul);
		util.AddDhtmlx(new UsoDhtmlx() { Administracao = true, Internas = false, Componentes = new CompDhtml[] { CompDhtml.Completo } });
		util.Addw3Lib();
		util.Addw3Form();
		util.Addw3Select();
		util.AddJson();
		util.AddAutoNumeric();
		util.Renderiza();
	%>
	<script type="text/javascript">
		var layout;

		$(document).ready(function () {
			DadosCorporativos.inicializar();
			DadosCorporativos.carregarFormulario();
			DadosCorporativos.removerBotaoCancelar();
			DadosCorporativos.atachaEventos();

			$('#CkCompetencias').change(function () {
				if ($(this).is(':checked')) {
					$('#SelModelosCompetencias, #SelNotaDeOrigemCompetencias, #RadioJaExistaResultadoConsolidado, #RadioRegraVerAvaliacao, #RadioRegraEspecifica').prop('disabled', false);
				} else {
					$('#SelModelosCompetencias, #SelNotaDeOrigemCompetencias, #RadioJaExistaResultadoConsolidado, #RadioRegraVerAvaliacao, #RadioRegraEspecifica').prop('disabled', true);
				}

				$('#SelRegraEspecifica').prop('disabled', !$('#RadioRegraEspecifica').is(':checked'));
			});

			$('#CkCalibragem').change(function () {
				if ($(this).is(':checked')) {
					$('#SelNotaDeOrigemCalibragem').prop('disabled', false);
				} else {
					$('#SelNotaDeOrigemCalibragem').prop('disabled', true);
				}
			});

			$('#CkMetas').change(function () {
				if ($(this).is(':checked')) {
					$('#SelNotaDeOrigemMetas').prop('disabled', false);
				} else {
					$('#SelNotaDeOrigemMetas').prop('disabled', true);
				}
			});

			$('#CkConfiguracoesAdicionais').change(function () {
				if ($(this).is(':checked')) {
					$('#CkDesconsiderarResultados, #CkLimitarResultados, #CkConverterResultados').prop('disabled', false);
				} else {
					$('#CkDesconsiderarResultados, #CkLimitarResultados, #CkConverterResultados').prop('disabled', true);
				}
			});

			$('#CkConfiguracoesAdicionais').change(function () {
				if ($(this).is(':checked')) {
					if ($('#CkDesconsiderarResultados').is(':checked'))
						$('#Dias').prop('disabled', false);
				} else {
					$('#Dias').prop('disabled', true);
				}
			});

			$('#CkConfiguracoesAdicionais').change(function () {
				if ($(this).is(':checked')) {
					if ($('#CkLimitarResultados').is(':checked'))
						$('#Ano').prop('disabled', false);
				} else {
					$('#Ano').prop('disabled', true);
				}
			});

			$('#CkConfiguracoesAdicionais').change(function () {
				if ($(this).is(':checked')) {
					if ($('#CkConverterResultados').is(':checked'))
						$('#Minimo, #Maximo').prop('disabled', false);
				} else {
					$('#Minimo, #Maximo').prop('disabled', true);
				}
			});

			$('#CkDesconsiderarResultados').change(function () {
				if ($(this).is(':checked')) {
					$('#Dias').prop('disabled', false);
				} else {
					$('#Dias').prop('disabled', true);
				}
			});

			$('#CkLimitarResultados').change(function () {
				if ($(this).is(':checked')) {
					$('#Ano').prop('disabled', false);
				} else {
					$('#Ano').prop('disabled', true);
				}
			});

			$('#RadioRegraEspecifica').change(function () {
				if ($(this).is(':checked')) {
					$('#SelRegraEspecifica').prop('disabled', false);
				} else {
					$('#SelRegraEspecifica').prop('disabled', true);
				}
			});

			$('#RadioJaExistaResultadoConsolidado, #RadioRegraVerAvaliacao').click(function () {
				$('#SelRegraEspecifica').prop('disabled', true);
			});

			$('#CkConverterResultados').change(function () {
				if ($(this).is(':checked')) {
					$('#Minimo, #Maximo').prop('disabled', false);
				} else {
					$('#Minimo, #Maximo').prop('disabled', true);
				}
			});

		});

		w3.setOpcoes({
			fnAjaxSucesso: function () {
				parent.layPrincipal.w3SetTextRodape(w3.getUsuario().Nome + ' (' + w3.getIdioma() + ')');
			}
		});

		var DadosCorporativos = {
			inicializar: function () {
				layout = new dhtmlXLayoutObject(document.body, '1C');
				layout.w3Init({ rodape: false });
				layout.cells("a").hideHeader();
				w3Form.setOpcoes({
					pai: window,
					pop: null
				});
			},
			carregarFormulario: function () {
				var forms = w3.ajax("RetornarFormularios", null);
				if (forms.OK) {
					w3Form.attachFormulario(forms.Geral, layout.cells("a"), DadosCorporativos.salvar, null);
				}
				else {
					w3.alertarErro(Msg("erro"), forms.Msg);
				}
			},
			salvar: function (pop, form, param, window) {
				var numMinimo = parseInt($('#Minimo').val().replace(".", ""));
				var numMaximo = parseInt($('#Maximo').val().replace(".", ""));
				if (numMinimo > numMaximo) {
					w3.alertar(Msg("editar"), Msg("o_minimo_do_range_nao_pode_ser_maior_que_o_maximo"), null);
				}
				else {
					w3.loading(true);
					w3.ajax("SalvarConfiguracao", param,
						function (retorno) {
							w3.loading(false);
							if (retorno.OK) {
								w3Form.clearNote();
								w3.alertar(Msg("editar"), Msg("dados_salvos_com_sucesso"), null);
							}
							else {
								w3Form.clearNote();
								var erros = retorno.Msg;
								if (typeof (erros) == 'string') {
									w3.alertarErro(Msg("erro"), erros);
									return;
								} else {
									for (var i = 0; i < erros.length; i++) {
										var id = erros[i].Nome.split("-");
										w3Form.setNote(id[0], erros[i].Msg);
									}
								}

							}
						}
					);
				}
			},
			atachaEventos: function () {
				var retorno = w3.ajax("CarregarSelect", { idSelect: "SelModelosCompetencias" });
				$("#SelModelosCompetencias").w3MultiSelect({
					idSelect: 'SelModelosCompetencias',
					optionsSelected: retorno.vinculosSelecionados
				});
			},
			removerBotaoCancelar: function () {
				// Remover o botão cancelar. Não está sendo utilizado
				if ($('.divBotoesModal :button')[0].innerText == 'Cancelar'
					|| $('.divBotoesModal :button')[0].innerText == 'Cancel') {
					$('.divBotoesModal :button')[0].remove();
				}
			}
		};
	</script>
	<style>
	  .selectworkflow {
	    max-width: 250px;
	  }

	  .numerico {
	    margin-left: 5px;
	    margin-right: 5px;
	  }
	</style>
</head>

<body>
	<div id="divGrid" style="width: 100%; height: 100%;"></div>
</body>
</html>
