using System;

namespace Xsolla.Subscriptions
{
	[Serializable]
	public class PaymentSettings
	{
		public bool sandbox;

		public PaymentSettings(bool sandbox)
		{
			this.sandbox = sandbox;
		}
	}
}