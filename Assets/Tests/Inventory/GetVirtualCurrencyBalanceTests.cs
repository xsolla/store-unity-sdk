using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Inventory;

namespace Xsolla.Tests.Inventory
{
	public class GetVirtualCurrencyBalanceTests : InventoryTestsBase
	{
		[UnityTest]
		public IEnumerator GetVirtualCurrencyBalance_Default_Success()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaInventory.GetVirtualCurrencyBalance(balances =>
			{
				isComplete = true;
				Assert.NotNull(balances);
				Assert.NotNull(balances.items);
				Assert.Greater(balances.items.Length, 0);
			}, error =>
			{
				isComplete = true;
				Assert.Fail(error?.errorMessage);
			});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetVirtualCurrencyBalance_Parametrized_Success()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaInventory.GetVirtualCurrencyBalance(
				balances =>
				{
					isComplete = true;
					Assert.NotNull(balances);
				}, error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				},
				"xsolla");

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetVirtualCurrencyBalance_DefaultValues_InvalidToken_Success()
		{
			yield return SetOldAccessToken();
			yield return GetVirtualCurrencyBalance_Default_Success();
		}
	}
}