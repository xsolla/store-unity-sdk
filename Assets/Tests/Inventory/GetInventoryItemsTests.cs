using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Inventory;

namespace Xsolla.Tests.Inventory
{
	public class GetInventoryItemsTests : InventoryTestsBase
	{
		[UnityTest]
		public IEnumerator GetInventoryItems_Default_Success()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaInventory.GetInventoryItems(
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
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetInventoryItems_Parametrized_Success()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaInventory.GetInventoryItems(
				items =>
				{
					isComplete = true;
					Assert.AreEqual(items.items.Length, 5);
				},
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				},
				5,
				0,
				"en_US",
				"xsolla");

			yield return new WaitUntil(() => isComplete);
		}

		// [UnityTest]
		// public IEnumerator GetInventoryItems_DefaultValues_InvalidToken_Success()
		// {
		// 	yield return SetOldAccessToken();
		// 	yield return GetInventoryItems_Default_Success();
		// }
	}
}