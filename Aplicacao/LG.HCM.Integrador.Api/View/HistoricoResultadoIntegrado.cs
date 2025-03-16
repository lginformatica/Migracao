namespace LG.HCM.Integrador.Api.View {
    public class HistoricoResultadoIntegrado {
        /// <summary>
        /// Código do Clico
        /// </summary>
        public int CodigoCiclo { get; set; }
        /// <summary>
        /// Nome do Clico
        /// </summary>
        public string Ciclo { get; set; }
        /// <summary>
        /// Informa se o ciclo é o atual
        /// </summary>
        public bool cicloAtual { get; set; }
        /// <summary>
        /// Resultado de Metas
        /// </summary>
        public LG.Metas.Api.View.ResultadoProfileMetas resultadoMetas { get; set; }
        /// <summary>
        /// Resultado de Competências
        /// </summary>
        public LG.Competence.Api.View.HistoricoAvaliacoesIntegrado resultadoCompetencias { get; set; }
        /// <summary>
        /// Resultado de Calibragem
        /// </summary>
        public LG.Calibragem.Api.View.ResultadoIntegrado resultadoCalibragem { get; set; }
    }
}
