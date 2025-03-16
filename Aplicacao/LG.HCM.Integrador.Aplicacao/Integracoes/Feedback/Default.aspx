<%@ Page Language="C#" AutoEventWireup="true" Inherits="LG.HCM.Integrador.Aplicacao.BasePage" %>

<%@ Import Namespace="W3.Framework.Servico" %>

<% = LibUtil.docType %>
<html class="no-ie" xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
	<title>Integração Feedback</title>
	<style>
        button {
            font-weight: bold;
        }

        .iframe-feedback {
            width: 100%;
            height: 100%;
            border: none;
        }
    </style>
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
			w3.ajax("RecuperarUrlFeedback", { url: window.location.href }, function (retorno) {
				    if (retorno.OK) {
                        $('#frameFeedback').attr('src', retorno.urlCDN);
                        setTimeout(preparaEventoFrame(retorno.urlRedirect), 200);
                        w3.loading(false);
				    }
				    else {
				    	w3.alertar(retorno.Titulo, retorno.Msg, function () { window.close(); });
				    	w3.loading(false);
				    }
			    }
			);

            let intervaloMsg = null;

            const ouvinteRespostaIntegracao = function (evt) {
                if (evt.data === 'integracaoOK') {
                    clearInterval(intervaloMsg);
                    $(window).off('message', ouvinteRespostaIntegracao);
                }
            };

            const preparaEventoFrame = function (urlBaseAPI) {
                const elFrame = $('#frameFeedback')[0];

                if (elFrame) {
                    $(window).on('message', ouvinteRespostaIntegracao);

                    intervaloMsg = setInterval(function () {
                        if (elFrame && elFrame.contentWindow) {
                            elFrame.contentWindow.postMessage(
                                JSON.stringify({
                                    urlApiBase: urlBaseAPI,
                                    tema: 'PADRAO',
                                    urlExterna: ''
                                }),
                                '*'
                            );
                        } else {
                            clearInterval(intervaloMsg);
                        }
                    }, 300);
                }
            };

		});
    </script>
</head>
<body>
    <iframe id="frameFeedback" class="iframe-feedback"></iframe>
</body>
</html>
