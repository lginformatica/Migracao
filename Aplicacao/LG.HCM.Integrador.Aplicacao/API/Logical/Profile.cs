using LG.HCM.Integrador.Api.View;
using LG.HCM.Integrador.Logical;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using W3.Framework.Servico.Colecao;
using W3.Framework.Servico.Colecao.Classes;
using W3.Framework.Servico.Colecao.DadosCorporativos;

namespace LG.HCM.Integrador.Aplicacao.API.Logical {
    public class Profile {
        public static bool ValidarCodigoFuncionario(string codigoFuncionario) {
            if (Colecao<VwFuncionario>.Instance.Itens.Where(x => x.CodFuncionarioExterno == codigoFuncionario).Count() == 0) {
                return false;
            }

            return true;
        }

        public static bool ValidarIdioma(string sglIdioma) {
            if (Colecao<Idioma>.Instance.Itens.Where(x => x.SglIdioma.Equals(sglIdioma) && x.IndAtivo.Equals("S")).Count() == 0) {
                return false;
            }

            return true;
        }

        public static List<LG.Metas.Api.View.ResultadoProfileMetas> RecuperarResultadosCiclosMetas(string CodigoFuncionarioAcesso, string CodigoFuncionario, string SglIdioma, string CodigosCiclos) {
            List<LG.Metas.Api.View.ResultadoProfileMetas> listaCiclosMetas = new List<LG.Metas.Api.View.ResultadoProfileMetas>();

            if (!LG.HCM.Integrador.Logical.Util.ValidaModulo(LG.HCM.Integrador.Logical.EnumModulo.Metas)) {
                throw new Exception("Módulo não instalado");
            }

            var rotaApiMetas = string.Format("api/v1/profile/historicoAvaliacoes/{0}/{1}/{2}/{3}/", CodigoFuncionarioAcesso, CodigoFuncionario, SglIdioma, CodigosCiclos);
            var integracaoApiMetas = ExecutarApiGet(rotaApiMetas, Integrador.Logical.EnumModulo.MetasNovo);
            if (integracaoApiMetas.StatusCode == HttpStatusCode.OK) {
                RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
                if (integracaoApiMetas.Content != "null")
                    return deserial.Deserialize<List<LG.Metas.Api.View.ResultadoProfileMetas>>(integracaoApiMetas);
            }

            return listaCiclosMetas;
        }

        public static List<LG.Competence.Api.View.HistoricoAvaliacoesIntegrado> RecuperarResultadosCiclosCompetencias(string CodigoFuncionarioAcesso, string CodigoFuncionario, string SglIdioma, string CodigosCiclos) {
            List<LG.Competence.Api.View.HistoricoAvaliacoesIntegrado> listaCiclosCompentencias = new List<LG.Competence.Api.View.HistoricoAvaliacoesIntegrado>();

            if (!LG.HCM.Integrador.Logical.Util.ValidaModulo(LG.HCM.Integrador.Logical.EnumModulo.Competencias)) {
                throw new Exception("Módulo não instalado");
            }

            var rotaApiCompetencias = string.Format("api/v1/profile/historicoAvaliacoesIntegrado/{0}/{1}/{2}/{3}/", CodigoFuncionarioAcesso, CodigoFuncionario, SglIdioma, CodigosCiclos);
            var integracaoApiCompetencias = ExecutarApiGet(rotaApiCompetencias, Integrador.Logical.EnumModulo.Competencias);
            if (integracaoApiCompetencias.StatusCode == HttpStatusCode.OK) {
                RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
                if (integracaoApiCompetencias.Content != "null")
                    return deserial.Deserialize<List<LG.Competence.Api.View.HistoricoAvaliacoesIntegrado>>(integracaoApiCompetencias);
            }

            return listaCiclosCompentencias;
        }

        public static List<LG.Calibragem.Api.View.ResultadoIntegrado> RecuperarResultadosCiclosCalibragem(string CodigoFuncionarioAcesso, string CodigoFuncionario, string SglIdioma, string CodigosCiclos) {
            List<LG.Calibragem.Api.View.ResultadoIntegrado> listaCiclosCalibragem = new List<LG.Calibragem.Api.View.ResultadoIntegrado>();
            // Recupera informacoes Calibragem
            if (!LG.HCM.Integrador.Logical.Util.ValidaModulo(LG.HCM.Integrador.Logical.EnumModulo.Calibragem)) {
                throw new Exception("Módulo não instalado");
            }

            var rotaApiCalibragem = string.Format("api/v1/profile/historicoAvaliacoes/{0}/{1}/{2}/{3}/", CodigoFuncionarioAcesso, CodigoFuncionario, SglIdioma, CodigosCiclos);
            var integracaoApiCalibragem = ExecutarApiGet(rotaApiCalibragem, Integrador.Logical.EnumModulo.Calibragem);
            if (integracaoApiCalibragem.StatusCode == HttpStatusCode.OK) {
                RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
                if (integracaoApiCalibragem.Content != "null")
                    return deserial.Deserialize<List<LG.Calibragem.Api.View.ResultadoIntegrado>>(integracaoApiCalibragem);

            }

            return listaCiclosCalibragem;
        }

        public static IRestResponse ExecutarApiGet(string rotaApi, Integrador.Logical.EnumModulo enumModulo) {
            var request = new RestRequest(rotaApi, Method.GET);

            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-type", "application/json");

            var authApi = LG.HCM.Integrador.Logical.Autenticacao.Autenticar(enumModulo);
            IRestResponse integracaoApi = authApi.Execute(request);

            return integracaoApi;
        }
    }
}