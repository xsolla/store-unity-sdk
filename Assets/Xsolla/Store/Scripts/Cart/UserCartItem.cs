using Xsolla.Core;

namespace Xsolla.Demo
{
	public class UserCartItem
	{
		public string Sku { get; set; }
		public string Name { get; set; }
		public float  Price { get; set; }
		public float  PriceWithoutDiscount { get; set; }
		public string Currency { get; set; }
		public string ImageUrl { get; set; }
		public float  TotalPrice => PriceWithoutDiscount * Quantity;
		
		public int Quantity { get; set; }

		public UserCartItem()
		{
			Quantity = 1;
		}

		private static bool IsInRange(int value, int minimum, int maximum)
		{
			return value >= minimum && value <= maximum;
		}

		public override bool Equals(object obj)
		{
			return obj is UserCartItem item && Sku.Equals(item.Sku);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}