using System;
using System.Collections;
using System.Linq;
using Xsolla.Catalog;
using Xsolla.Core;
using Xsolla.Inventory;

namespace Xsolla.TechDemo.Scenarios
{
	public class ConsumesConsumableItemAndUpdatesQuantity : Scenario
	{
		private const string ConsumableItemSku = "booster_max_1";
		private const int InventoryCheckAttempts = 5;
		private const float InventoryCheckDelaySeconds = 1.0f;

		public override IEnumerator Execute(Action onSuccess, Action<Error> onError)
		{
			InitExecution(onSuccess, onError);

			yield return AuthAsDefaultUser();
			if (IsCompleted)
				yield break;

			var initialQuantity = 0;
			yield return RunAsync<int>(RequestItemQuantity, quantity => initialQuantity = quantity);
			if (IsCompleted)
				yield break;

			yield return RunAsync<OrderStatus>((success, failure) => XsollaCatalog.Purchase(ConsumableItemSku, success, failure));
			if (IsCompleted)
				yield break;

			var expectedAfterPurchase = initialQuantity + 1;
			yield return VerifyWithRetry<int>(
				InventoryCheckAttempts,
				InventoryCheckDelaySeconds,
				RequestItemQuantity,
				current => current == expectedAfterPurchase
					? null
					: new Error(errorMessage: "Unexpected consumable item amount after purchase. Expected: " + expectedAfterPurchase + ". Actual: " + current),
				false);

			if (IsCompleted)
				yield break;

			var consumeItem = new ConsumeItem {
				sku = ConsumableItemSku,
				quantity = 1
			};
			yield return RunAsync((success, failure) => XsollaInventory.ConsumeInventoryItem(consumeItem, success, failure));
			if (IsCompleted)
				yield break;

			yield return VerifyWithRetry<int>(
				InventoryCheckAttempts,
				InventoryCheckDelaySeconds,
				RequestItemQuantity,
				current => current == initialQuantity
					? null
					: new Error(errorMessage: "Unexpected consumable item amount after consume. Expected: " + initialQuantity + ". Actual: " + current));
		}

		private static void RequestItemQuantity(Action<int> onSuccess, Action<Error> onError)
		{
			XsollaInventory.GetInventoryItems(
				items => {
					var quantity = items.items
						.Where(item => item.sku == ConsumableItemSku)
						.Sum(item => item.quantity ?? 1);

					onSuccess?.Invoke(quantity);
				},
				error => onError?.Invoke(error));
		}
	}
}