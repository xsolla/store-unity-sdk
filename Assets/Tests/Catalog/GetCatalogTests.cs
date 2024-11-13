using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Catalog;

namespace Xsolla.Tests.Catalog
{
	public class GetCatalogTests : CatalogTestsBase
	{
		[UnityTest]
		public IEnumerator GetCatalogFull_Success()
		{
			DeleteSavedToken();

			var isComplete = false;
			XsollaCatalog.GetCatalogFull(
				items => {
					isComplete = true;
					Assert.NotNull(items);
					Assert.NotNull(items.items);
					Assert.Greater(items.items.Length, 0);
					CheckPersonalization(items.items, false);
					CheckPromotion(items.items, false);
				},
				error => {
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetCatalog_Success()
		{
			DeleteSavedToken();

			var isComplete = false;
			XsollaCatalog.GetCatalog(
				items => {
					isComplete = true;
					Assert.NotNull(items);
					Assert.NotNull(items.items);
					Assert.Greater(items.items.Length, 0);
					CheckPersonalization(items.items, false);
					CheckPromotion(items.items, false);
				},
				error => {
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		// [UnityTest]
		// public IEnumerator GetCatalog_Parametrized_Success()
		// {
		// 	DeleteSavedToken();
		//
		// 	var isComplete = false;
		// 	XsollaCatalog.GetCatalog(
		// 		items =>
		// 		{
		// 			isComplete = true;
		// 			Assert.AreEqual(items.items.Length, 10);
		// 		},
		// 		error =>
		// 		{
		// 			isComplete = true;
		// 			Assert.Fail(error?.errorMessage);
		// 		},
		// 		10,
		// 		locale: "en_US",
		// 		country: "US");
		//
		// 	yield return new WaitUntil(() => isComplete);
		// }

		// [UnityTest]
		// public IEnumerator GetCatalog_HasPersonalizedItem_Success()
		// {
		// 	yield return CheckSession();
		//
		// 	var isComplete = false;
		// 	XsollaCatalog.GetCatalog(
		// 		items =>
		// 		{
		// 			isComplete = true;
		// 			CheckPersonalization(items.items, true);
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