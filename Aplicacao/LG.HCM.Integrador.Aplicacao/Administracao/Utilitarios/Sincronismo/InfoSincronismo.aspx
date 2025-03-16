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
        util.Renderiza();
    %>
    <style type="text/css">
        ul {
            padding: 20px;
        }
        li {
            font-size: 11px;
            list-style-type: circle;
            padding:2px;
        }

    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            var retorno = w3.ajax("RetornarInfo");
            var h = [];
            h[h.length] = "<ul>";
            $.each(retorno.Lista, function (ix, item) {
                h[h.length] = "<li>" + item.replace(/{T}/g, '&nbsp;&nbsp;') + "</li>";
            });
            h[h.length] = "</ul>";
            $('#divInfo').html(h.join(''));
        });
    </script>
</head>

<body>
    <div id="divInfo" style="width: 100%; height: 100%; padding: 10px"></div>
</body>
</html>
