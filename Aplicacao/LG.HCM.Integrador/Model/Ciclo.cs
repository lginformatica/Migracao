/// <summary>
/// Classe Modelo de Ciclos 
/// </summary>
namespace LG.HCM.Integrador.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Ciclo
    {
        /// <summary>
        /// Código do Ciclo
        /// </summary>
        public int CodCiclo { get; set; }

        /// <summary>
        /// Nome ou descrição do Ciclo
        /// </summary>
        public string NomeCiclo { get; set; }

        /// <summary>
        /// Data de início do Ciclo
        /// </summary>
        public DateTime DataInicio { get; set; }

        /// <summary>
        /// Data final do Ciclo
        /// </summary>
        public DateTime DataFim { get; set; }
    }
}
