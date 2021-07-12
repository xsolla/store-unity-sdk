using System;
using System.Collections.Generic;
using System.Linq;

namespace Xsolla.Login
{
	[Serializable]
	public class RegistrationJson
	{
		public string username;
		public string password;
		public string email;
		public bool accept_consent;
		public int promo_email_agreement;
		public List<string> fields;

		public RegistrationJson(string userName, string password, string email, bool acceptConsent, bool promoEmailAgreement, List<string> fields)
		{
			username = userName;
			this.password = password;
			this.email = email;
			accept_consent = acceptConsent;
			promo_email_agreement = promoEmailAgreement ? 1 : 0;
			this.fields = fields != null && fields.Any() ? fields : new List<string>();
		}
	}
}