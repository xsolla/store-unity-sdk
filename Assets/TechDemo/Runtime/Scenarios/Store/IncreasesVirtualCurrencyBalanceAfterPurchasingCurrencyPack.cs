using System;
using System.Collections;
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

			yield return AuthAsDefaultUser();
			if (IsCompleted)
				yield break;

			var previousBalance = 0;
			yield return RunAsync<int>(RequestCurrencyBalance, balance => previousBalance = balance);
			if (IsCompleted)
				yield break;

			yield return RunAsync<OrderStatus>((success, failure) => XsollaCatalog.Purchase(PackSku, success, failure));
			if (IsCompleted)
				yield break;

			var expectedBalance = previousBalance + BalanceIncreaseAmount;
			yield return VerifyWithRetry<int>(
				BalanceCheckAttempts,
				BalanceCheckDelaySeconds,
				RequestCurrencyBalance,
				current => current - previousBalance == BalanceIncreaseAmount
					? null
					: new Error(errorMessage: "Unexpected balance increase amount. Expected: " + expectedBalance + ". Actual: " + current));
		}

		private static void RequestCurrencyBalance(Action<int> onSuccess, Action<Error> onError)
		{
			GetVirtualCurrencyBalance(CurrencySku, onSuccess, onError);
		}
	}
}