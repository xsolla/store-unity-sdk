using System;
using System.Collections;
using System.Linq;
using UnityEngine;
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

		private Action OnSuccessCallback { get; set; }
		private Action<Error> OnErrorCallback { get; set; }
		private bool IsCompleted { get; set; }

		public override IEnumerator Execute(Action onSuccess, Action<Error> onError)
		{
			OnSuccessCallback = onSuccess;
			OnErrorCallback = onError;
			IsCompleted = false;

			var authFailed = false;
			yield return AuthAsDefaultUser(error => {
				authFailed = true;
				TryCompleteError(error);
			});

			if (authFailed || IsCompleted)
				yield break;

			var initialQuantityReceived = false;
			var initialQuantity = 0;
			Error initialQuantityError = null;

			RequestItemQuantity(
				quantity => {
					initialQuantity = quantity;
					initialQuantityReceived = true;
				},
				error => {
					initialQuantityError = error;
					initialQuantityReceived = true;
				});

			yield return new WaitUntil(() => initialQuantityReceived);

			if (IsCompleted)
				yield break;

			if (initialQuantityError != null)
			{
				TryCompleteError(initialQuantityError);
				yield break;
			}

			var purchaseCompleted = false;
			Error purchaseError = null;

			XsollaCatalog.Purchase(
				ConsumableItemSku,
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

			yield return VerifyQuantityWithRetry(
				expectedQuantity: initialQuantity + 1,
				errorPrefix: "Unexpected consumable item amount after purchase.",
				completeOnSuccess: false);

			if (IsCompleted)
				yield break;

			var consumeCompleted = false;
			Error consumeError = null;
			var consumeItem = new ConsumeItem
			{
				sku = ConsumableItemSku,
				quantity = 1
			};

			XsollaInventory.ConsumeInventoryItem(
				consumeItem,
				() => consumeCompleted = true,
				error => {
					consumeError = error;
					consumeCompleted = true;
				});

			yield return new WaitUntil(() => consumeCompleted);

			if (IsCompleted)
				yield break;

			if (consumeError != null)
			{
				TryCompleteError(consumeError);
				yield break;
			}

			yield return VerifyQuantityWithRetry(
				expectedQuantity: initialQuantity,
				errorPrefix: "Unexpected consumable item amount after consume.",
				completeOnSuccess: true);
		}

		private IEnumerator VerifyQuantityWithRetry(int expectedQuantity, string errorPrefix, bool completeOnSuccess)
		{
			for (var attempt = 0; attempt < InventoryCheckAttempts; attempt++)
			{
				var quantityReceived = false;
				var currentQuantity = 0;
				Error quantityError = null;

				RequestItemQuantity(
					quantity => {
						currentQuantity = quantity;
						quantityReceived = true;
					},
					error => {
						quantityError = error;
						quantityReceived = true;
					});

				yield return new WaitUntil(() => quantityReceived);

				if (IsCompleted)
					yield break;

				if (quantityError == null && currentQuantity == expectedQuantity)
				{
					if (completeOnSuccess)
						TryCompleteSuccess();

					yield break;
				}

				var isLastAttempt = attempt == InventoryCheckAttempts - 1;
				if (isLastAttempt)
				{
					if (quantityError != null)
						TryCompleteError(quantityError);
					else
						TryCompleteError(new Error(errorMessage: errorPrefix + " Expected: " + expectedQuantity + ". Actual: " + currentQuantity));

					yield break;
				}

				yield return new WaitForSeconds(InventoryCheckDelaySeconds);
			}

			TryCompleteError(new Error(errorMessage: "Inventory verification did not complete."));
		}

		private void RequestItemQuantity(Action<int> onSuccess, Action<Error> onError)
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

		private void TryCompleteSuccess()
		{
			if (IsCompleted)
				return;

			IsCompleted = true;
			OnSuccessCallback?.Invoke();
		}

		private void TryCompleteError(Error error)
		{
			if (IsCompleted)
				return;

			IsCompleted = true;
			OnErrorCallback?.Invoke(error);
		}
	}
}