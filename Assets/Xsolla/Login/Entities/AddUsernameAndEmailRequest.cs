using Newtonsoft.Json;
using System;

namespace Xsolla.Login
{
	[Serializable]
	public class AddUsernameAndEmailRequest
	{
		public string username;
		public string password;
		public string email;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? promo_email_agreement;

		public AddUsernameAndEmailRequest(string username, string password, string email, int? promo_email_agreement)
		{
			this.username = username;
			this.password = password;
			this.email = email;
			this.promo_email_agreement = promo_email_agreement;
		}
	}
}
