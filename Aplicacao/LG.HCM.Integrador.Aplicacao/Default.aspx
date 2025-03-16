<%@ Page Language="C#" AutoEventWireup="true" Inherits="LG.HCM.Integrador.Aplicacao.BasePage" %>

<%@ Import Namespace="W3.Framework.Servico" %>

<% = LibUtil.docType %>
<html class="no-ie" xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
    <title>Configuracoes</title>
    <%
        var util = new LibUtil();
        util.Addw3Lib();
        util.Renderiza();
    %>
    <script type="text/javascript">
        
        $(document).ready(function () {
        	$("#NomeModulo").text(Msg("nomeModulo"));
        });
    </script>
</head>

<body>
	<span id="NomeModulo"></span>
</body>
</html>