using Newtonsoft.Json;
using System;

namespace Xsolla.Login
{
	[Serializable]
	public class LoginRequest
	{
		public string username;
		public string password;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public bool? remember_me;

		public LoginRequest(string username, string password, bool? rememberMe = null)
		{
			this.username = username;
			this.password = password;
			this.remember_me = rememberMe;
		}
	}
}
