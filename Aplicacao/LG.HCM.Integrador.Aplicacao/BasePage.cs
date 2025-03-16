using System;
using System.Web;
using W3.Library.PageConfiguration;

namespace LG.HCM.Integrador.Aplicacao
{
	[IntegratedLogin("~/Sair.aspx")]
	public partial class BasePage : W3.Library.Framework.BasePage {
		public BasePage() : base() {
		}
	}
}
