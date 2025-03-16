<%@ Page Language="C#" AutoEventWireup="true" %>

<%@ Import Namespace="W3.Framework.Servico" %>

<% = LibUtil.docType %>
<html class="no-ie" xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
    <title>Configuracoes</title>
    <%
        LibUtil.ValidarPerfil(new Perfil[] { Perfil.AdministradorW3net, Perfil.AdministradorGeral, Perfil.AdministradorAuxiliar });

        var util = new LibUtil(EnumSkin.Azul);
        util.AddDhtmlx(new UsoDhtmlx() { Administracao = true, Internas = false, Componentes = new CompDhtml[] { CompDhtml.Completo } });
        util.Addw3Lib();
        util.Addw3Form();
        util.Addw3Colecao();
        util.AddAutoNumeric();
        util.Addw3PopupFormulario(); //**** importante para ter controle sobre o handler de janelas
        util.Renderiza();
    %>
    <script type="text/javascript">
        var botSalvar = parent.$('.divBotoesModal button[ref="salvar"]');
        botSalvar.find('span').html(Msg('substituir'));
        botSalvar.prop('disabled', true);
        var nItens = 0;

        $(document).ready(function () {
            Configuracao.inicializar();
            Configuracao.carregarHtml()
            //Configuracao.redimPagina();
            //debugger;
            Configuracao.iniciaFormulario();
        });

        var Configuracao = {
            iniciaFormulario: function () {
                //var formulario = $('body').html();
                //w3Form.attachCalendario(formulario, true);
            },
            inicializar: function () {
                w3Form.setOpcoes({
                    pai: window,
                    pop: null
                });
                layout = new dhtmlXLayoutObject(document.body, '1C');
                layout.w3Init({ rodape: false, offset: false });
                layout.cells("a").hideHeader();
                layout.cells("a").attachObject("divFormulario");
            },
            carregarHtml: function () {
                var form = w3.ajax("RetornarFormulario");
                $('#divFormulario').html(form.Html);
                $('input[name="IdGrid"]').val(hndJanela.window.idGrid);
                //$('input[name="ckIdioma"]').click(function () {
                //    idiomas = [];
                //    $('input[name="ckIdioma"]').each(function () {
                //        if ($(this).prop('checked')) {
                //            idiomas[idiomas.length] = $(this).attr('value');
                //        }
                //    });
                //    $('#LocalizarIdioma').val(idiomas.join(','));
                //});
                //$('input[name="ckIdioma"]').trigger('click');
                //$('input[name="ckIdioma"]').trigger('click');
                var $input = $('input[name="Localizar"]');
                $input.focus();
                window.timeOutId = null;
                var palavraChaveLocalizar = '';
                $input.keyup(function () {
                    window.clearTimeout(window.timeOutId);
                    window.timeOutId = window.setTimeout(function () {
                        var opc = $('input[name="Opcoes"]:checked').val();
                        //*** fixando apenas esta opcao (nesta versao)
                        opc = 'MC';
                        if ((opc == 'MC') && ($.trim($input.val()) != palavraChaveLocalizar)) {
                            if ($.trim($input.val()).indexOf(' ') != -1) {
                                $input.val(palavraChaveLocalizar);
                                return;
                            }
                            palavraChaveLocalizar = $.trim($input.val());
                            w3.ajax('RetornaTotais', { idCadastro: 'Mensagem', idGrid: hndJanela.window.idGrid, palavraChave: palavraChaveLocalizar, idioma: $('#ckIdioma').val() }, function (retorno) {
                                if (retorno.OK) {
                                    var msg = Msg("retorno_encontradas");
                                    msg = msg.replace('{nEncontradas}', retorno.TotalEncontrados).replace('{nItens}', retorno.TotalItens);
                                    $('#inputInfo').html(msg);
                                    nItens = retorno.TotalEncontrados;
                                    botSalvar.prop('disabled', (nItens == 0));
                                }
                                else {
                                    //*** grid foi reiniciada
                                    alert('Sessao foi recuperada. A página será recarregada...');
                                    hndJanela.window.document.location.reload();
                                    var win = parent.dhxWins.getTopmostWindow(true);
                                    win.close();
                                    document.write('xxx');
                                }
                            });
                        }
                    }, 500);
                });
            }
        }

        function salvar(param) {
            if (nItens == 0) {
                alert('Não existem mensagens com "' + $('input[name="Localizar"]').val() + '"');
                return;
            }
            var retorno = w3.ajax("SubstituirMensagens", param);
            hndJanela.window.document.location.reload();
            win.close();
        }
    </script>

</head>

<body class="popup iframe-form">
    <div id="divFormulario" style="width: 100%; height: 100%;"></div>
</body>
</html>
