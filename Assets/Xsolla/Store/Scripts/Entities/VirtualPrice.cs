using System;
using Newtonsoft.Json;

namespace Xsolla.Store
{
	[Serializable]
	public class VirtualPrice
	{
		[Serializable]
		private class CalculatedPrice
		{
			public string amount = default;
			public string amount_without_discount = default;
		}
		public string sku;
		public string name;
		public string type;
		public string description;
		public string image_url;
		public uint amount;
		public uint amount_without_discount;
		[JsonProperty]private CalculatedPrice calculated_price = default;
		public bool is_default;
		

		public uint GetAmount()
		{
			return amount;
		}

		public uint GetAmountWithoutDiscount()
		{
			return amount_without_discount;
		}
		
		public string GetAmountRaw()
		{
			return calculated_price.amount;
		}

		public string GetAmountWithoutDiscountRaw()
		{
			return calculated_price.amount_without_discount;
		}
	}
}
