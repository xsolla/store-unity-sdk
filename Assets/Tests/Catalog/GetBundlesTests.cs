using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Catalog;

namespace Xsolla.Tests.Catalog
{
	public class GetBundlesTests : CatalogTestsBase
	{
		[UnityTest]
		public IEnumerator GetAllBundles_Default_Success()
		{
			DeleteSavedToken();

			var isComplete = false;
			XsollaCatalog.GetBundles(items => {
				isComplete = true;
				Assert.NotNull(items);
				Assert.NotNull(items.items);
				Assert.Greater(items.items.Length, 0);
				CheckPersonalization(items.items, false);
			}, error => {
				isComplete = true;
				Assert.Fail(error?.errorMessage);
			});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetBundles_Default_Success()
		{
			DeleteSavedToken();

			var isComplete = false;
			XsollaCatalog.GetPaginatedBundles(items => {
				isComplete = true;
				Assert.NotNull(items);
				Assert.NotNull(items.items);
				Assert.Greater(items.items.Length, 0);
				CheckPersonalization(items.items, false);
			}, error => {
				isComplete = true;
				Assert.Fail(error?.errorMessage);
			});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetBundles_Parametrized_Success()
		{
			DeleteSavedToken();

			var isComplete = false;
			XsollaCatalog.GetPaginatedBundles(
				items => {
					isComplete = true;
					Assert.AreEqual(items.items.Length, 1);
				}, error => {
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				},
				locale: "de_DE",
				limit: 1,
				offset: 1);

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetBundles_Personalized_HasPersonalizedItem()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaCatalog.GetPaginatedBundles(
				items => {
					isComplete = true;
					CheckPersonalization(items.items, true);
				}, error => {
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}
	}
}