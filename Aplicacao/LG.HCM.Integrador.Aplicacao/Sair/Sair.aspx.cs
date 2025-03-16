using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LG.HCM.Integrador.Aplicacao.Sair {
	public partial class Sair : System.Web.UI.Page {
		protected void Page_Load(object sender, System.EventArgs e) {
			string redirectPage = Request.QueryString["Page"];
			string urlSI = "" + Convert.ToString(Application["UrlSI"]);
			string url = "Sair.aspx";

			if (urlSI != "") {
				url = urlSI + url;
			}
			else {
				url = "../../" + url;
			}

			if (String.IsNullOrEmpty(redirectPage)) {
				url = String.Format("{0}?Pagina={1}", url, redirectPage);
			}

			lkLogin.NavigateUrl = url;
			Session.Abandon();
			Response.Redirect(url);
		}
	}
}