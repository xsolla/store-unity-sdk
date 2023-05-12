using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Catalog;

namespace Xsolla.Tests.Catalog
{
	public class GetVirtualCurrencyListTests : CatalogTestsBase
	{
		[UnityTest]
		public IEnumerator GetVirtualCurrencyList_Default_Success()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaCatalog.GetVirtualCurrencyList(
				items =>
				{
					isComplete = true;
					Assert.NotNull(items);
					Assert.NotNull(items.items);
					Assert.Greater(items.items.Length, 0);
				},
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetVirtualCurrencyList_Parametrized_Success()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaCatalog.GetVirtualCurrencyList(
				items =>
				{
					isComplete = true;
					Assert.AreEqual(items.items.Length, 1);
				},
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				},
				1,
				1,
				"en_US",
				"US",
				"long_description");

			yield return new WaitUntil(() => isComplete);
		}
	}
}