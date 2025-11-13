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
			XsollaCatalog.GetItems(
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
			XsollaCatalog.GetItems(
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
	}
}