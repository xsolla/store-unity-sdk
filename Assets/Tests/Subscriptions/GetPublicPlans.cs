using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Subscriptions;

namespace Xsolla.Tests.Subscriptions
{
	public class GetPublicPlans : SubscriptionsTestsBase
	{
		[UnityTest]
		public IEnumerator GetSubscriptionPublicPlans_Default_Success()
		{
			yield return SignOut();

			var isComplete = false;
			XsollaSubscriptions.GetSubscriptionPublicPlans(
				plans =>
				{
					isComplete = true;
					Assert.NotNull(plans);
					Assert.NotNull(plans.items);
					Assert.Greater(plans.items.Length, 0);
					Assert.IsFalse(plans.has_more);
				},
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				}
			);

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionPublicPlans_FilteredByPlanId_Success()
		{
			yield return GetSubscriptionPublicPlans_Filtered(268800, null, 1);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionPublicPlans_FilteredByNonExistentPlanId_Success()
		{
			yield return GetSubscriptionPublicPlans_Filtered(123, null, 0);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionPublicPlans_FilteredByExternalId_Success()
		{
			yield return GetSubscriptionPublicPlans_Filtered(null, "tNuy9WMo", 1);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionPublicPlans_FilteredBNonExistentExternalId_Success()
		{
			yield return GetSubscriptionPublicPlans_Filtered(null, "123", 0);
		}

		private static IEnumerator GetSubscriptionPublicPlans_Filtered(int? planId, string planExternalId, int expectedCount)
		{
			yield return SignOut();

			int[] planIdFilter = null;
			if (planId.HasValue)
			{
				planIdFilter = new[] {
					planId.Value
				};
			}

			string[] planExternalIdFilter = null;
			if (!string.IsNullOrEmpty(planExternalId))
			{
				planExternalIdFilter = new[] {
					planExternalId
				};
			}

			var isComplete = false;
			XsollaSubscriptions.GetSubscriptionPublicPlans(
				plans =>
				{
					isComplete = true;
					Assert.AreEqual(plans.items.Length, expectedCount);
				},
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				},
				planIdFilter,
				planExternalIdFilter
			);

			yield return new WaitUntil(() => isComplete);
		}
	}
}