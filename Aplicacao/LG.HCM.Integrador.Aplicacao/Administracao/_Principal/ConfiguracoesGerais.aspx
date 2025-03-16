<%@ Page Language="C#" AutoEventWireup="true" Inherits="LG.HCM.Integrador.Aplicacao.BaseContentPage" %>

<asp:Content ID="CnHeader" ContentPlaceHolderID="CnHeader" runat="server">
<%@ Import Namespace="W3.Framework.Servico" %>
<%
		LibUtil.ValidarPerfil(new Perfil[] { Perfil.AdministradorW3net, Perfil.Administrador });
%>
    <script type="text/javascript">
    	var tree;
    	var treeIconsPath = iconsPath;
    	var naoPossuiCiclos = false;
    	var ciclos;

    	fnMasterReady = function () {
    		ConfiguracoesGerais.inicializa();
            ConfiguracoesGerais.redimPagina();            
    	}

    	var ConfiguracoesGerais = {
    	    inicializa: function () {
    	        ConfiguracoesGerais.carregaTreeConfiguracoes();    			
    		},
            carregaTreeConfiguracoes: function () {
    			$("#divContent").empty();
    			if (typeof (tree) == "object") {
    				tree.destructor();
    			}
    			
    			tree = new dhtmlXTreeObject("divContent", "100%", "100%", 0);
    			tree.enableTreeLines("disable");
    			tree.setSkin('dhx_skyblue');
    			tree.setImagePath(imagePath + "administracao/");
    			tree.setIconsPath(treeIconsPath);
    			tree.setDataMode("json");
    			tree.enableHighlighting(true);
    			tree.setXMLAutoLoadingBehaviour("function");
                tree.setXMLAutoLoading(function (id) {                    
    				ConfiguracoesGerais.buscarTree(id);
    			});
    			ConfiguracoesGerais.buscarTree(0);

                tree.attachEvent('onClick', function (id) {                                        
                    tree.refreshItem(id);
                   if (id.indexOf('sep_') == 0) {                                              
    					var next = tree.getAllSubItems(id).split(',');
    					if (next.length > 0) {
    						tree.selectItem(next[0], true);
    					}
    				}
                    else {                       

    					var img = treeIconsPath + tree.getItemImage(id, 0, 0);
    					var nome = tree.getItemText(id);
    					marcaSubItem(nome, img);
    					var url = tree.getUserData(id, 'url');
    					url = (typeof (url) == 'undefined') ? '../Comum/Cadastros.aspx?idCadastro=' + id : url;
    					$('#ifConteudo').attr('src', url);
    				}
                });
                

    			//*** menu de contexto
    			menu = new dhtmlXMenuObject();
    			menu.setIconsPath(iconsPath + "W3Icons/");
    			menu.renderAsContextMenu();
    			tree.enableContextMenu(menu);
    			menu.addNewChild(null, 1, 'atualizar', 'Atualizar', false, 'refresh.png');
    			tree.attachEvent("onBeforeContextMenu", function (id) {
    				tree.selectItem(id, false, false);
    				return true;
    			});
    			menu.attachEvent("onClick", function (id) {
    				switch (id) {
    					case "atualizar":
    						var selId = tree.getSelectedItemId();
    						tree.refreshItem(selId);
    						break;
    				}
    			});
    			tree.selectItem(tree.getItemIdByIndex(0, 0), true);
    		},
            buscarTree: function (id) {                
                var retorno = w3.ajax('BuscarTree', { "pai": id });                
    			if (retorno.OK)
    				tree.loadJSONObject(retorno);
    			else
    				w3.alertarErro(Msg("erro"), retorno.Msg);
    		},
    		redimPagina: function () {
    			var lay = $(layDireita);
    			lay.find("div:eq(1)").css("overflow", "hidden");
    			$(".wrap-content-iframe").height(layDireita.getHeight() - $(".titulo").outerHeight() - 9);
    			$(".wrap-conteudo-admin .content #divContent").height($("body").innerHeight() - $("#divHeaderAdmin").innerHeight() - $(".statusbar").innerHeight() - $(".links-accordion").innerHeight() - $("#divNavContent h2").innerHeight() - 26);
    		},
    		atualizarFrame: function () {
    			if (top.frames["ifConteudo"].contentWindow != undefined) {
    				top.frames["ifConteudo"].contentWindow.location.reload();
    			} else {
    				top.frames["ifConteudo"].document.location.reload();
    			}
    		}
    	}
    	$(window).resize(function () {
    		ConfiguracoesGerais.redimPagina();
    	});
    	function selecionaItem(id) {
    		var img = treeIconsPath + tree.getItemImage(id, 0, 0);
    		var nome = tree.getItemText(id);
    		marcaSubItem(nome, img);
    		var url = tree.getUserData(id, 'url');
    		url = (typeof (url) == 'undefined') ? '../Comum/Cadastros.aspx?idCadastro=' + id + '&CodSubModulo=' + codSubModulo : url;
    		$('#ifConteudo').attr('src', url);
    	}

    </script>
</asp:Content>
<asp:Content ID="CnDireita" ContentPlaceHolderID="CnDireita" runat="server">
    <div id="divConteudo" style="width: 100%; height: 100%;">
        <iframe id="ifConteudo" frameborder="0" style="width: 100%; height: 100%;"></iframe>

    </div>
</asp:Content>
<asp:Content ID="CnEsquerda" ContentPlaceHolderID="CnEsquerda" runat="server">
    <div id="divContent" style="width: 100%; height: 100%; padding-top: 10px;">
    </div>
</asp:Content>

