using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Catalog;

namespace Xsolla.Tests.Catalog
{
	public class RedeemCouponCodeTests : CatalogTestsBase
	{
		// [UnityTest]
		public IEnumerator RedeemCouponCode_Success()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaCatalog.RedeemCouponCode(
				"WINTER2021",
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
		public IEnumerator RedeemCouponCode_IncorrectCode_Failure()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaCatalog.RedeemCouponCode(
				"SUMMER1967",
				items =>
				{
					isComplete = true;
					Assert.Fail("Coupon code is incorrect");
				},
				error =>
				{
					isComplete = true;
					Assert.NotNull(error);
					Assert.NotNull(error.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		// [UnityTest]
		public IEnumerator RedeemCouponCode_InvalidToken_Success()
		{
			yield return SetOldAccessToken();
			yield return RedeemCouponCode_Success();
		}
	}
}