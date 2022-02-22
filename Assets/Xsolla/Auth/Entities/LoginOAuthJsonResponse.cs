using System;

namespace Xsolla.Auth
{
	[Serializable]
	public class LoginOAuthJsonResponse
	{
		public string access_token;
		public string token_type;
		public int expires_in;
		public string refresh_token;
		public string scope;
	}
}
