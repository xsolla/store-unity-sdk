using System;

namespace Xsolla.Login
{
	[Serializable]
	public class StartAuthByEmailRequest
	{
		public string email;
		public string link_url;
		public bool send_link;

		public StartAuthByEmailRequest(string email, string linkUrl, bool sendLink)
		{
			this.email = email;
			link_url = linkUrl;
			send_link = sendLink;
		}
	}
}