using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Catalog;

namespace Xsolla.Tests.Catalog
{
	public class CreateOrderWithSpecifiedFreeItemTests : CatalogTestsBase
	{
		[UnityTest]
		public IEnumerator CreateOrderWithSpecifiedFreeItem_Success()
		{
			yield return SignInAsTestUser();

			var isComplete = false;
			XsollaCatalog.CreateOrderWithFreeItem(
				"Xsolla_free_item",
				orderId =>
				{
					isComplete = true;
					Assert.NotNull(orderId);
					Assert.Greater(orderId.order_id, 0);
				},
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator CreateOrderWithSpecifiedFreeItem_NonExistentSku_Failure()
		{
			yield return SignInAsTestUser();

			var isComplete = false;
			XsollaCatalog.CreateOrderWithFreeItem(
				"abcdef",
				orderId =>
				{
					isComplete = true;
					Assert.Fail("Order created with invalid sku");
				},
				error =>
				{
					isComplete = true;
					Assert.NotNull(error);
					Assert.NotNull(error.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator CreateOrderWithSpecifiedFreeItem_InvalidUser_Failure()
		{
			yield return SignIn();

			var isComplete = false;
			XsollaCatalog.CreateOrderWithFreeItem(
				"Xsolla_free_item",
				orderId =>
				{
					isComplete = true;
					Assert.Fail("Order created with invalid user");
				},
				error =>
				{
					isComplete = true;
					Assert.NotNull(error);
					Assert.NotNull(error.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}
	}
}