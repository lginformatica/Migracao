using LG.HCM.Integrador.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LG.HCM.Integrador.Aplicacao.Profile.Simplifica {
    public partial class IntegracaoClave : BasePage {
        protected void Page_Load(object sender, EventArgs e) {
            string idioma = Convert.ToString(HttpContext.Current.Session["Idioma"]);
            labelPainel.Text = Utilitarios.ObterTextoIdiomizado("tituloClave", idioma);
            labelTitulo.Text = Utilitarios.ObterTextoIdiomizado("subtituloClave", idioma);
            labelDescricao.Text = Utilitarios.ObterTextoIdiomizado("mensagemPaginaClave", idioma);
            btnAcessar.Text = Utilitarios.ObterTextoIdiomizado("acessarSolucao", idioma);
        }

        protected void btn_Click(object sender, EventArgs e) {
            string url = HttpContext.Current.Application["UrlSI"].ToString() + "Modulos/LG.HCM.Integrador/Integracoes/Clave/SSO.aspx?tipo-conteudo=Home";
            string openUrl = String.Format("window.open('{0}', '_blank');", url);
            Page.ClientScript.RegisterStartupScript(this.GetType(), "OpenWindow", openUrl, true);
        }
    }
}