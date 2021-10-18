using System;

namespace Xsolla.Login
{
	[Serializable]
	public class StartAuthByPhoneNumberRequest
	{
		public string phone_number;
		public string link_url;
		public bool send_link;

		public StartAuthByPhoneNumberRequest(string phoneNumber, string linkUrl, bool sendLink)
		{
			phone_number = phoneNumber;
			link_url = linkUrl;
			send_link = sendLink;
		}
	}
}