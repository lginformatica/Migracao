using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LG.HCM.Integrador.Model.Integracoes.Clave {
	public class LoginSSO {
		public string nome { get; set; }
		public string email { get; set; }
		public string cliente { get; set; }
		public string chave { get; set; }
		public string data { get; set; }

		public LoginSSO(string nome, string email, int codigoCliente, string valorChave, string dataExecucao) {
			this.nome = nome;
			this.email = email;
			this.cliente = codigoCliente.ToString();
			this.chave = valorChave;
			this.data = dataExecucao;
		}
	}
}
