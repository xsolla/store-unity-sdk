using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Xsolla.Auth
{
	[Serializable]
	public class RegistrationJson
	{
		public string username;
		public string password;
		public string email;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public bool? accept_consent;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<string> fields;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? promo_email_agreement;

		public RegistrationJson(string userName, string password, string email, bool? acceptConsent = null, List<string> fields = null, bool? promoEmailAgreement = null)
		{
			this.username = userName;
			this.password = password;
			this.email = email;
			this.accept_consent = acceptConsent;
			this.fields = fields;

			if (promoEmailAgreement.HasValue)
				this.promo_email_agreement = promoEmailAgreement.Value ? 1 : 0;
		}
	}
}
