using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Subscriptions;

namespace Xsolla.Tests.Subscriptions
{
	public class GetPlans : SubscriptionsTestsBase
	{
		[UnityTest]
		public IEnumerator GetSubscriptionPlans_Default_Success()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaSubscriptions.GetSubscriptionPlans(
				items =>
				{
					isComplete = true;
					Assert.NotNull(items);
					Assert.NotNull(items.items);
					Assert.Greater(items.items.Length, 0);
				}, error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionPlans_FilteredByPlanId_Success()
		{
			yield return GetSubscriptionPlans_Filtered(268800, null, 1);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionPlans_FilteredByNonExistentPlanId_Success()
		{
			yield return GetSubscriptionPlans_Filtered(123, null, 0);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionPlans_FilteredByExternalId_Success()
		{
			yield return GetSubscriptionPlans_Filtered(null, "tNuy9WMo", 1);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionPlans_FilteredByNonExistentExternalId_Success()
		{
			yield return GetSubscriptionPlans_Filtered(null, "123", 0);
		}

		private static IEnumerator GetSubscriptionPlans_Filtered(int? planId, string planExternalId, int expectedCount)
		{
			yield return CheckSession();

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
			XsollaSubscriptions.GetSubscriptionPlans(
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