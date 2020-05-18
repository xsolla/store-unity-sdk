using System;

namespace Xsolla.Store
{
	[Serializable]
	public class Price
	{
		public string amount;
		public string amount_without_discount;
		public string currency;

		public float GetAmount()
		{
			return float.Parse(amount);
		}

		public float GetAmountWithoutDiscount()
		{
			return float.Parse(amount_without_discount);
		}
	}
}

