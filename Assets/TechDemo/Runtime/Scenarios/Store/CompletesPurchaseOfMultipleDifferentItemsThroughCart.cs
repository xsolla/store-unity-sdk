using System.Collections.Generic;
using Xsolla.Cart;

namespace Xsolla.TechDemo.Scenarios
{
	public class CompletesPurchaseOfMultipleDifferentItemsThroughCart : CartPurchaseScenario
	{
		private const string FirstItemSku = "booster_min_1";
		private const string SecondItemSku = "booster_max_1";

		protected override List<CartFillItem> GetCartItems()
		{
			return new List<CartFillItem> {
				new() {
					sku = FirstItemSku,
					quantity = 1
				},
				new() {
					sku = SecondItemSku,
					quantity = 1
				}
			};
		}
	}
}