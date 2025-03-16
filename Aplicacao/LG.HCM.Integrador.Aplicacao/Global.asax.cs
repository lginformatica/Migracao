using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Security;
using System.Web.SessionState;
using W3.Framework.Servico.MasterPage;

namespace LG.HCM.Integrador.Aplicacao {
	public class Global : System.Web.HttpApplication {

		protected void Application_Start(object sender, EventArgs e) {
			try {
				MasterPageVirtualPathProvider vpp = new MasterPageVirtualPathProvider();
				HostingEnvironment.RegisterVirtualPathProvider(vpp);
				Application.Lock();
				Application["CodModulo"] = 30;
				Application["SglModulo"] = "IN";
				Application["AppsCarregadas"] = false;
				Application["LogicalAssemblyQualifiedName"] = "LG.HCM.Integrador.Aplicacao";
				Application["DatAlteracaoParametro"] = DateTime.Now;
				CarregarColecoes();
			}
			finally {
				Application.UnLock();
			}
		}

		private void CarregarColecoes() {
			W3.Framework.Servico.W3ContainerBuilder.RegisterCadastrosFromAssemblyList(typeof(LG.HCM.Integrador.Classes.ConfiguracaoIntegracao).Assembly);
		}

		protected void Session_Start(object sender, EventArgs e) {
			if (Application["SessionTimeout"] != null) {
				Session.Timeout = Convert.ToInt32(Application["SessionTimeout"]);
			}

			string licenseFile = Server.MapPath("~/Aspose.Total.lic");

			if (!System.IO.File.Exists(licenseFile))
				return;

			new Aspose.Cells.License().SetLicense(licenseFile);
		}

		protected void Application_BeginRequest(object sender, EventArgs e) {
			if (Application["AppsCarregadas"] != null && (bool)Application["AppsCarregadas"] == false) {
				lock (Application) {
					if (Application["AppsCarregadas"] != null && (bool)Application["AppsCarregadas"] == false) {
						W3.Library.DataSql.GlobalSettingsModule.CarregarParametros(30, "IN", 1);
						Application["AppsCarregadas"] = true;
						PlugIn.Context.Current.OnApplicationStart(EventArgs.Empty);
					}
				}
			}

			if (ValidateRequest()) {
				Response.Headers.Remove("Access-Control-Allow-Origin");

				Response.AddHeader("Access-Control-Allow-Origin", Request.UrlReferrer.GetLeftPart(UriPartial.Authority));
				Response.AddHeader("Checksum", "H-" + DateTime.Now.ToString());
			}
			else {
				Response.AddHeader("Checksum", "NH-" + DateTime.Now.ToString());
			}
			PlugIn.Context.Current.OnBeginRequest(EventArgs.Empty);
		}

		protected bool ValidateRequest() {
			string host = (Request?.UrlReferrer?.Host ?? "");
			if (!string.IsNullOrEmpty(host)) {
				foreach (string chave in ConfigurationManager.AppSettings.AllKeys.Where(prop => prop.StartsWith("UrlPermitida"))) {
					string urlPermitida = ConfigurationManager.AppSettings[chave];
					if (host.IndexOf(urlPermitida) >= 0) {
						return true;
					}
				}
			}

			return false;
		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e) {

		}

		protected void Application_Error(object sender, EventArgs e) {

		}

		protected void Session_End(object sender, EventArgs e) {

		}

		protected void Application_End(object sender, EventArgs e) {

		}
	}
}