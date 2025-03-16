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
        var w3Colecao;
        $(document).ready(function () {
            w3Colecao = new W3Colecao({
                idCadastro: "LogSincronismo",
                idContainer: "divGrid"
            });
            w3Colecao.inicializa();
        });
    </script>
</head>

<body>
    <div id="divGrid" style="width: 100%; height: 100%;"></div>
    <div id="divSubGrid" style="width: 100%; height: 100%;"></div>
</body>
</html>
