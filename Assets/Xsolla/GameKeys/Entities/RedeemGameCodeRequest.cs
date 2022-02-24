using System;

namespace Xsolla.GameKeys
{
	[Serializable]
	public class RedeemGameCodeRequest
	{
		public string code;
		public bool sandbox;

		public RedeemGameCodeRequest(string code, bool sandbox)
		{
			this.code = code;
			this.sandbox = sandbox;
		}
	}
}
