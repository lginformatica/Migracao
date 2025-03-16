<%@ Page Language="C#" AutoEventWireup="true" Inherits="LG.HCM.Integrador.Aplicacao.BasePage" %>
<%@ Import Namespace="W3.Framework.Servico" %>
<% = LibUtil.docType %>
<html class="no-ie" xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
	
	<title>Configurar Competências</title>
	<%
        LibUtil.ValidarPerfil(new Perfil[] { Perfil.AdministradorW3net, Perfil.Administrador });
        var util = new LibUtil(EnumSkin.Azul);
        util.AddDhtmlx(new UsoDhtmlx() { Administracao = true, Internas = false, Componentes = new CompDhtml[] { CompDhtml.Completo } });
        util.Addw3Lib();
        util.Addw3Form();
        util.AddJson();        
        util.Renderiza();
	%>
	<script type="text/javascript">
		var layout;

		$(document).ready(function () {
            ConfigurarCompetencias.inicializar();
            ConfigurarCompetencias.carregarFormulario();
            ConfigurarCompetencias.carregarTabelaCiclos();     
            ConfigurarCompetencias.removerBotaoCancelar();            
		});

		w3.setOpcoes({
			fnAjaxSucesso: function () {
				parent.layPrincipal.w3SetTextRodape(w3.getUsuario().Nome + ' (' + w3.getIdioma() + ')');
			}
		});

        var ConfigurarCompetencias = {

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
                    w3Form.attachFormulario(forms.Geral, layout.cells("a"), ConfigurarCompetencias.salvar, null);
				}
				else {
					w3.alertarErro(Msg("erro"), forms.Msg);
				}
			},
            salvar: function (pop, form, param, window) {
                w3.loading(true);
                w3.ajax("SalvarConfiguracaoIntegracao", param,
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
            carregarTabelaCiclos: function () {
                var linhaTabela = [];
                var totalCiclosMarcados = 0;

                w3.loading(true);

                var ciclos = w3.ajax("RetornaListaCiclos", null);                                                             

                $.each(ciclos, function (index, item) {
                    linhaTabela[linhaTabela.length] = "<tr><td><input onclick='ConfigurarCompetencias.controleCheckTodos(this)' type='checkbox' " + (item.IndMarcado == "S" ? "checked='checked'" : "") + " id='cbCiclo_" + item.CodCiclo + "' /></td>";
                    linhaTabela[linhaTabela.length] = "<td style='text-align: left;'>" + item.NomCiclo + "</td>";
                    linhaTabela[linhaTabela.length] = "<td style='text-align: left;'>" + item.DataInicio + "</td>";
                    linhaTabela[linhaTabela.length] = "<td style='text-align: left;'>" + item.DataFim + "</td></tr>";

                    if (item.IndMarcado == "S") {
                        totalCiclosMarcados++;
                    }
                });

                $('#tabelaCiclos tbody').html(linhaTabela.join(''));

                w3.loading(false);

                // Se todos os ciclos estão marcados ao carregar os dados, checkar o cb_MarcarTodos
                if (ciclos.length == totalCiclosMarcados && ciclos.length > 0) {
                    $('#cb_TodosCiclos').prop('checked', true);
                }

                ConfigurarCompetencias.criarEventoMarcarTodosCiclos();                
            },
            criarEventoMarcarTodosCiclos: function () {                
                $("#cb_TodosCiclos").on("click", function () {                                                            
                    $('#tabelaCiclos input:checkbox').map(function () {
                        if ($(this).attr('id') != 'cb_TodosCiclos') {                           
                            $(this).prop('checked', $('#cb_TodosCiclos').is(':checked'));                            
                        }
                    })
                });
            },
            removerBotaoCancelar: function () {
                // Remover o botão cancelar. Não está sendo utilizado
                if ($('.divBotoesModal :button')[0].innerText == 'Cancelar'
                    || $('.divBotoesModal :button')[0].innerText == 'Cancel') {
                    $('.divBotoesModal :button')[0].remove();
                }
            },
            controleCheckTodos: function (ciclo) {                                
                if ($('#cb_TodosCiclos').is(':checked') && $(ciclo).is(':checked') == false) {                    
                    $('#cb_TodosCiclos').prop('checked', false);
                } else {
                    var todosMarcados = true;                    
                    $('#tabelaCiclos input:checkbox').each(function () {
                        if ($(this).attr('id') != 'cb_TodosCiclos') {                            
                            if ($(this).is(':checked') == false) {
                                todosMarcados = false;                                
                            }
                        }
                    });                    

                    if (todosMarcados) {
                        $('#cb_TodosCiclos').prop('checked', true);
                    }
                }                
            }
		};

    </script>
</head>

<body>
	<div id="divGrid" style="width: 100%; height: 100%;"></div>
</body>
</html>
