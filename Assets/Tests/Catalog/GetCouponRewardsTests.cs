using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Catalog;

namespace Xsolla.Tests.Catalog
{
	public class GetCouponRewardsTests : CatalogTestsBase
	{
		[UnityTest]
		public IEnumerator GetCouponRewards_Success()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaCatalog.GetCouponRewards(
				"WINTER2021",
				reward =>
				{
					isComplete = true;
					Assert.NotNull(reward);
					Assert.Greater(reward.bonus.Length, 0);
				},
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetCouponRewards_IncorrectCode_Failure()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaCatalog.GetCouponRewards(
				"SUMMER1967",
				reward =>
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

		[UnityTest]
		public IEnumerator GetCouponRewards_InvalidToken_Success()
		{
			yield return SetOldAccessToken();
			yield return GetCouponRewards_Success();
		}
	}
}