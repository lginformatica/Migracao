<%@ Page Language="C#" AutoEventWireup="true" Inherits="LG.HCM.Integrador.Aplicacao.BasePage" %>

<%@ Import Namespace="W3.Framework.Servico" %>

<% = LibUtil.docType %>
<html class="no-ie" xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
	<title>Integração Eguru</title>

	<%
		var util = new LibUtil();
		util.AddDhtmlxMessage();
		util.AddCssGeral();
		util.AddCssFormularioUsuario();
		util.AddBoostrap();
		util.AddCssNovaHome();
		util.Addw3Lib();
		util.AddFontAwesome();
		util.AddOpenSans();
		util.Addw3Popup();
		util.AddMobileDetect();
		util.AddBoostrapValidator();
		util.AddVariaviesResources();

		util.Renderiza();

	%>
	<script type="text/javascript">
		var PaginaAdmBase = true;
		$(document).ready(function () {
			w3.loading(true);
			w3.ajax("RecuperarUrlSSoEguru", { url: window.location.href }, function (retorno) {
				if (retorno.OK) {
					$('#dados').val(retorno.parametros);
					$("form").attr("action", retorno.urlRedirect);
					$("form").submit();
				}
				else {
					w3.alertar(retorno.Titulo, retorno.Msg, function () { window.close(); });
					w3.loading(false);
				}
			}
			);

		});
	</script>
</head>
<body>
	<form method="post">
		<input type="hidden" name="dados" id="dados" />
	</form>
</body>
</html>
