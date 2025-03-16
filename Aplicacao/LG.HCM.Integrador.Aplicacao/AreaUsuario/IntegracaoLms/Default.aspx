<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LG.HCM.Integrador.Aplicacao.Profile.Simplifica.IntegracaoLms" %>

<!DOCTYPE html>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
      <title>Integração Clave</title>
        <link href="https://unpkg.com/primevue/resources/themes/lara-light-indigo/theme.css" rel="stylesheet"/>
        <link href="https://unpkg.com/primevue/resources/primevue.min.css" rel="stylesheet"/>
        <link href="https://unpkg.com/primeicons/primeicons.css" rel="stylesheet"/>
        <link rel="stylesheet" href="https://unpkg.com/primeflex@3.1.2/primeflex.css"/>

        <script src="https://unpkg.com/vue@next"></script>
        <script src="https://unpkg.com/primevue/core/core.min.js"></script>
        <script src="https://unpkg.com/primevue/card/card.min.js"></script>
        <script src="https://unpkg.com/primevue/button/button.min.js"></script>
         <style>
            body {
              font-family: 'Ubuntu', sans-serif;
              color:#0071BC;
            } 
            .p-button {
              background-color:#0071BC;
               border-color:#0071BC;
            }
            .p-button:enabled:hover {
              background-color:#006ba7;
              border-color:#006ba7;
            }
            .text-blue-700 {
              color:#0071BC;
            }
            .button{
              color: white;
              font-size: 0.875rem;
              padding: 0.65625rem 1.09375rem;
              background: #0071BC;
              border: 1px solid #0071BC;
              margin: 0;
              display: inline-flex;
              cursor: pointer;
              user-select: none;
              align-items: center;
              vertical-align: bottom;
              text-align: center;
              overflow: hidden;
              position: relative;
              border-radius: 6px !important;
            }
        </style>
</head>
<body class="lg-aa-layout">
    <form id="form1" runat="server">
    <div id="app">
    <div class="p-grid">
          <div class="p-col-2 p-md-2 p-lg-2 p-pb-0">
            <div class="p-d-flex p-flex-wrap-reverse">
              <div class="p-mr-2">
                <h3 class="p-mb-1 p-mt-1 font-bold text-3xl ml-2"><asp:Label ID="labelPainel" runat="server"></asp:Label></h3>
              </div>
            </div>
          </div>
          </div>
        <p-card class="ml-1 mr-1">
           <template #content>
		     <div class="lg-card-blank">
                <div class="text-center">
                    <div class="grid grid-nogutter">
                        <div class="col-12 md:col-6">
                            <div class="grid">
                                <div class="col-12 line-height-1 text-left mt-2 fe-line-text-truncate-4 text-blue-700 text-lg font-bold">
                                   <asp:Label ID="labelTitulo" runat="server"></asp:Label>
                                </div>
                                <div class="col-12 line-height-1 text-left mt-2 fe-line-text-truncate-4 text-gray-500 text-sm">
                                     <asp:Label ID="labelDescricao" runat="server"></asp:Label>
                                </div>
                                <div class="col-4 text-left">
                                    <asp:Button runat="server" id="btnAcessar" OnClick="btn_Click" CssClass="button" />
                                </div>
                            </div>
                        </div>
                        <div class="col-12 md:col-6">
                            <img class="fe-center-y" src="treinamento.png" alt="">
                        </div>
                    </div>
                </div>
               </div>
              </template>
             </p-card>
        </div>

        <script>
			const { createApp, ref } = Vue;

			const App = {
				setup() {
					const val = ref(null);

					return {
						val
					};
				},
				components: {
					'p-card': primevue.card,
					'p-button': primevue.button
				},
				methods: {
					openUrl(value) {
						window.open("<% = HttpContext.Current.Application["UrlSI"].ToString() %>Modulos/LG.HCM.Integrador/Integracoes/Eguru/SSO.aspx?tipo-conteudo=Home");
					},
				},
			};
			createApp(App).use(primevue.config.default).mount("#app");
        </script>
    </form>
</body>
</html>
