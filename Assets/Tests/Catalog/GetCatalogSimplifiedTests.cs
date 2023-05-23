using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Catalog;

namespace Xsolla.Tests.Catalog
{
	public class GetCatalogSimplifiedTests : CatalogTestsBase
	{
		[UnityTest]
		public IEnumerator GetCatalogSimplified_Default_Success()
		{
			DeleteSavedToken();

			var isComplete = false;
			XsollaCatalog.GetCatalogSimplified(
				items =>
				{
					isComplete = true;
					Assert.NotNull(items);
					Assert.NotNull(items.items);
					Assert.Greater(items.items.Length, 0);
					CheckPersonalization(items.items, false);
				}, error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetCatalogSimplified_Parametrized_Success()
		{
			DeleteSavedToken();

			var isComplete = false;
			XsollaCatalog.GetCatalogSimplified(
				items =>
				{
					isComplete = true;
					Assert.Greater(items.items.Length, 0);
				},
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				},
				"de_DE");

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetCatalogSimplified_HasPersonalizedItem()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaCatalog.GetCatalogSimplified(
				items =>
				{
					isComplete = true;
					CheckPersonalization(items.items, true);
				}, error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}
	}
}