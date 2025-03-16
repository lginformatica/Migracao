using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using W3.Framework.Servico;
using W3.Framework.Servico.Colecao;

namespace LG.HCM.Integrador.Aplicacao.Administracao.Principal.Servicos
{
    /// <summary>
    /// Summary description for Adm
    /// </summary>
    public class PrincipalMaster : ServicoBaseAshx
    {
        public object Informacoes() {
            return new { Idioma = Idioma, Idiomas = ClIdiomas.Idiomas };
        }

        public object ListarLinksAccordion()
        {
            List<object> Item = new List<object>();
			Item.Add(new { Nome = Msg("integracoes"), Url = "ConfiguracoesGerais.aspx", Ico = "W3Icons/icoTreeBarra1.gif" });

            var retorno = new { Itens = Item };
            return retorno;
        }

		/// <summary>
		/// Busca os ciclos cadastrados no sistema
		/// </summary>
		/// <returns>Retorna lista de ciclos</returns>
		public object BuscarCiclos() {
			try {
				//int codCiclo = (Context.Request.Cookies["CodCicloAdmModuloPadrao"] != null) ? Convert.ToInt32(Context.Request.Cookies["CodCicloAdmModuloPadrao"].Value) : 0;
				//var cicloAtual = Ciclo.CicloAtual;

				List<object> ciclos = new List<object>();
				//var listaCiclos = Colecao<Ciclo>.Instance.Itens;
				//foreach (var ciclo in listaCiclos) {
				//	var selected = (codCiclo != 0 ? (codCiclo == ciclo.CodCiclo) : (cicloAtual != null && cicloAtual.CodCiclo == ciclo.CodCiclo ? true : false));
				//	ciclos.Add(new { CodCiclo = ciclo.CodCiclo.ToString(), NomCiclo = ciclo.NomCiclo.ToIdiomaCorrente(), Selected = selected });
				//}

				return new { OK = true, Ciclos = ciclos };
			}
			catch (Exception ex) {
				return new { OK = false, Msg = "Erro BuscarCiclos: " + ex.Message };
			}
		}

	}
}