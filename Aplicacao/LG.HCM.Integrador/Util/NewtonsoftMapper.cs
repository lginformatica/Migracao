
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LG.HCM.Integrador.Util {
    public class NewtonsoftMapper {
        public string Serialize(object obj) {
            var settings = new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
            };

            return JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
        }

        public T Parse<T>(string json) {
            var settings = new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
            };

            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}