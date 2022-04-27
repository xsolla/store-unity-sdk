using System;

namespace Xsolla.Auth
{
	[Serializable]
	public class ResendConfirmationLinkRequest
	{
		public string username;

		public ResendConfirmationLinkRequest(string username)
		{
			this.username = username;
		}
	}
}
