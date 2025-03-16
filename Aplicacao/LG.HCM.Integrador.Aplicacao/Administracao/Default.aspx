<%@ Page Language="C#" AutoEventWireup="true" Inherits="LG.HCM.Integrador.Aplicacao.BasePage" %>

<%@ Import Namespace="W3.Framework.Servico" %>
	
<% = LibUtil.docType %>
<html class="no-ie" xmlns="http://www.w3.org/1999/xhtml">

<head id="Head2" runat="server">
	<title>Configuracoes</title>
	<%
		LibUtil.ValidarPerfil(new Perfil[] { Perfil.AdministradorW3net, Perfil.Administrador });
		var util = new LibUtil(EnumSkin.Azul);

		util.Addw3Lib();
		util.Addw3Popup();
		util.Renderiza();
	%>
	<script type="text/javascript">
		function openWindow() {
            var idSelecao = w3.accentsTidy(Msg("integracoes"));

			var url = '_Principal/ConfiguracoesGerais.aspx?<%= Request.QueryString %>' + '#' + idSelecao;

			if (top.document.location.href.toLowerCase().indexOf("administracao/_principal/") == -1) {
			    var h = screen.height;
			    var w = screen.width;
				w3Popup.abrePopupCentro(url, 'W3_Integrador', w, h);
			}
			else {
				document.location.href = url;
			}
		}

		$(document).ready(function () {
			openWindow();
			$('#link').text(Msg('link'));
			$('#nomeModulo').text(Msg('nomeModulo'));
		});
    </script>
	<style type="text/css">
		#link { font-family: Tahoma; font-size: 8pt; font-weight: normal; }
		#nomeModulo { font-family: Tahoma; font-size: 8pt; color: #666666; }
		body { background-color: #E7E7E7; height: 100%; }
	</style>
</head>
<body style="padding: 0px; margin: 0px;">
	<table cellpadding="0" border="0" class="defaultAdministracao" style="position: absolute; width: 100%; height: 100%">
		<tr>
			<td align="center" valign="middle">
				<span id="nomeModulo"></span>
				<br /><br />
				<a id="link" href="#" onclick="javascript:openWindow(); return false;"></a>
			</td>
		</tr>
	</table>
</body>
</html>
