using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

		private Dictionary<string, int> BundleItems { get; set; }

		public override IEnumerator Execute(Action onSuccess, Action<Error> onError)
		{
			InitExecution(onSuccess, onError);

			BundleItems = new Dictionary<string, int> {
				{ "key_1", 1 },
				{ "lootbox_1", 1 }
			};

			var authFailed = false;
			yield return AuthAsDefaultUser(error => {
				authFailed = true;
				TryCompleteError(error);
			});

			if (authFailed || IsCompleted)
				yield break;

			var initialQuantitiesReceived = false;
			Dictionary<string, int> initialQuantities = null;
			Error initialQuantitiesError = null;

			RequestBundleQuantities(
				quantities => {
					initialQuantities = quantities;
					initialQuantitiesReceived = true;
				},
				error => {
					initialQuantitiesError = error;
					initialQuantitiesReceived = true;
				});

			yield return new WaitUntil(() => initialQuantitiesReceived);

			if (IsCompleted)
				yield break;

			if (initialQuantitiesError != null)
			{
				TryCompleteError(initialQuantitiesError);
				yield break;
			}

			var purchaseCompleted = false;
			Error purchaseError = null;

			XsollaCatalog.Purchase(
				PackSku,
				_ => purchaseCompleted = true,
				error => {
					purchaseError = error;
					purchaseCompleted = true;
				});

			yield return new WaitUntil(() => purchaseCompleted);

			if (IsCompleted)
				yield break;

			if (purchaseError != null)
			{
				TryCompleteError(purchaseError);
				yield break;
			}

			yield return VerifyBundleIncreaseWithRetry(initialQuantities);
		}

		private IEnumerator VerifyBundleIncreaseWithRetry(Dictionary<string, int> initialQuantities)
		{
			for (var attempt = 0; attempt < InventoryCheckAttempts; attempt++)
			{
				var currentQuantitiesReceived = false;
				Dictionary<string, int> currentQuantities = null;
				Error currentQuantitiesError = null;

				RequestBundleQuantities(
					quantities => {
						currentQuantities = quantities;
						currentQuantitiesReceived = true;
					},
					error => {
						currentQuantitiesError = error;
						currentQuantitiesReceived = true;
					});

				yield return new WaitUntil(() => currentQuantitiesReceived);

				if (IsCompleted)
					yield break;

				Error mismatchError = null;
				if (currentQuantitiesError == null && IsBundleIncreaseValid(initialQuantities, currentQuantities, out mismatchError))
				{
					TryCompleteSuccess();
					yield break;
				}

				var isLastAttempt = attempt == InventoryCheckAttempts - 1;
				if (isLastAttempt)
				{
					if (currentQuantitiesError != null)
						TryCompleteError(currentQuantitiesError);
					else
						TryCompleteError(mismatchError);

					yield break;
				}

				yield return new WaitForSeconds(InventoryCheckDelaySeconds);
			}

			TryCompleteError(new Error(errorMessage: "Inventory verification did not complete."));
		}

		private bool IsBundleIncreaseValid(Dictionary<string, int> initialQuantities, Dictionary<string, int> currentQuantities, out Error mismatchError)
		{
			foreach (var bundleItem in BundleItems)
			{
				var sku = bundleItem.Key;
				var expectedIncrease = bundleItem.Value;
				var initialQuantity = initialQuantities.GetValueOrDefault(sku);
				var currentQuantity = currentQuantities.GetValueOrDefault(sku);

				if (currentQuantity - initialQuantity != expectedIncrease)
				{
					mismatchError = new Error(errorMessage:
						"Unexpected bundle item amount for sku '" + sku + "'. Expected increase: " + expectedIncrease +
						". Initial: " + initialQuantity + ". Actual: " + currentQuantity);
					return false;
				}
			}

			mismatchError = null;
			return true;
		}

		private void RequestBundleQuantities(Action<Dictionary<string, int>> onSuccess, Action<Error> onError)
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