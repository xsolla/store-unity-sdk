using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Xsolla.Catalog;
using Xsolla.Core;
using Xsolla.Inventory;

namespace Xsolla.TechDemo.Scenarios
{
	public class GrantsPersonalizedItemToUserInventoryAfterPurchase : Scenario
	{
		private const string PersonalizedItemSku = "xsollus_personalized_item";
		private const int InventoryCheckAttempts = 5;
		private const float InventoryCheckDelaySeconds = 1.0f;

		public override IEnumerator Execute(Action onSuccess, Action<Error> onError)
		{
			InitExecution(onSuccess, onError);

			var authFailed = false;
			yield return AuthAsDefaultUser(error => {
				authFailed = true;
				TryCompleteError(error);
			});

			if (authFailed || IsCompleted)
				yield break;

			var previousQuantityReceived = false;
			var previousQuantity = 0;
			Error previousQuantityError = null;

			RequestItemQuantity(
				quantity => {
					previousQuantity = quantity;
					previousQuantityReceived = true;
				},
				error => {
					previousQuantityError = error;
					previousQuantityReceived = true;
				});

			yield return new WaitUntil(() => previousQuantityReceived);

			if (IsCompleted)
				yield break;

			if (previousQuantityError != null)
			{
				TryCompleteError(previousQuantityError);
				yield break;
			}

			var purchaseCompleted = false;
			Error purchaseError = null;

			XsollaCatalog.Purchase(
				PersonalizedItemSku,
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

			yield return VerifyItemQuantityIncreaseWithRetry(previousQuantity);
		}

		private IEnumerator VerifyItemQuantityIncreaseWithRetry(int previousQuantity)
		{
			var expectedQuantity = previousQuantity + 1;

			for (var attempt = 0; attempt < InventoryCheckAttempts; attempt++)
			{
				var currentQuantityReceived = false;
				var currentQuantity = 0;
				Error quantityError = null;

				RequestItemQuantity(
					quantity => {
						currentQuantity = quantity;
						currentQuantityReceived = true;
					},
					error => {
						quantityError = error;
						currentQuantityReceived = true;
					});

				yield return new WaitUntil(() => currentQuantityReceived);

				if (IsCompleted)
					yield break;

				if (quantityError == null && currentQuantity - previousQuantity == 1)
				{
					TryCompleteSuccess();
					yield break;
				}

				var isLastAttempt = attempt == InventoryCheckAttempts - 1;
				if (isLastAttempt)
				{
					if (quantityError != null)
						TryCompleteError(quantityError);
					else
						TryCompleteError(new Error(errorMessage: "Unexpected personalized item amount. Expected: " + expectedQuantity + ". Actual: " + currentQuantity));

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
						.Where(item => item.sku == PersonalizedItemSku)
						.Sum(item => item.quantity ?? 1);

					onSuccess?.Invoke(quantity);
				},
				error => onError?.Invoke(error));
		}

	}
}