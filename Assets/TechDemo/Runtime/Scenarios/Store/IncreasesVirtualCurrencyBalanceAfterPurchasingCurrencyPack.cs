using System;
using System.Collections;
using UnityEngine;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public class IncreasesVirtualCurrencyBalanceAfterPurchasingCurrencyPack : Scenario
	{
		private const string PackSku = "crystal_pack_3";
		private const string CurrencySku = "crystals";
		private const int BalanceIncreaseAmount = 1000;
		private const int BalanceCheckAttempts = 5;
		private const float BalanceCheckDelaySeconds = 1.0f;

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

			var previousBalanceReceived = false;
			var previousQuantity = 0;
			Error previousBalanceError = null;

			RequestCurrencyBalance(
				quantity => {
					previousQuantity = quantity;
					previousBalanceReceived = true;
				},
				error => {
					previousBalanceError = error;
					previousBalanceReceived = true;
				});

			yield return new WaitUntil(() => previousBalanceReceived);

			if (IsCompleted)
				yield break;

			if (previousBalanceError != null)
			{
				TryCompleteError(previousBalanceError);
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

			yield return VerifyBalanceIncreaseWithRetry(previousQuantity);
		}

		private IEnumerator VerifyBalanceIncreaseWithRetry(int previousQuantity)
		{
			var expectedQuantity = previousQuantity + BalanceIncreaseAmount;

			for (var attempt = 0; attempt < BalanceCheckAttempts; attempt++)
			{
				var balanceReceived = false;
				var currentQuantity = 0;
				Error balanceError = null;

				RequestCurrencyBalance(
					quantity => {
						currentQuantity = quantity;
						balanceReceived = true;
					},
					error => {
						balanceError = error;
						balanceReceived = true;
					});

				yield return new WaitUntil(() => balanceReceived);

				if (IsCompleted)
					yield break;

				if (balanceError == null && currentQuantity - previousQuantity == BalanceIncreaseAmount)
				{
					TryCompleteSuccess();
					yield break;
				}

				var isLastAttempt = attempt == BalanceCheckAttempts - 1;
				if (isLastAttempt)
				{
					if (balanceError != null)
						TryCompleteError(balanceError);
					else
						TryCompleteError(new Error(errorMessage: "Unexpected balance increase amount. Expected: " + expectedQuantity + ". Actual: " + currentQuantity));

					yield break;
				}

				yield return new WaitForSeconds(BalanceCheckDelaySeconds);
			}

			TryCompleteError(new Error(errorMessage: "Balance verification did not complete."));
		}

		private void RequestCurrencyBalance(Action<int> onSuccess, Action<Error> onError)
		{
			GetVirtualCurrencyBalance(CurrencySku, onSuccess, onError);
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