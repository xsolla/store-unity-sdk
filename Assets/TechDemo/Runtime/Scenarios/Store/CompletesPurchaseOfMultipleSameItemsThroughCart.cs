using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Cart;
using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public class CompletesPurchaseOfMultipleSameItemsThroughCart : Scenario
	{
		private const string ItemSku = "booster_max_1";
		private const int ItemQuantity = 5;

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

			var clearCartCompleted = false;
			Error clearCartError = null;

			XsollaCart.ClearCart(
				() => clearCartCompleted = true,
				error => {
					clearCartError = error;
					clearCartCompleted = true;
				});

			yield return new WaitUntil(() => clearCartCompleted);

			if (IsCompleted)
				yield break;

			if (clearCartError != null)
			{
				TryCompleteError(clearCartError);
				yield break;
			}

			var fillCartCompleted = false;
			Error fillCartError = null;

			XsollaCart.FillCart(
				new List<CartFillItem>
				{
					new CartFillItem { sku = ItemSku, quantity = ItemQuantity }
				},
				() => fillCartCompleted = true,
				error => {
					fillCartError = error;
					fillCartCompleted = true;
				});

			yield return new WaitUntil(() => fillCartCompleted);

			if (IsCompleted)
				yield break;

			if (fillCartError != null)
			{
				TryCompleteError(fillCartError);
				yield break;
			}

			var purchaseCompleted = false;
			OrderStatus purchaseStatus = null;
			Error purchaseError = null;

			XsollaCart.Purchase(
				status => {
					purchaseStatus = status;
					purchaseCompleted = true;
				},
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

			if (purchaseStatus == null)
			{
				TryCompleteError(new Error(errorMessage: "Cart purchase completed without order status."));
				yield break;
			}

			if (purchaseStatus.order_id <= 0)
			{
				TryCompleteError(new Error(errorMessage: "Invalid order id after cart purchase."));
				yield break;
			}

			if (!string.Equals(purchaseStatus.status, "done", StringComparison.OrdinalIgnoreCase))
			{
				TryCompleteError(new Error(errorMessage: "Cart purchase order is not completed. Status: " + purchaseStatus.status));
				yield break;
			}

			TryCompleteSuccess();
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