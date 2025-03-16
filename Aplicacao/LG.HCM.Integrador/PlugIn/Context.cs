using LG.HCM.Integrador.Api.View;
using System;
using System.IO;
using System.Reflection;
using W3.Library.PlugIn;

namespace LG.HCM.Integrador.PlugIn {
	public class Context : BaseContext, IDisposable {
		#region Auxiliares (Private)
		private string GetManifestContent(string manifestFile) {
			Assembly assembly = Assembly.GetAssembly(typeof(Context));
			string content = String.Empty;
			using (Stream stream = assembly.GetManifestResourceStream(manifestFile)) {
				using (StreamReader reader = new StreamReader(stream)) {
					content = reader.ReadToEnd();
				}
			}
			return content;
		}
		#endregion

		#region [ Singleton Pattern ]

		private static readonly object controleConcorrencia = new object();

		public bool IsInitialized { get; set; }

		public static Context Current {
			get {
				var _current = Singleton.Current;
				_current.Initialize();

				return _current;
			}
		}

		private void Initialize() {
			if (this.IsInitialized)
				return;

			lock (controleConcorrencia) {

				if (this.IsInitialized)
					return;

				W3.SistemaIntegrado.PlugIn.PlugInProvider<Context> contextProvider = new W3.SistemaIntegrado.PlugIn.PlugInProvider<Context>();
				contextProvider.FillContext(this);
				if (this.Init != null)
					this.Init(this, EventArgs.Empty);

				this.IsInitialized = true;
			}
		}

		private static class Singleton {
			public static Context Current;
			static Singleton() {
				Current = new Context();
			}
		}

		#endregion

		#region Events
		public event EventHandler Init;
		public event EventHandler Dispose;

		//-- Eventos de Sistema
		public event EventHandler<DesempenhoArgs> RequestResultadoDesempenho;

		public event EventHandler ApplicationStart;
		public event EventHandler BeginRequest;
		#endregion

		#region Event Methods
		public void OnRequestResultadoDesempenho(DesempenhoArgs e) {
			if (this.RequestResultadoDesempenho != null)
				this.RequestResultadoDesempenho(this, e);
		}

		public void OnApplicationStart(EventArgs e) {
			W3.Library.DataSql.GlobalSettingsModule.OnApplicationStart(this, e);
			if (this.ApplicationStart != null)
				this.ApplicationStart(this, e);
		}

		public void OnBeginRequest(EventArgs e) {
			if (this.BeginRequest != null)
				this.BeginRequest(this, e);
		}

		#endregion

		#region Methods
		public bool ChecRequestResultadoDesempenho() {
			return this.RequestResultadoDesempenho != null;
		}
		#endregion

		#region IDisposable
		void IDisposable.Dispose() {
			if (this.Dispose != null)
				this.Dispose(this, EventArgs.Empty);
		}
		#endregion
	}
}