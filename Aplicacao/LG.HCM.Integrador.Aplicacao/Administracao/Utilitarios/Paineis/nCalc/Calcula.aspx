<%@  Language="C#" AutoEventWireup="true" Inherits="W3.Metas.BasePage" %>

<%@ Import Namespace="W3.Framework.Servico" %>

<% = LibUtil.docType %>
<html class="no-ie" xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
    <title>Configuracoes</title>
    <%
        LibUtil.ValidarPerfil(new Perfil[] { Perfil.AdministradorW3net, Perfil.Administrador });
        var util = new LibUtil(EnumSkin.Azul);
        util.AddDhtmlx(new UsoDhtmlx() { Administracao = true, Internas = false, Componentes = new CompDhtml[] { CompDhtml.Completo } });
        util.Addw3Lib();
        util.Renderiza();
    %>
    <script type="text/javascript">
        $(document).ready(function () {
        });

        function Processar() {
            var retorno = w3.ajax("ProcessarFormula", { "Formula": encodeURIComponent($('#txFormula').val()) });
            if (retorno.OK) {
                $('#divResultado').html(retorno.Resultado);
            }
            else {
                alert(retorno.Msg);
            }
        }


    </script>
</head>
<body class="popup iframe-form-grid" style="margin:10px">
    <textarea id="txFormula" rows="8" style="width:90%"></textarea>
    <br /><input type="button" value="Processar Formula >>>" onclick="Processar()" />
    <div style="width: 90%; height: 100px; padding: 10px; border:2px solid gray; background-color: lightgray" id="divResultado"></div>
</body>
</html>
