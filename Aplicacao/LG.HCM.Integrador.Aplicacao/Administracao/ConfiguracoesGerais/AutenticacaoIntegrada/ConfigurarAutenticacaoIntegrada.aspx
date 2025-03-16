<%@ Page Language="C#" AutoEventWireup="true" Inherits="LG.HCM.Integrador.Aplicacao.BasePage" %>

<%@ Import Namespace="W3.Framework.Servico" %>
<% = LibUtil.docType %>

<html class="no-ie" xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
	<title>Configurar AutenticacaoIntegrada</title>
	<%
			LibUtil.ValidarPerfil(new Perfil[] { Perfil.AdministradorW3net, Perfil.Administrador });
			var util = new LibUtil(EnumSkin.Azul);
			util.AddDhtmlx(new UsoDhtmlx() { Administracao = true, Internas = false, Componentes = new CompDhtml[] { CompDhtml.Completo } });
			util.Addw3Lib();
			util.Addw3Form();
			util.AddJson();
			util.AddAutoNumeric();
			util.AddFontAwesome();
			util.Renderiza();
	%>
	<script type="text/javascript">
		var layout;

		$(document).ready(function () {
			ConfigurarAutenticacaoIntegrada.inicializar();
			ConfigurarAutenticacaoIntegrada.carregarFormulario();
			ConfigurarAutenticacaoIntegrada.removerBotaoCancelar();

            var inputs = $('#ChavePublica, #ChavePrivada');
            var valoresPadrao = {
                '#ChavePublica': $('#ChavePublica').val(),
                '#ChavePrivada': $('#ChavePrivada').val()
            };

            inputs.on('click', function () {
                var input = $(this);
                var vlrPadrao = valoresPadrao['#' + input.attr('id')];
                if (input.val() === vlrPadrao) {
                    input.val('');
                }
            });

            $(document).on('click', function (event) {
                inputs.each(function () {
                    var input = $(this);
                    var vlrPadrao = valoresPadrao['#' + input.attr('id')];
                    if (!$(event.target).closest(input).length) {
                        if (input.val() === '') {
                            input.val(vlrPadrao);
                        }
                    }
                });
            });
		});

		w3.setOpcoes({
			fnAjaxSucesso: function () {
				parent.layPrincipal.w3SetTextRodape(w3.getUsuario().Nome + ' (' + w3.getIdioma() + ')');
			}
		});

		var ConfigurarAutenticacaoIntegrada = {
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
					w3Form.attachFormulario(forms.Geral, layout.cells("a"), ConfigurarAutenticacaoIntegrada.salvar, null);
				}
				else {
					w3.alertarErro(Msg("erro"), forms.Msg);
				}
			},
			salvar: function (pop, form, param, window) {
				w3.loading(true);
				w3.ajax("SalvarConfigurarAutenticacaoIntegrada", param,
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
</head>

<body>
	<div id="divGrid" style="width: 100%; height: 100%;"></div>
</body>
</html>
