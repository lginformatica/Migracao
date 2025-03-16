using System;
using System.Collections.Generic;

namespace LG.HCM.Integrador.Api.View {
    public class CicloIntegrado {
        /// <summary>
        /// Código do Clico
        /// </summary>
        public int codigoCiclo { get; set; }
        /// <summary>
        /// Nome do Clico
        /// </summary>
        public string nomeCiclo { get; set; }
        /// <summary>
        /// Numero Ordem
        /// </summary>
        public int numeroOrdem { get; set; }
        /// <summary>
        /// Data de Inicio
        /// </summary>
        public DateTime dataInicio { get; set; }
        /// <summary>
        /// Data de Fim
        /// </summary>
        public DateTime dataFim { get; set; }
        /// <summary>
        /// Compõem Resultado
        /// </summary>
        public bool compoeResultado { get; set; }
        /// <summary>
        /// Informa se o ciclo é o atual
        /// </summary>
        public bool cicloAtual { get; set; }
        /// <summary>
        /// Lista de Ciclos Integrados
        /// </summary>
        public List<CicloIntegradoModulo> ciclosIntegrados { get; set; }

    }
}
