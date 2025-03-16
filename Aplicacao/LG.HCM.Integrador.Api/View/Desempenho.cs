using System;

namespace LG.HCM.Integrador.Api.View {
	/// <summary>
	/// Dados de Performance
	/// </summary>
	public class Desempenho {
		/// <summary>
		/// Código de Funcionário do Colaborador
		/// </summary>
		public string CodigoFuncionario { get; set; }
		/// <summary>
		/// Nome do Colaborador
		/// </summary>
		public string NomeFuncionario { get; set; }
		/// <summary>
		/// Resultado do Desempenho do Colaborador
		/// </summary>
		public NomeIdiomizado ResultadoDesempenho { get; set; }
		/// <summary>
		/// Nota de Desempenho do Colaborador
		/// </summary>
		public decimal? NotaDesempenho { get; set; }
		/// <summary>
		/// Ano de referência do resultado de Desempenho
		/// </summary>
		public int? AnoReferencia { get; set; }
	}

	public class NomeIdiomizado {
		public NomeIdiomizado(string ptBR, string enUS, string esES) {
			this.ptBR = ptBR;
			this.enUS = enUS;
			this.esES = esES;
		}

		public string ptBR { get; set; }
		public string enUS { get; set; }
		public string esES { get; set; }
	}

	public class DesempenhoRetorno : Desempenho {
		/// <summary>
		/// Data Inicio do resultado
		/// </summary>
		public DateTime? DataInicio { get; set; }
		/// <summary>
		/// Data Fim  do resultado
		/// </summary>
		public DateTime? DataFim { get; set; }
		/// <summary>
		/// Valor do limite inferior do range utilizado para gerar a nota de origem no módulo
		/// </summary>
		public double? ValorLimiteInferior { get; set; }
		/// <summary>
		/// Valor do limite superior do range utilizado para gerar a nota de origem no módulo
		/// </summary>
		public double? ValorLimiteSuperior { get; set; }
		/// <summary>
		/// Quantidade de dias passados desde que o resultado foi gerado
		/// </summary>
		public int QuantidadeDiasResultado{ get; set; }
}

	public class DesempenhoArgs : EventArgs {
		public string CodigoFuncionario { get; set; }
		public Desempenho Desempenho { get; set; }
	}
}
