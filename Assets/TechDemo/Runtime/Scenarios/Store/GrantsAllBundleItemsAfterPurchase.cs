using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xsolla.Catalog;
using Xsolla.Core;
using Xsolla.Inventory;

namespace Xsolla.TechDemo.Scenarios
{
	public class GrantsAllBundleItemsAfterPurchase : Scenario
	{
		private const string PackSku = "lootbox_pack_1";
		private const int InventoryCheckAttempts = 5;
		private const float InventoryCheckDelaySeconds = 1.0f;

		private static readonly Dictionary<string, int> BundleItems = new() {
			{ "key_1", 1 },
			{ "lootbox_1", 1 }
		};

		public override IEnumerator Execute(Action onSuccess, Action<Error> onError)
		{
			InitExecution(onSuccess, onError);

			yield return AuthAsDefaultUser();
			if (IsCompleted)
				yield break;

			Dictionary<string, int> initialQuantities = null;
			yield return RunAsync<Dictionary<string, int>>(RequestBundleQuantities, quantities => initialQuantities = quantities);
			if (IsCompleted)
				yield break;

			yield return RunAsync<OrderStatus>((success, failure) => XsollaCatalog.Purchase(PackSku, success, failure));
			if (IsCompleted)
				yield break;

			yield return VerifyWithRetry<Dictionary<string, int>>(
				InventoryCheckAttempts,
				InventoryCheckDelaySeconds,
				RequestBundleQuantities,
				current => GetBundleMismatchError(initialQuantities, current));
		}

		private static Error GetBundleMismatchError(Dictionary<string, int> initialQuantities, Dictionary<string, int> currentQuantities)
		{
			foreach (var bundleItem in BundleItems)
			{
				var sku = bundleItem.Key;
				var expectedIncrease = bundleItem.Value;
				var initialQuantity = initialQuantities.GetValueOrDefault(sku);
				var currentQuantity = currentQuantities.GetValueOrDefault(sku);

				if (currentQuantity - initialQuantity != expectedIncrease)
				{
					return new Error(errorMessage:
						"Unexpected bundle item amount for sku '" + sku + "'. Expected increase: " + expectedIncrease +
						". Initial: " + initialQuantity + ". Actual: " + currentQuantity);
				}
			}

			return null;
		}

		private static void RequestBundleQuantities(Action<Dictionary<string, int>> onSuccess, Action<Error> onError)
		{
			XsollaInventory.GetInventoryItems(
				items => {
					var quantitiesBySku = items.items
						.Where(item => BundleItems.ContainsKey(item.sku))
						.GroupBy(item => item.sku)
						.ToDictionary(group => group.Key, group => group.Sum(item => item.quantity ?? 1));

					foreach (var bundleItem in BundleItems.Keys)
					{
						if (!quantitiesBySku.ContainsKey(bundleItem))
							quantitiesBySku[bundleItem] = 0;
					}

					onSuccess?.Invoke(quantitiesBySku);
				},
				error => onError?.Invoke(error));
		}
	}
}