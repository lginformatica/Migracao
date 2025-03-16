namespace LG.HCM.Integrador.Model {
	public class ConfiguracaoApi {
		public int CodigoApi { get; set; }
		public int CodigoModulo { get; set; }
		public int? CodigoRegraVerAvaliacao { get; set; }

		public string TipoValidacaoCompetencias { get; set; }

		public string TipoNotaOrigemCompetencias { get; set; }
		public string TipoNotaOrigemCalibragem { get; set; }
		public string TipoNotaOrigemMetas { get; set; }

		public bool ExpirarResultadosPorTempo { get; set; }
		public bool UtilizarAnoDeReferencia { get; set; }
		public bool UtilizarRangeEspecifico { get; set; }

		public int? QuantidadeDiasExpiracao { get; set; }
		public int? AnoDeReferencia { get; set; }
		public double? ValorMinimoRange { get; set; }
		public double? ValorMaximoRange { get; set; }
	}

	public enum OrigemResultadoDesempenho { 
		NotaDoEixoX = 'X',
		NotaDoEixoY = 'Y',
		NotaDoEixoZ = 'Z',
		ResultadoCiclo = 'C'
	}

	public enum TipoValidacaoCompetencias { 
		ResultadoConsolidado = 'C',
		RegraDoVerAvaliacao = 'V',
		RegraEspecifica = 'R'
	}

	public enum ApisComConfiguracao { 
		WorkflowMovimentacaoEDesligamento = 1
	}
}
