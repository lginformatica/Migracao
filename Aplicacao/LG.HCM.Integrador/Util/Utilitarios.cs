using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LG.HCM.Integrador.Util {
	public class Utilitarios {
		public static string ObterTextoIdiomizado(string chave, string siglaIdioma) {
			var localization = new W3.Library.Framework.Localization.LocalizationProvider();
			return localization.GetText(String.Empty, chave, siglaIdioma);
		}
	}
}
