using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using W3.Framework.Servico.Colecao;
using W3.Framework.Servico.Colecao.Classes;
using W3.Library.Data;

namespace LG.HCM.Integrador.Classes
{
    public class TipoIntegracao : BaseItem
    {
        #region Propriedades basicas
        public Int32 CodTipoIntegracao { get; set; }
        public String NomTipoIntegracao { get; set; }
        #endregion
    }

    public enum EnumTipoIntegracao
    {
        Autoatendimento = 1,
        AutenticacaoIntegrada = 2
    }

}