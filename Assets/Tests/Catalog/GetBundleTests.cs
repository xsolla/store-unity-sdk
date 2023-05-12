using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Catalog;

namespace Xsolla.Tests.Catalog
{
	public class GetBundleTests : CatalogTestsBase
	{
		[UnityTest]
		public IEnumerator GetBundle_Default_Success()
		{
			yield return CheckSession();

			var isComplete = false;

			XsollaCatalog.GetBundle(
				"starter_pack",
				item =>
				{
					isComplete = true;
					Assert.NotNull(item);
				},
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetBundle_Parametrized_Success()
		{
			yield return CheckSession();

			var isComplete = false;

			XsollaCatalog.GetBundle(
				"starter_pack",
				item =>
				{
					isComplete = true;
					Assert.NotNull(item);
				},
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				},
				"de_DE",
				"DE");

			yield return new WaitUntil(() => isComplete);
		}
	}
}