using System.Collections.Generic;
using Xsolla.Cart;

namespace Xsolla.TechDemo.Scenarios
{
	public class CompletesPurchaseOfSingleItemThroughCart : CartPurchaseScenario
	{
		private const string ItemSku = "booster_max_1";

		protected override List<CartFillItem> GetCartItems()
		{
			return new List<CartFillItem> {
				new() {
					sku = ItemSku,
					quantity = 1
				}
			};
		}
	}
}