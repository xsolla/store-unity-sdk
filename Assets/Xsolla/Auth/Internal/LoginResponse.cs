using System;

namespace Xsolla.Auth
{
	[Serializable]
	internal class LoginResponse
	{
		public string login_url;

		public string token;
	}
}