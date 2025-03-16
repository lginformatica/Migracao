<%@ Page Language="C#" AutoEventWireup="true" %>

<%@ Import Namespace="W3.Framework.Servico" %>

<% = LibUtil.docType %>
<html class="no-ie" xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
    <title>Utilitarios</title>
    <%
		LibUtil.ValidarPerfil(new Perfil[] { Perfil.AdministradorW3net, Perfil.AdministradorGeral, Perfil.AdministradorAuxiliar });
		var util = new LibUtil(EnumSkin.Azul);
        util.Addw3Lib();
        util.Renderiza();
    %>
    <style type="text/css">
        html, body, iframe {
            height: 100%;
            width: 100%;
        }

    </style>
    <script type="text/javascript">
        var PaginaAdmBase = true; //*** para ser encontrado pela paginas internas

        var url = w3.getQueryString('url');

        $(document).ready(function () {
            $('#ifConteudo').attr('src', url);
        });
    </script>
</head>

<body>
    <iframe id="ifConteudo" style="width:99%; overflow: auto"></iframe>
</body>
</html>
