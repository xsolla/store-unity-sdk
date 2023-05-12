using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Inventory;

namespace Xsolla.Tests.Inventory
{
	public class ConsumeInventoryItemTests : InventoryTestsBase
	{
		private static ConsumeItem GenerateItemForConsume()
		{
			return new ConsumeItem {
				sku = "lootbox_1",
				quantity = 1
			};
		}

		[UnityTest]
		public IEnumerator ConsumeInventoryItem_DefaultValues_Success()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaInventory.ConsumeInventoryItem(
				GenerateItemForConsume(),
				() => isComplete = true,
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator ConsumeInventoryItem_Parametrized_Success()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaInventory.ConsumeInventoryItem(
				GenerateItemForConsume(),
				() => isComplete = true,
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				},
				"xsolla");

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator ConsumeInventoryItem_DefaultValues_InvalidToken_Success()
		{
			yield return SetOldAccessToken();
			yield return ConsumeInventoryItem_DefaultValues_Success();
		}
	}
}