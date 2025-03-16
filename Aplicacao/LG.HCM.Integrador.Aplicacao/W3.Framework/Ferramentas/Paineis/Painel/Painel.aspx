<%@ Page Language="C#" AutoEventWireup="true" %>

<%@ Import Namespace="W3.Framework.Servico" %>

<% = LibUtil.docType %>
<html class="no-ie" xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
    <title>Configuracoes</title>
    <%
        LibUtil.ValidarPerfil(new Perfil[] { Perfil.AdministradorW3net, Perfil.Consultor, Perfil.AdministradorGeral, Perfil.AdministradorAuxiliar });

        var util = new LibUtil(EnumSkin.Azul);
        util.AddDhtmlx(new UsoDhtmlx() { Administracao = true, Internas = false, Componentes = new CompDhtml[] { CompDhtml.Completo } });
        util.Addw3Lib();
        util.Addw3Form();
        util.Addw3Grid();
        util.AddJson();
        util.AddJsonCookie();
        util.AddJquerySelection();
        util.Renderiza();
    %>
    <style type="text/css">
        html, body {
            width: 100%;
            height: 100%;
            overflow: hidden;
            margin: 0px;
            font: 11px tahoma;
            background-color: #f0f0f0;
        }

        select {
        }

        textarea {
            overflow: auto;
        }

        #divRestaurarUltimas a {
            color: Blue;
            text-decoration: underline;
            cursor: pointer;
        }

            #divRestaurarUltimas a:hover {
                background-color: Yellow;
            }

        .toolbarTable {
            background-color: #EDEDED;
        }

        #divRestaurarUltimas {
            color: #333333;
        }

        #msgStatus {
            color: #333333;
            background-color: #F0F0F0;
        }

        table.dhtmlxLayoutPolyContainer_dhx_skyblue div.dhxcont_statusbar {
            background-color: #F0F0F0;
        }

        #divMensagens {
            color: #333333;
            font: 11px tahoma;
            padding: 20px;
        }

        table.dhtmlxLayoutPolyContainer_dhx_skyblue td.dhtmlxLayoutSinglePoly div.dhxcont_global_content_area {
            border: none !important;
        }

        table.dhtmlxLayoutPolyContainer_dhx_skyblue div.dhxcont_statusbar {
            border-top: none;
        }

        table.dhtmlxLayoutPolyContainer_dhx_skyblue td.dhtmlxLayoutPolySplitterHor {
            border: none;
            font-size: 2px;
            width: 5px;
            cursor: n-resize;
            background-repeat: no-repeat !important;
            background-position: 50% center !important;
            background-color: #6F94B9;
            vertical-align: top;
            -webkit-user-select: none;
            -khtml-user-select: none;
            -moz-user-select: none;
            -o-user-select: none;
            user-select: none;
        }
    </style>
    <script type="text/javascript">
        var queries;

        var dhxLayout;
        var tabbar;
        var statBar;
        var winRestore;
        var currentTime;
        var gridx;
        var mouse = { X: 0, Y: 0 };
        var toolbar;
        var popMemoria;

        $(document).ready(function () {
            queries = $.cookie('W3DIRETORIO_PAINEL');
            if (!queries || typeof (queries.ultimaQuery) == 'undefined') {
                queries = {
                    "ultimas": ['', '', '', '', '', ''],
                    "ultimaQuery": ""
                };
            }
            popMemoria = new dhtmlXPopup();

            //*** Layout
            dhxLayout = new dhtmlXLayoutObject(document.body, "2E");
            w3Form.setOpcoes({
                pai: window,
                pop: null
            });

            Configuracoes.carregaToolBar();
            Configuracoes.carregaBancos();
            Configuracoes.carregaTextarea();
            Configuracoes.carregaTabBar();
        });

        var Configuracoes = {
            carregaBancos: function () {
                {
                    w3.loading(true, "carregando");
                    w3.ajax("ListarBancos", null, function (retorno) {
                        if (retorno.OK) {
                            var h = [];
                            $.each(retorno.Itens, function (ix, item) {
                                h[h.length] = '<option value="' + item.Texto + '">' + item.Valor + '</option>';
                            });
                            $('#selBanco').html(h.join('')).show();
                            $('#selBanco').val(retorno.Banco);
                        }
                        else {
                            alert(retorno.Msg);
                        }
                        w3.loading(false);
                    });
                }
            },
            carregaToolBar: function () {
                toolbar = dhxLayout.cells("a").attachToolbar();
                toolbar.setIconsPath(iconsPath)
                toolbar.addSeparator("sep1", 0);
                toolbar.addText("combo", 1, "");
                var comboDIV = toolbar.objPull[toolbar.idPrefix + "combo"].obj;
                comboDIV.style.margin = '4px';
                var combocont = document.getElementById("lbBanco");
                toolbar.objPull[toolbar.idPrefix + "combo"].obj.appendChild(combocont);
                toolbar.addButton("executar", 2, "Executar", "W3Icons/exclamation.png", "W3Icons/exclamation.png");
                toolbar.addSeparator("sep1", 3);
                toolbar.addButton("export", 4, "Exportar Resultados", "W3Icons/excel.png", "W3Icons/excel.png");
                toolbar.disableItem("export");

                toolbar.attachEvent("onClick", function (id) {
                    switch (id) {
                        case "executar":
                            Configuracoes.executaQuery();
                            break;
                        case "export":
                            Configuracoes.exportExcel();
                            break;
                    }
                });
            },
            carregaTabBar: function () {
                dhxLayout.cells("b").hideHeader();
                dhxLayout.cells("b").setHeight('50%');

                var xmlAbas = "<?xml version='1.0' encoding='utf-8' ?><tabbar><row><tab id='mensagens' width='120px'>Mensagens</tab></row></tabbar>";
                tabbar = w3Form.attachTabbar(xmlAbas, true, null);
                dhxLayout.cells("b").attachObject("_divTabbar");

                tabbar.cells("mensagens").attachObject("divMensagens");
                tabbar.tabs("mensagens").setActive();

                tabbar.attachEvent("onSelect", function (idAba, last_id) {
                    if (idAba != "mensagens") {
                        Configuracoes.montaGrid(idAba);
                    }
                    return true;
                });
            },
            carregaTextarea: function () {
                $('body').append('<div id="divQuery" style="width:99%; height:100%; padding: 8px;"><textarea id="txQuery"></textarea></div>')
                //$('#txQuery').val('select top 10 name, object_id from sys.tables;');
                //$('#txQuery').val('use db_camargo_correa_si; select cod_usuario, nom_usuario from w3si.usuario;');
                dhxLayout.cells("a").attachObject("divQuery");
                dhxLayout.cells("a").hideHeader();
                dhxLayout.cells("a").setHeight('250');

                var sb = dhxLayout.cells("a").attachStatusBar();
                //sb.setText('<div id="divSalvar">Salvar Memória: <a>M1</a>&nbsp;<a>M2</a>&nbsp;<a>M3</a>&nbsp;<a>M4</a>&nbsp;<a>M5</a>&nbsp;</div>');
                sb.setText('<div id="divMemoria" style="position:absolute"><span id="divRestaurarUltimas">Queries Anteriores: <a id="m1">Q1</a>&nbsp;<a>Q2</a>&nbsp;<a>Q3</a>&nbsp;<a>Q4</a>&nbsp;<a>Q5</a>&nbsp;</span></div><div id="msgStatus" style="padding-left:400px"></div>');

                $('#txQuery').height($('#txQuery').parent().height() - 22).width('98%').focus();
                //$('#txQuery').focus();

                $('#txQuery').mousedown(function () {
                    switch (event.which) {
                        case 2: //Middle mouse button pressed
                            executaQuery();
                            break;
                    }
                });
                $(document).mousemove(function (e) {
                    mouse = { X: e.pageX, Y: e.pageY };
                });
                $('a').click(function () {
                    var s = $(this).closest('span');
                    var p = $(this).html().substr(1, 1);
                    switch (s.attr('id')) {
                        case "divRestaurarUltimas":
                            $('#txQuery').val(queries.ultimas[p]);
                            break;
                    }
                });
                $('a').hover(
                    function () {
                        var s = $(this).closest('span');
                        var p = $(this).html().substr(1, 1);
                        popMemoria.clear();
                        popMemoria.attachHTML('<div style="width:200px; white-space: pre-line; padding:10px;">' + queries.ultimas[p] + '</div>');
                        pos = $(this).offset();
                        popMemoria.show(pos.left, pos.top, 20, 20);
                    },
                    function () {
                        popMemoria.hide();
                    }
                   );
            },
            executaQuery: function (banco, query) {
                if (typeof (banco) == 'undefined') {
                    var selection = $('#txQuery').caretSelection();
                    query = (selection.text != '') ? selection.text : $('#txQuery').val();
                    query = query.replace(/'/g, "[[");
                    banco = $('#selBanco').val();
                }

                tabbar.forEachTab(function (tab) {
                    if (tab.getId() != 'mensagens')
                        tabbar.tabs(tab.getId()).close();
                });                

                $('#msgStatus').html('');
                //$('#divCarregando').remove();
                //$('body').append('<div id="divCarregando" style="z-index: 9999; position: absolute; width: 100%; height: 100%; top: 0; left: 0; background-color: transparent;"><div style="background-color: #FFFFFF; border: solid 1px #666666; position: absolute; top: 20%; left: 50%; width: 300px; margin-top: -45px; margin-left: -220px; z-index: 30000; text-align: center; padding: 25px 0; color: #666666;"><img src="../Lib/dhtmlx/w3_skin_azul/imgs/spinner.gif" style="vertical-align: middle; margin-right: 5px;" /><span>Executando Consulta...</span></div></div>');
                //$('#divCarregando div').show();
                var start = +new Date();
                w3.ajax("ExecutarQuery", { Banco: banco, Query: encodeURI(query) }, function (resposta) {
                    for (var i = 0; i < resposta.NumResultados; i++) {
                        var ref = i + 1;
                        tabbar.addTab("resultado_" + ref, "Resultado " + ((resposta.NumResultados == 1) ? '' : ref), "120px");
                    }
                    if (resposta.NumResultados > 0) {
                    	tabbar.tabs("resultado_1").setActive();
                        toolbar.enableItem("export");
                    }
                    else {
                        toolbar.disableItem("export");
                        tabbar.tabs("mensagens").setActive();
                        //$('#divCarregando').hide();
                    }
                    
                    //*** setar banco corrente
                    $('#selBanco').val(resposta.BancoCorrente.toLowerCase());

                    $('#divMensagens').html(resposta.Mensagem);
                    $('#txQuery').focus();

                    //*** salvar query
                    if (queries.ultimaQuery != query) {
                        for (var i = 5; i > 1; i--) {
                            queries.ultimas[i] = queries.ultimas[i - 1];
                        }
                        queries.ultimas[1] = query;
                        queries.ultimaQuery = query;
                        $.cookie('W3DIRETORIO_PAINEL', queries, { expires: 1000 * 60 * 24 * 7 });
                    }
                    $('#msgStatus').html('|' + resposta.BancoCorrente + '| Elapsed: ' + w3.getElapsed(start) + '| Runtime: ' + resposta.ElapsedTime + '|' + resposta.NumResultados + ' linha(s)');
                    //$('#divCarregando').hide();
                });
            },
            montaGrid: function (idAba) {
                w3.loading(true, "Formatando tabela");
                w3.ajax("PrepararDadosGrid", { IdGrid: idAba }, function (resposta) {
                    if (resposta.OK) {
                        ref = idAba.split('_')[1];
                        $('#divGrid').remove();
                        //tabbar.setContentHTML("resultado_" + ref, '<div id="divGrid" style="height:100%; width:100%"></div>');
                        tabbar.tabs("resultado_" + ref).attachHTMLString('<div id="divGrid" style="height:100%; width:100%"></div>');
                        //gridx = new w3libGrid(UrlServico, 'divGrid', idAba);
                        gridx = new W3Grid({ idGrid: idAba, iconsPath: iconsPath, idContainerGrid: 'divGrid', celulaDhtmlx: dhxLayout.cells("b") });
                    }
                    else {
                        toolbar.disableItem("export");
                        tabbar.removeTab(idAba, false);
                        tabbar.tabs("mensagens").setActive();
                        $('#divMensagens').html(resposta.Mensagem);
                        $('#txQuery').focus();
                    }

                    w3.loading(false);                    
                    //$('#msgStatus').html('|' + resposta.BancoCorrente + '| Elapsed: ' + getElapsed(start) + '| Runtime: ' + resposta.ElapsedTime + '|' + resposta.NumResultados + ' linha(s)');
                });
            },
            exportExcel: function () {
                $('#iframex').attr('src', UrlServico + '/GerarExcelCompleto');
            }
        }
    </script>
</head>
<body>
    <label class="cbToolbar" id="lbBanco">
        <b>Banco:</b>
        <select id="selBanco">
        </select>
    </label>
    <div id='divShowMemory' style='position: absolute; background-color: Yellow; border: 2px solid navy; display: none; padding: 5px; z-index: 1000'>
    </div>
    <div id="divMensagens">
    </div>
    <iframe id="iframex" src="" style="visibility: hidden;"></iframe>
</body>
</html>
