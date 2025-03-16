<%@ WebHandler Language="C#" Class="Substituir" %>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Xml.Linq;
using Aspose.Cells;
using W3.Framework.Servico;
using W3.Framework.Servico.ServicoColecao;
using W3.Framework.Servico.Colecao;
using W3.Framework.Servico.Colecao.Classes;
using W3.Framework.Servico.Colecao.Utilitarios;

/// <summary>
/// Summary description for Configuracoes
/// </summary>
public class Substituir : ServicoBaseAshx {
    public object RetornarFormulario() {
        string form = RetornarFormularioHtml();
        XElement xForm = XElement.Parse(form);
        XElement xEl = xForm.Descendants("input").Where(p => (string)p.Attribute("id") == "LocalizarIdioma").FirstOrDefault();
        foreach (var idioma in Colecao<Idioma>.Instance.Itens.Where(p => p.IndAtivo == "S").OrderBy(p => p.CodIdioma)) {
            xEl.AddBeforeSelf(XElement.Parse("<input id='ckIdioma'  type='radio' name='ckIdioma' class='radio' " + ((Idioma == idioma.SglIdioma) ? " checked='checked' " : "") + "value='" + idioma.SglIdioma + "' style='margin-left: 12px' />"));
            xEl.AddBeforeSelf(XElement.Parse("<span>" + idioma.NomIdioma + "</span>"));
        }
        //xEl.Remove();
        return new { Html = xForm.ToString() };
    }

    public object RetornaTotais(string idGrid, string palavraChave, string idioma) {
        ClGrid gridx = ClGrid.RetornaGrid(idGrid);
        if (gridx == null) {
            return new { OK = false, GridNula = true };
        }
        int codModulo = Convert.ToInt32(Context.Application["CodModulo"].ToString());
        int[] codigosBusca = (gridx.ListarRegistrosSelecionados().Count == 0) ? Colecao<Mensagem>.Instance.Itens.Where(p => p.CodModulo == codModulo).Select(p => p.ChaveHash).ToArray() : gridx.ListarRegistrosSelecionados().Select(p => p.Field<int>(BaseItem.IdChavePrimaria)).ToArray();
        int totItens = codigosBusca.Length;
        List<int> retorno = W3.Framework.Servico.Colecao.Classes.Mensagem.BuscarItens(palavraChave, idioma, "MC", codigosBusca);
        return new { OK = true, TotalItens = totItens, TotalEncontrados = retorno.Count };
    }

    public object SubstituirMensagens() {
        Validacao validacao = new Validacao();
        //if (string.IsNullOrEmpty(Parametros["LocalizarIdioma"].ToString())) {
        //    validacao.Erros.Add(new ErroValidacao() { NomColuna = "LocalizarIdioma", Validador = EnumTipoValidacao.IndPermitirNulo, Msg = Msg("preenchimento_obrigatorio") });
        //}
        if (string.IsNullOrEmpty(Parametros["Localizar"].ToString())) {
            validacao.Erros.Add(new ErroValidacao() { NomColuna = "Localizar", Validador = EnumTipoValidacao.IndPermitirNulo, Msg = Msg("preenchimento_obrigatorio") });
        }
        if (string.IsNullOrEmpty(Parametros["SubstituirPor"].ToString())) {
            validacao.Erros.Add(new ErroValidacao() { NomColuna = "SubstituirPor", Validador = EnumTipoValidacao.IndPermitirNulo, Msg = Msg("preenchimento_obrigatorio") });
        }
        string palavraChave = Parametros["Localizar"].ToString();
        string substituirPor = Parametros["SubstituirPor"].ToString();
        //string[] idiomas = Parametros["LocalizarIdioma"].ToString().Split(',');
        string[] idiomas = new string[] { Parametros["ckIdioma"].ToString() };
        validacao.OK = (validacao.Erros.Count == 0);
        if (validacao.OK) {
            string idGrid = Parametros["IdGrid"].ToString();
            ClGrid gridx = ClGrid.RetornaGrid(idGrid);
            if (gridx == null) {
                return new { OK = false, GridNula = true };
            }
            int codModulo = Convert.ToInt32(Context.Application["CodModulo"].ToString());
            int[] codigosBusca = (gridx.ListarRegistrosSelecionados().Count == 0) ? Colecao<Mensagem>.Instance.Itens.Where(p => p.CodModulo == codModulo).Select(p => p.ChaveHash).ToArray() : gridx.ListarRegistrosSelecionados().Select(p => p.Field<int>(BaseItem.IdChavePrimaria)).ToArray();
            int totItens = codigosBusca.Length;
            List<int> retorno = W3.Framework.Servico.Colecao.Classes.Mensagem.SubstituirItens(palavraChave, substituirPor, idiomas, "MC", codigosBusca);
            Context.Response.Cookies.Add(new HttpCookie("Mensagem_PalavraChave") { Expires = DateTime.Now.AddDays(7), Value = palavraChave });
            return new { OK = true, TotalItens = totItens, TotalEncontrados = retorno.Count };
        }
        else {
            return new { OK = validacao.OK, Chave = validacao.Chave, Msg = validacao.Erros.Select(p => new { Nome = p.NomColuna, Msg = p.Msg }).ToList() };
        }

    }

}