<%@ Page Language="C#" AutoEventWireup="true" Inherits="W3.Framework.Servico.API.Ferramentas.BasePageFerramentas" %>

<%@ Import Namespace="W3.Framework.Servico" %>

<% = LibUtil.docType %>
<html class="no-ie" xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
	<title>Configurações</title>
	<%
		LibUtil.ValidarPerfil(new Perfil[] { Perfil.AdministradorW3net, Perfil.Consultor, Perfil.Administrador });
		var util = new LibUtil(EnumSkin.Azul);
		util.AddDhtmlx(new UsoDhtmlx() { Administracao = true, Internas = false, Componentes = new CompDhtml[] { CompDhtml.Completo } });
		util.Addw3Lib();
		util.Addw3Grid();
		util.Addw3Colecao();
		util.Addw3Popup();
		util.Renderiza();
	%>

	<script type="text/javascript">
		var pai = w3.janelaPai();
		var idCadastro = "W3.Framework.Servico.Colecao.Classes.Parametro";

		$(document).ready(function () {
			var ret = w3.ajax("ColecaoExiste", { "idCadastro": idCadastro });
			if (ret.OK) {
				w3Colecao = new W3Colecao({ "idCadastro": idCadastro, "idContainer": "divGrid" });
				w3Colecao.inicializa();

				w3Colecao.getToolbar(idCadastro).addButton("aplicar", 2, Msg("aplicar"), "W3Icons/concluir.png", "W3Icons/concluir.png");

				//w3Colecao.getToolbar(idCadastro).setUserData("aplicar", "habilitar", "selecaoMultipla");				

				w3Colecao.getToolbar(idCadastro).attachEvent("onClick", function (id) {
					if (id == "aplicar") {
						if (w3Colecao.getGrid(idCadastro) != undefined) {
							var retorno = w3.ajax('SetarParametros');
							if (retorno.OK) {
								w3.alertar(Msg("aplicar_parametros"), Msg("parametros_aplicados_com_sucesso"));
							}
							else {
								var erros = retorno.Msg;
								if (typeof (erros) == 'string') {
									w3.alertarErro(Msg("erro"), erros);
									return;
								}
							}
						}
					}

				}
				);

			}
			else {
				$('#divGrid').html("O cadastro " + idCadastro + " não é uma coleção válida.");
			}
		});

	</script>
</head>
<body>
	<div id="divGrid" style="width: 100%; height: 100%;"></div>
</body>
</html>
