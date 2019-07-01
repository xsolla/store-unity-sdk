using System;

namespace Xsolla.PayStation
{
	[Serializable]
	public class TokenRequest
	{
		public User user;
		public Settings settings;
		public Purchase purchase;
	}
}