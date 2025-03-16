using System;
using System.Web;
using System.Web.UI;
using W3.Framework.Servico.Colecao;
using W3.Framework.Servico.Colecao.Classes;
using W3.Framework.Servico.MasterPage;

namespace LG.HCM.Integrador.Aplicacao
{
    public partial class BaseContentPage : BasePage
    {
        protected override void OnPreInit(EventArgs e) {
			MasterPageFile = MasterPageVirtualPathProvider.MasterPageFileLocation;

			base.OnPreInit(e);
		}
	}
}
