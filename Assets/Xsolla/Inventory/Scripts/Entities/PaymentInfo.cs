using System;

namespace Xsolla.Store
{
	[Serializable]
	public class PaymentInfo
	{
		public string status { get; set; }

		public string email { get; set; }

		public int invoice { get; set; }

		public object virtualCurrencyAmount { get; set; }

		public string userId { get; set; }

		public object discount { get; set; }
	}
}