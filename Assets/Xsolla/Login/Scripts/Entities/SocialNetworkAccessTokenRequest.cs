using System;

namespace Xsolla.Login
{
	[Serializable]
	public class SocialNetworkAccessTokenRequest
	{
		public string access_token;
		public string access_token_secret;
		public string openId;
	}
}