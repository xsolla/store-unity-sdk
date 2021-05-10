using Newtonsoft.Json;
using System;

namespace Xsolla.Login
{
	[Serializable]
	public class AddUsernameAndEmailRequest
	{
		public string email;
		public string password;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? promo_email_agreement;

		public string username;

		public AddUsernameAndEmailRequest(string email, string password, int? promo_email_agreement, string username)
		{
			this.email = email;
			this.password = password;
			this.promo_email_agreement = promo_email_agreement;
			this.username = username;
		}
	}
}
