using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Catalog;

namespace Xsolla.Tests.Catalog
{
	public class GetVirtualCurrencyPackagesListTests : CatalogTestsBase
	{
		[UnityTest]
		public IEnumerator GetAllVirtualCurrencyPackagesList_Default_Success()
		{
			DeleteSavedToken();

			var isComplete = false;
			XsollaCatalog.GetVirtualCurrencyPackagesList(
				packages =>
				{
					isComplete = true;
					Assert.NotNull(packages);
					Assert.NotNull(packages.items);
					Assert.Greater(packages.items.Length, 0);
					CheckPersonalization(packages.items, false);
				}, error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}
		
		[UnityTest]
		public IEnumerator GetVirtualCurrencyPackagesList_Default_Success()
		{
			DeleteSavedToken();

			var isComplete = false;
			XsollaCatalog.GetPaginatedVirtualCurrencyPackagesList(
				packages =>
				{
					isComplete = true;
					Assert.NotNull(packages);
					Assert.NotNull(packages.items);
					Assert.Greater(packages.items.Length, 0);
					CheckPersonalization(packages.items, false);
				}, error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetVirtualCurrencyPackagesList_Parametrized_Success()
		{
			DeleteSavedToken();

			var isComplete = false;
			XsollaCatalog.GetPaginatedVirtualCurrencyPackagesList(
				packages =>
				{
					isComplete = true;
					Assert.AreEqual(packages.items.Length, 1);
				}, error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				},
				1,
				locale: "en_US",
				country: "US");

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetVirtualCurrencyPackagesList_Personalized_HasPersonalizedItem()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaCatalog.GetPaginatedVirtualCurrencyPackagesList(
				packages =>
				{
					isComplete = true;
					CheckPersonalization(packages.items, true);
				}, error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}
	}
}