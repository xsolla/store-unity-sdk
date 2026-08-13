using System;
using System.Collections;
using System.Linq;
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

			yield return AuthAsDefaultUser();
			if (IsCompleted)
				yield break;

			var previousQuantity = 0;
			yield return RunAsync<int>(RequestItemQuantity, quantity => previousQuantity = quantity);
			if (IsCompleted)
				yield break;

			yield return RunAsync<OrderStatus>((success, failure) => XsollaCatalog.Purchase(PersonalizedItemSku, success, failure));
			if (IsCompleted)
				yield break;

			var expectedQuantity = previousQuantity + 1;
			yield return VerifyWithRetry<int>(
				InventoryCheckAttempts,
				InventoryCheckDelaySeconds,
				RequestItemQuantity,
				current => current == expectedQuantity
					? null
					: new Error(errorMessage: "Unexpected personalized item amount. Expected: " + expectedQuantity + ". Actual: " + current));
		}

		private static void RequestItemQuantity(Action<int> onSuccess, Action<Error> onError)
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