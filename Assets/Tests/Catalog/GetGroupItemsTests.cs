using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Catalog;

namespace Xsolla.Tests.Catalog
{
	public class GetGroupItemsTests : CatalogTestsBase
	{
		[UnityTest]
		public IEnumerator GetAllGroupItems_Default_Success()
		{
			DeleteSavedToken();

			var isComplete = false;
			XsollaCatalog.GetGroupItems(
				"Featured",
				items => {
					isComplete = true;
					Assert.NotNull(items);
					Assert.NotNull(items.items);
					Assert.Greater(items.items.Length, 0);
					CheckPromotion(items.items, false);
				},
				error => {
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetGroupItems_Default_Success()
		{
			DeleteSavedToken();

			var isComplete = false;
			XsollaCatalog.GetPaginatedGroupItems(
				"Featured",
				items => {
					isComplete = true;
					Assert.NotNull(items);
					Assert.NotNull(items.items);
					Assert.Greater(items.items.Length, 0);
					CheckPromotion(items.items, false);
				},
				error => {
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetGroupItems_Parametrized_Success()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaCatalog.GetPaginatedGroupItems(
				"Featured",
				items => {
					isComplete = true;
					Assert.AreEqual(items.items.Length, 10);
				},
				error => {
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				},
				locale: "en_US",
				country: "US",
				limit: 10);

			yield return new WaitUntil(() => isComplete);
		}

		// [UnityTest]
		// public IEnumerator GetGroupItems_HasPromotionItem()
		// {
		// 	yield return CheckSession();
		//
		// 	var isComplete = false;
		// 	XsollaCatalog.GetGroupItems(
		// 		"Featured",
		// 		items =>
		// 		{
		// 			isComplete = true;
		// 			CheckPromotion(items.items, true);
		// 		},
		// 		error =>
		// 		{
		// 			isComplete = true;
		// 			Assert.Fail(error?.errorMessage);
		// 		});
		//
		// 	yield return new WaitUntil(() => isComplete);
		// }
	}
}