using System;
using Newtonsoft.Json;

namespace Xsolla.Login
{
	[Serializable]
	public class StartAuthByEmailRequest
	{
		public string email;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string link_url;
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public bool? send_link;

		public StartAuthByEmailRequest(string email, string linkUrl = null, bool? sendLink = null)
		{
			this.email = email;
			link_url = linkUrl;
			send_link = sendLink;
		}
	}
}