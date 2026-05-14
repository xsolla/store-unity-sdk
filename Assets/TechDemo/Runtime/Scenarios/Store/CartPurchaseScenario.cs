using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Cart;
using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public abstract class CartPurchaseScenario : Scenario
	{
		protected abstract List<CartFillItem> GetCartItems();

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
				GetCartItems(),
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
	}
}
