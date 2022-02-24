using System;

namespace Xsolla.Auth
{
	[Serializable]
	public class ResetPassword
	{
		public string username;

		public ResetPassword(string username)
		{
			this.username = username;
		}
	}
}
