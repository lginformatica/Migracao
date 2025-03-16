<%@ Page Language="C#" AutoEventWireup="true" Inherits="LG.HCM.Integrador.Aplicacao.BasePage" %>

<%@ Import Namespace="W3.Framework.Servico" %>

<% = LibUtil.docType %>
<html class="no-ie" xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
    <title>Configuracoes</title>
    <style type="text/css">
        .csInfoImportacao li {
            margin: 10px;
        }

        .csInfoImportacao li {
            padding: 10px;
        }

        .csUl {
            background-color: white;
            padding: 0px 0px 10px 50px;
        }

            .csUl li {
                color: navy;
                font-size: 11px;
                list-style-type: circle;
            }
    </style>
    <%
        var util = new LibUtil(EnumSkin.Azul);
        util.AddDhtmlx(new UsoDhtmlx() { Administracao = true, Internas = false, Componentes = new CompDhtml[] { CompDhtml.Completo } });
        util.Addw3Lib();
        util.Addw3Grid();
        util.Renderiza();
    %>
    <script type="text/javascript">
        var idCadastro = w3.getQueryString('idCadastro');
        var idGrid;
        var layPrincipal;
        var dhxAcc;
        var guid;

        var toolbar, gridx, popUp;
        $(document).ready(function () {
            layPrincipal = new dhtmlXLayoutObject(document.body, "1C");
            layPrincipal.w3Init({ rodape: false });
            layPrincipal.cells("a").hideHeader();

            dhxAcc = layPrincipal.cells("a").attachAccordion();
            dhxAcc.addItem("a1", "Upload da planilha excel");
            dhxAcc.addItem("a2", "Verificação e confirmação da importação");
            dhxAcc.cells("a1").attachObject("divImport");
            dhxAcc.cells("a1").open();
            $('#formImport').attr('action', w3.getUrlServico() + '/Upload');
        });

        function retornaUpload(nFiles, _guid, msg) {
            guid = _guid;
            if (nFiles <= 0) {
                alert(msg);
                return;
            }
            //*** Processar importacao
            $('#divInfoImportacao').html('');
            w3.ajax('ValidarImportacao', '{"idCadastro":"' + idCadastro + '","idCache":"' + guid + '"}', function (resposta) {
                window.clearInterval(infoHnd);
                montaToolbar();
                if (! resposta.PreValidacao)
                    mostraGrid("grid_" + guid);
                if (!resposta.OK) {
                    toolbar.disableItem('importar');
                    window.setTimeout(function () { alert(resposta.Msg); }, 1000);
                }
                else {
                    toolbar.enableItem('importar');
                    window.setTimeout(function () { alert(resposta.Msg); }, 1000);
                }
                $('#divInfoImportacao').html('');
            });
            var msg = '';
            var infoHnd = window.setInterval(function () {
                var retorno = w3.ajax('InfoImportacao');
                if (msg != retorno.Msg) {
                    $('#divInfoImportacao').append('<li>' + retorno.Msg + '</li>');
                    msg = retorno.Msg;
                }
            }, 1000);
        }

        function montaToolbar() {
            if (typeof (toolbar) != 'object') {
                var xmlTool = w3.ajax('RetornarToolbar');
                toolbar = dhxAcc.cells("a2").attachToolbar();
                toolbar.setIconsPath(iconsPath)
                toolbar.loadXMLString(xmlTool, function () {
                    toolbar.attachEvent("onClick", function (id) {
                        switch (id) {
                            case "retornar":
                                $("#arquivo").replaceWith($("#arquivo").clone());
                                dhxAcc.cells("a1").open();
                                break;
                            case "importar":
                                w3.ajax('Importar', '{"idCadastro":"' + idCadastro + '","idCache":"' + guid + '"}', function (resposta) {
                                    if (!resposta.OK) {
                                        alert(resposta.Msg);
                                    }
                                    else {
                                        document.location.href = 'Cadastros.aspx?idCadastro=' + idCadastro;
                                    }
                                });
                                break;
                            case "cancelar":
                                document.location.href = 'Cadastros.aspx?idCadastro=' + idCadastro;
                                break;
                            case "exportar":
                                gridx.Exportar();
                                break;
                            default:
                                alert("Opção não encontrada: " + id);
                        }
                    });
                });
            }
        }


        function mostraGrid(idGrid) {
            dhxAcc.cells("a2").attachObject("divGrid");
            dhxAcc.cells("a2").open();
            gridx = new W3Grid({
                //urlServico: UrlServico,
                idGrid: idGrid,
                iconsPath: iconsPath,
                idContainerGrid: 'divGrid',
                celulaDhtmlx: layPrincipal.cells("a"),
                onDynXLS: function (grid) {
                    var ixErros = grid.getColIndexById("Erros");
                    var ixLinhaOK = grid.getColIndexById("LinhaOK");
                    var ixTipoUpdate = grid.getColIndexById("TipoUpdate");
                    var ixColunas = grid.getColIndexById("Colunas");
                    grid.setColWidth(ixErros, 1);
                    var tudoOk = true;
                    grid.forEachRow(function (rowId) {
                        grid.cellById(rowId, ixErros).open();
                        var ok = grid.cells(rowId, ixLinhaOK).getValue().toLowerCase();
                        if (ok != 'true') {
                            tudoOk = false;
                            grid.setRowTextStyle(rowId, 'color:#fd482b; border-bottom: 1px solid #cccccc; border-left: none; border-right: none !important; vertical-align: top;');
                            var cols = grid.cells(rowId, ixColunas).getValue().split(',');
                            for (var i = 0; i < cols.length; i++) {
                                var colIx = grid.getColIndexById(cols[i]);
                                grid.setCellTextStyle(rowId, colIx, 'color:red; border-bottom: 1px solid #cccccc; border-left: none; border-right: none !important; vertical-align: top;font-weight:bold; background-color: #FBFB88;');
                            }
                        }
                        if (typeof (ixTipoUpdate) != 'undefined') {
                            switch (grid.cells(rowId, ixTipoUpdate).getValue()) {
                                case "Ignorar":
                                    grid.setRowTextStyle(rowId, 'color:gray;');
                                    break;
                                case "Inserir":
                                    grid.setRowTextStyle(rowId, 'color:navy;');
                                    break;
                                case "Atualizar":
                                    grid.setRowTextStyle(rowId, 'color:navy;');
                                    var cols = grid.cells(rowId, ixColunas).getValue().split(',');
                                    for (var i = 0; i < cols.length; i++) {
                                        var colIx = grid.getColIndexById(cols[i]);
                                        grid.setCellTextStyle(rowId, colIx, 'color:navy; font-weight:bold; background-color: #FBFB88;');
                                    }
                                    break;
                            }
                        }

                    });
                    grid.deleteColumn(ixColunas);
                    //grid.setColumnHidden(ixColunas, true);
                    //if (tudoOk)
                    //    grid.setColumnHidden(ixLinhaOK, true);
                    //grid.setSizes();
                }
            });
        }

    </script>
</head>
<body>
    <div id="divGrid" style="width: 100%; height: 100%;"></div>
    <div id="divImport">
        <form method='post' enctype='multipart/form-data' id='formImport' target='ifImport'>
            <fieldset style='margin: 20px; padding: 10px; border: 1px solid gray'>
                <legend>Passo 1: Seleção da planilha</legend>
                <div style='color: gray; font-style: italic;'>Busque a planilha excel a ser importada e clique no botão Processar</div>
                <div style='padding: 20px 0px 20px 0px'>
                    Planilha excel:
                <input type='file' name='arquivo' id="arquivo" style='height: 22px' /><input type='submit' id='impButton' value='Processar >>' style='height: 22px; margin-left: 10px' />
                </div>
            </fieldset>
        </form>
        <iframe name="ifImport" id="ifImport" style="width: 100%; height: 50px; display: block"></iframe>
        <ul class='csInfoImportacao' id='divInfoImportacao'></ul>
    </div>
</body>
</html>
