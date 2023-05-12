using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Inventory;

namespace Xsolla.Tests.Inventory
{
	public class GetTimeLimitedItemsTests : InventoryTestsBase
	{
		[UnityTest]
		public IEnumerator GetTimeLimitedItems_DefaultValues_Success()
		{
			yield return GetTimeLimitedItems();
		}

		[UnityTest]
		public IEnumerator GetTimeLimitedItems_Parametrized_Success()
		{
			yield return GetTimeLimitedItems("xsolla");
		}

		[UnityTest]
		public IEnumerator GetTimeLimitedItems_DefaultValues_InvalidToken_Success()
		{
			yield return SetOldAccessToken();
			yield return GetTimeLimitedItems();
		}

		private static IEnumerator GetTimeLimitedItems(string platform = null)
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaInventory.GetTimeLimitedItems(
				items =>
				{
					isComplete = true;
					Assert.NotNull(items);
					Assert.NotNull(items.items);
					Assert.Greater(items.items.Length, 0);
				}, error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				},
				platform);

			yield return new WaitUntil(() => isComplete);
		}
	}
}