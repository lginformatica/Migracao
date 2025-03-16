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
        $(document).ready(function () {
            if (!usuarioLogado) {
            	document.location.href = logoutPage;
                return;
            }
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
                idSubContainer: (idSubCadastro) ? "divSubGrid" : ""
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
