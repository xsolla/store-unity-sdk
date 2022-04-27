using System;
using Newtonsoft.Json;

namespace Xsolla.Auth
{
	[Serializable]
	public class StartAuthByPhoneNumberRequest
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string link_url;
		public string phone_number;
		public bool send_link;

		public StartAuthByPhoneNumberRequest(string phoneNumber, string linkUrl, bool sendLink)
		{
			link_url = linkUrl;
			phone_number = phoneNumber;
			send_link = sendLink;
		}
	}
}
