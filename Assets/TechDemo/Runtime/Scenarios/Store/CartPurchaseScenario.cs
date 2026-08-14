using System;
using System.Collections;
using System.Collections.Generic;
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

			yield return AuthAsDefaultUser();
			if (IsCompleted)
				yield break;

			yield return RunAsync((success, failure) => XsollaCart.ClearCart(success, failure));
			if (IsCompleted)
				yield break;

			yield return RunAsync((success, failure) => XsollaCart.FillCart(GetCartItems(), success, failure));
			if (IsCompleted)
				yield break;

			OrderStatus purchaseStatus = null;
			yield return RunAsync<OrderStatus>(
				(success, failure) => XsollaCart.Purchase(success, failure),
				status => purchaseStatus = status);

			if (IsCompleted)
				yield break;

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