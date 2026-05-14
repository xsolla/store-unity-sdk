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

			var previousBalanceReceived = false;
			var previousBalance = 0;
			Error previousBalanceError = null;

			RequestCurrencyBalance(
				balance => {
					previousBalance = balance;
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

			yield return VerifyBalanceIncreaseWithRetry(previousBalance);
		}

		private IEnumerator VerifyBalanceIncreaseWithRetry(int previousBalance)
		{
			var expectedBalance = previousBalance + BalanceIncreaseAmount;

			for (var attempt = 0; attempt < BalanceCheckAttempts; attempt++)
			{
				var balanceReceived = false;
				var currentBalance = 0;
				Error balanceError = null;

				RequestCurrencyBalance(
					balance => {
						currentBalance = balance;
						balanceReceived = true;
					},
					error => {
						balanceError = error;
						balanceReceived = true;
					});

				yield return new WaitUntil(() => balanceReceived);

				if (IsCompleted)
					yield break;

				if (balanceError == null && currentBalance - previousBalance == BalanceIncreaseAmount)
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
						TryCompleteError(new Error(errorMessage: "Unexpected balance increase amount. Expected: " + expectedBalance + ". Actual: " + currentBalance));

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
	}
}