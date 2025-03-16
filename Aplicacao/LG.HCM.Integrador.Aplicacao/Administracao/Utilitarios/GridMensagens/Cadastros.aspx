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
        util.Addw3Grid();
        util.Addw3Colecao();
        util.Renderiza();
    %>
    <script type="text/javascript">
        var idCadastro = w3.getQueryString('idCadastro');
        var idSubCadastro = w3.getQueryString('idSubCadastro');
        var w3Colecao;
        var idGrid;
        $(document).ready(function () {
            var ret = w3.ajax("ColecaoExiste", { "idCadastro": idCadastro });
            if (!ret.OK) {
                $('#divGrid').html("O cadastro " + idCadastro + " não é uma coleção válida.");
                return;
            }
            if (idSubCadastro) {
                var ret = w3.ajax("ColecaoExiste", { "idCadastro": idSubCadastro });
                if (!ret.OK) {
                    $('#divSubGrid').html("O cadastro " + idSubCadastro + " não é uma coleção válida.");
                    return;
                }
            }
            w3Colecao = new W3Colecao({
                idCadastro: idCadastro,
                idContainer: "divGrid",
                idSubCadastro: (idSubCadastro) ? idSubCadastro : "",
                idSubContainer: (idSubCadastro) ? "divSubGrid" : "",
                desabilitaInclusoesImportacao: true
            });
            w3Colecao.inicializa();
            idGrid = w3Colecao.getIdGrid(idCadastro);
            var toolbar = w3Colecao.getToolbar(idCadastro);
            //toolbar.addText('xx', 1, '<input type="text" class="dhx_toolbar_w3buscar" />');
            //$('.dhx_toolbar_w3buscar').each(function () {
            //    var elem = $(this);
            //    var textoPadrao = 'Buscar';
            //    elem.val(textoPadrao);
            //    elem.attr('style', $(this).attr('style') + '; ' + "margin-top: -2px; border: 1px solid #A4BED4; color: gray !important;");
            //    elem.data('oldVal', elem.val());
            //    elem.focus(function (event) {
            //        if (elem.val() == textoPadrao) {
            //            elem.data('oldVal', '');
            //            elem.val("");
            //        }
            //    }).blur(function () {
            //        if (elem.val() == '') {
            //            elem.val(textoPadrao);
            //            elem.data('oldVal', textoPadrao);
            //        }
            //    });
            //    elem.bind("propertychange keyup input paste", function (event) {
            //        delay(function () {
            //            if (elem.data('oldVal') != elem.val()) {
            //                elem.data('oldVal', elem.val());
            //                alert(elem.val())
            //            }
            //        }, 500);
            //    });
            //});
            toolbar.attachEvent("onClick", function (id) {
                switch (id) {
                    case "remover_selecionados":
                        var idGrid = w3Colecao.getIdGrid(idCadastro);
                        var retorno = w3.ajax("RetornaSelecionados", { idGrid: idGrid });
                        if (retorno.TotalRegistros == 0) {
                            w3.alertar(Msg('aviso'), Msg('selecione_registros_para_alteracao'));
                            return;
                        }
                        w3.confirmar(Msg('confirmacao'), Msg('confirme_remover_customizacoes_selecionadas').replace('{nSel}', retorno.TotalRegistros), function (confirmado) {
                            if (confirmado) {
                                w3.ajax("RemoverCustomizacoesSelecionadas", { idGrid: idGrid });
                                w3Colecao.atualizar();
                            }
                        });
                        break;
                    case "remover_todas_customizacoes":
                        w3.confirmar(Msg('confirmacao'), Msg('confirme_remover_customizacoes'), function (confirmado) {
                            if (confirmado) {
                                w3.ajax("RemoverTodasCustomizacoes");
                                w3Colecao.atualizar();
                            }
                        });
                        break;
                }

            });
        });

        function gerarPacote() {
            $('body').append('<iframe style="position:absolute; width:100%; height:100%" id="ifExport"></iframe>');
            $('#ifExport').attr('src', w3.getUrlServico() + '/GerarPacote');
        }

        //var delay = (function () {
        //    var timer = 0;
        //    return function (callback, ms) {
        //        clearTimeout(timer);
        //        timer = setTimeout(callback, ms);
        //    };
        //})();
    </script>
</head>

<body>
    <div id="divGrid" style="width: 100%; height: 100%;"></div>
    <div id="divSubGrid" style="width: 100%; height: 100%;"></div>
</body>
</html>
