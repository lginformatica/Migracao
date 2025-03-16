<%@ WebHandler Language="C#" Class="Util" %>
using System;
using System.Web;
using System.Xml.Linq;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for Configuracoes
/// </summary>
public class Util : IHttpHandler {
	public virtual void ProcessRequest(HttpContext context) {
		JavaScriptSerializer json = new JavaScriptSerializer();
		context.Response.ContentType = "application/json";
		if (context.Request.Url.AbsoluteUri.ToLower().EndsWith("/path")) {
			var ret = new { d = new { Path = context.Server.MapPath("~") } };
			context.Response.Write(json.Serialize(ret));
		}
		if (context.Request.Url.AbsoluteUri.ToLower().EndsWith("/menu")) {
			XElement xMenu = new XElement("menu");
			XElement xEl = XElement.Parse("<item id='mensagem' text='Mensagens' img='W3Icons/visualizar.gif' />");
			xEl.Add(XElement.Parse("<item id='mensagem_framework' text='Mensagens (framework)' img='W3Icons/visualizar.gif' />"));
			xEl.Add(XElement.Parse("<item id='mensagem_si' text='Mensagens (modelo anterior)' img='W3Icons/visualizar.gif' />"));
			xMenu.Add(xEl);
			xMenu.Add(XElement.Parse("<item id='parametros' text='Parametros' img='W3Icons/parametros.png' />"));
			xMenu.Add(XElement.Parse("<item id='listas' text='Listas' img='W3Icons/interfaces.png' />"));
			xMenu.Add(XElement.Parse("<item id='sep1' type='separator' />"));
			xMenu.Add(XElement.Parse("<item id='logAjax' text='Log Ajax' img='W3Icons/log.png' />"));
			xMenu.Add(XElement.Parse("<item id='logDB' text='Log Db' img='W3Icons/log.png' />"));
			xMenu.Add(XElement.Parse("<item id='logColecao' text='Log coleção' img='W3Icons/log.png' />"));
			xMenu.Add(XElement.Parse("<item id='sep2' type='separator' />"));
			xMenu.Add(XElement.Parse("<item id='listaGrids' text='Lista Grids' img='W3Icons/interfaces.png' />"));
			xMenu.Add(XElement.Parse("<item id='sep4' type='separator' />"));
			xEl = XElement.Parse("<item id='sincronismo' text='Sincronimo' img='W3Icons/iconRefresh.png' />");
			xEl.Add(XElement.Parse("<item id='infoSincronismo' text='Status Monitor' img='Fugue/application-monitor.png' />"));
			xEl.Add(XElement.Parse("<item id='logSincronismo' text='Log Sincronimo' img='W3Icons/log.png' />"));
			xMenu.Add(xEl);
			xMenu.Add(XElement.Parse("<item id='sep3' type='separator' />"));
			xEl = XElement.Parse("<item id='paineis' text='Paineis' img='Fugue/animal-monkey.png' />");
			xEl.Add(XElement.Parse("<item id='ncalc' text='NCalc' img='Fugue/abacus.png' />"));
			xMenu.Add(xEl);
			xMenu.Add(XElement.Parse("<item id='geradorClasses' text='Gerador Classes' img='W3Icons/zip.png' />"));

			var ret = new { d = new { OK = true, Menu = xMenu.ToString() } };
			context.Response.Write(json.Serialize(ret));
		}
	}

	public bool IsReusable {
		get {
			return false;
		}
	}
}