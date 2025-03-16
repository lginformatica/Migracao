<%@ Page Language="C#" AutoEventWireup="true" %>

<%@ Import Namespace="W3.Framework.Servico" %>

<% = LibUtil.docType %>

<html class="no-ie" xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Configuracoes</title>
    <%
        LibUtil.ValidarPerfil(new Perfil[] { Perfil.AdministradorW3net, Perfil.Consultor, Perfil.AdministradorGeral, Perfil.AdministradorAuxiliar });

        var util = new LibUtil(EnumSkin.Azul);
        util.AddDhtmlx(new UsoDhtmlx() { Administracao = true, Internas = false, Componentes = new CompDhtml[] { CompDhtml.Completo } });
        util.Addw3Lib();
        util.Addw3Grid();
        util.AddJsonCookie();
        util.Renderiza();
    %>
    <style type="text/css">
        body {
            margin: 10px;
        }
    </style>
    <script type="text/javascript">
        //if(!dhtmlXForm.prototype.items[b]._getItemNode)dhtmlXForm.prototype.items[b]._getItemNode=function(b){return b}})();
    </script>
    <script type="text/javascript">
        var info, infoCookie;
        $(document).ready(function () {
            info = w3.ajax("RetornaInfo");

            infoCookie = $.cookie('W3SOL.GERACLASSES');
            infoCookie = (infoCookie == null) ? { "namespace": "", "path": "" } : infoCookie;
            var namespace = infoCookie.namespace;
            $('#txNamespace').val(namespace);

            var path = infoCookie.path;
            path = (path == null) ? info.Path + namespace + '/' : path;
            $('#txPath').val(path);
            var ret = w3.ajax("RetornaOwners");
            var op = [];
            op[op.length] = '<option value"">Selecione owner...</option>';
            $.each(ret, function (ix) {
                op[op.length] = '<option value="' + ret[ix].value + '">' + ret[ix].text + '</option>';
            });
            $('#selOwner').html(op.join(''));
            $('#selOwner').change(function () {
                $('#txClasse').val('');
                $('#txConfiguracao').val('');
                var owner = $('#selOwner option:selected').attr('value');
                var ownerName = $('#selOwner option:selected').text();

                $('#aZip').html((owner == '') ? '' : 'Gerar zip completo para o owner ' + ownerName);

                var ret = w3.ajax("RetornaTabelas", { "owner": owner });
                var op = [];
                op[op.length] = '<option value="0">Selecione tabela...</option>';
                $.each(ret, function (ix) {
                    op[op.length] = '<option value="' + ret[ix].value + '">' + ret[ix].text + '</option>';
                });
                $('#selTabela').html(op.join(''));
                $('#selTabela option[value="0"]').prop('selected', true);
            });
            $('#selTabela').change(function () {
                geraClasse();
            });
            $('#ckEstatica').click(function () {
                geraClasse();
            });
            $(":input").blur(function () {
                infoCookie = { "namespace": $('#txNamespace').val(), "path": $('#txPath').val() }
                $.cookie('W3SOL.GERACLASSES', infoCookie);
                geraClasse();
            });
        });

        function geraClasse() {
            $('#txClasse').val('');
            $('#txConfiguracao').val('');
            if (($('#selTabela').val() != null) && ($('#selTabela').val() != "0")) {
                $('#aCopy').html('Criar / atualizar em:');

                var nomTabela = $('#selTabela').val();

                var indEstatica = ($('#ckEstatica').is(':checked')) ? "S" : "N";
                var ret = w3.ajax("GerarClasse", { "nomTabela": nomTabela, "indEstatica": indEstatica, "nomespace": infoCookie.namespace });
                $('#txClasse').val(ret.Classe.join('\n'));
                $('#txConfiguracao').val(ret.Configuracao.join('\n'));

                var pathFile = $('#txPath').val();
                //pathFile = pathFile.substr(0, pathFile.lastIndexOf('/')) + '/' + ret.NomClasse + '.cs';
                //$('#txPath').val(pathFile);
                $('#txPath').show();
            }
        }

        function geraZip() {
            w3.loading(true);
            var indEstatica = ($('#ckEstatica').is(':checked')) ? "S" : "N";
            var owner = $('#selOwner option:selected').attr('value');
            $('#frZip').remove();
            var urlservico = w3.getUrlServico();
            $('body').append('<iframe id="frZip" src="' + urlservico + '/GerarZip?owner=' + owner + '&indEstatica=' + indEstatica + '&nomespace=' + infoCookie.namespace + '"></iframe>');
            //var ret = w3.ajax("GeraZip", { "owner": owner, "indEstatica": indEstatica });
            var timer;
            timer = window.setInterval(function () {
                var retorno = w3.ajax("RetornaZipCompleto");
                if (retorno.Completo) {
                    window.clearInterval(timer);
                    w3.loading(false);
                }
            }, 2000);
        }

        function copie(tx) {
            $('#' + tx).select();
            if (!!navigator.userAgent.match(/Trident\/7\./)) {
                clipboardData.setData("Text", $('#' + tx).val());
                $('#sp' + tx.substr(2)).html('Copiado para clipboard');
            }
            else {
                $('#sp' + tx.substr(2)).html('Selecionado. Use copy & paste.');
            }
        }

        function copiarArquivo() {
            $.coo
            var indEstatica = ($('#ckEstatica').is(':checked')) ? "S" : "N";
            var nomTabela = $('#selTabela').val();
            //var pathFile = $('#txPath').val() + nomtabela.substr(nomtabela.indexOf("}") + 1);
            var ret = w3.ajax("CopiarArquivo", { "nomTabela": nomTabela, "indEstatica": indEstatica, "path": infoCookie.path, "nomespace": infoCookie.namespace });
            if (ret.OK) {
                alert('copiado.');
            }
        }

        function mudaClasse() {
            namespace = $('#txNamespace').val();
            $('#txPath').val(info.Path + '/' + namespace + '/');
        }

    </script>

</head>
<body>
    <input type="checkbox" value="S" id="ckEstatica" checked="checked" />Gerar classe estatica
    <br />
    <br />
    Namespace:
    <input type="text" id="txNamespace" style="width: 150px;" onblur="mudaClasse()" />
    <br />
    <br />
    Owner:
    <select id="selOwner"></select>&nbsp;&nbsp;<a id="aZip" href="javascript:void(0)" onclick="geraZip()"></a>
    <br />
    <br />
    Tabela:
    <select id="selTabela"></select>&nbsp;&nbsp;<a id="aCopy" href="javascript:void(0)" onclick="copiarArquivo()"></a>
    <input type="text" id="txPath" style="width: 450px; margin-left: 6px; display: none;" />
    <br />
    <br />
    <table>
        <tr>
            <td>Classe</td>
            <td>&nbsp;</td>
            <td>Configuracao</td>
        </tr>
        <tr>
            <td>
                <textarea id="txClasse" rows="25" cols="80"></textarea></td>
            <td style="width: 20px">&nbsp;</td>
            <td>
                <textarea id="txConfiguracao" rows="25" cols="80"></textarea></td>
        </tr>
        <tr>
            <td>
                <input type="button" value="selecione" onclick="copie('txClasse')" /><span id="spClasse" /></td>
            <td>&nbsp;</td>
            <td>
                <input type="button" value="selecione" onclick="copie('txConfiguracao')" /><span id="spConfiguracao" /></td>
        </tr>
    </table>
</body>
</html>
