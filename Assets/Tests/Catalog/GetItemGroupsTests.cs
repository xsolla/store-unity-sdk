using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Catalog;

namespace Xsolla.Tests.Catalog
{
	public class GetItemGroupsTests : CatalogTestsBase
	{
		[UnityTest]
		public IEnumerator GetItemGroups_Default_Success()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaCatalog.GetItemGroups(
				groups =>
				{
					isComplete = true;
					Assert.NotNull(groups);
					Assert.NotNull(groups.groups);
					Assert.Greater(groups.groups.Length, 0);
				},
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetItemGroups_Parametrized_Success()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaCatalog.GetItemGroups(
				groups =>
				{
					isComplete = true;
					Assert.Greater(groups.groups.Length, 0);
				},
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				},
				locale: "en_US");

			yield return new WaitUntil(() => isComplete);
		}
	}
}