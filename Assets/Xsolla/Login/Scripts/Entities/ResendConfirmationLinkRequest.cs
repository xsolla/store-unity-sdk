using System;

namespace Xsolla.Login
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