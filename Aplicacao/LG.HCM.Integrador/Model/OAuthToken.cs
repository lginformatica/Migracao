using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LG.HCM.Integrador.Model
{
    public class OAuthToken
    {
		public string access_token { get; set; }
		public string created_at { get; set; }
		public int expires_in { get; set; }
		public string refresh_token { get; set; }
		public string token_type { get; set; }
		public int account_id { get; set; }
	}
}