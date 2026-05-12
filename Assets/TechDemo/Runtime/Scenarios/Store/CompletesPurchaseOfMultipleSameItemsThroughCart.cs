using System.Collections.Generic;
using Xsolla.Cart;

namespace Xsolla.TechDemo.Scenarios
{
	public class CompletesPurchaseOfMultipleSameItemsThroughCart : CartPurchaseScenario
	{
		private const string ItemSku = "booster_max_1";
		private const int ItemQuantity = 5;

		protected override List<CartFillItem> GetCartItems()
		{
			return new List<CartFillItem>
			{
				new CartFillItem { sku = ItemSku, quantity = ItemQuantity }
			};
		}
	}
}
