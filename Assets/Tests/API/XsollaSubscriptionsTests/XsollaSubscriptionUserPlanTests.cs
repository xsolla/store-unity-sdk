using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Core;
using Xsolla.Subscriptions;

namespace Xsolla.Tests
{
	public class XsollaSubscriptionUserPlanTests : XsollaSubscriptionTestsBase
	{
		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			if (XsollaSubscriptions.IsExist)
				Object.DestroyImmediate(XsollaSubscriptions.Instance.gameObject);
		}
		
		[UnityTest]
		public IEnumerator GetSubscriptionPlans_Default_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaSubscriptions.Instance.GetSubscriptionPlans(
				XsollaSettings.StoreProjectId,
				plans =>
				{
					if (!NotNull(nameof(plans.items), plans.items, ref errorMessage))
					{
						success = false;
						return;
					}
					
					if (!AreEqual(nameof(plans.items.Length), 2, plans.items.Length, ref errorMessage))
					{
						success = false;
						return;
					}

					if (!AreEqual(nameof(plans.has_more), false, plans.has_more, ref errorMessage))
					{
						success = false;
						return;
					}

					success = true;
				},
				error =>
				{
					errorMessage = error?.errorMessage ?? "ERROR IS NULL";
					success = false;
				}
			);

			yield return new WaitUntil(() => success.HasValue);
			HandleResult(nameof(GetSubscriptionPlans_Default_Success), success, errorMessage);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionPlans_FilteredByPlanId_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaSubscriptions.Instance.GetSubscriptionPlans(
				TEST_PROJECT_ID,
				plans =>
				{
					if (!NotNull(nameof(plans.items), plans.items, ref errorMessage))
					{
						success = false;
						return;
					}
					
					if (!AreEqual(nameof(plans.items.Length), 1, plans.items.Length, ref errorMessage))
					{
						success = false;
						return;
					}

					success = true;
				},
				error =>
				{
					errorMessage = error?.errorMessage ?? "ERROR IS NULL";
					success = false;
				},
				planId: new[]{
					269503
				}
			);

			yield return new WaitUntil(() => success.HasValue);
			HandleResult(nameof(GetSubscriptionPlans_FilteredByPlanId_Success), success, errorMessage);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionPlans_FilteredByNonExistentPlanId_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaSubscriptions.Instance.GetSubscriptionPlans(
				TEST_PROJECT_ID,
				plans =>
				{
					if (!NotNull(nameof(plans.items), plans.items, ref errorMessage))
					{
						success = false;
						return;
					}
					
					if (!AreEqual(nameof(plans.items.Length), 0, plans.items.Length, ref errorMessage))
					{
						success = false;
						return;
					}

					success = true;
				},
				error =>
				{
					errorMessage = error?.errorMessage ?? "ERROR IS NULL";
					success = false;
				},
				planId: new[]{
					123
				}
			);

			yield return new WaitUntil(() => success.HasValue);
			HandleResult(nameof(GetSubscriptionPlans_FilteredByNonExistentPlanId_Success), success, errorMessage);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionPlans_FilteredByExternalId_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaSubscriptions.Instance.GetSubscriptionPlans(
				TEST_PROJECT_ID,
				plans =>
				{
					if (!NotNull(nameof(plans.items), plans.items, ref errorMessage))
					{
						success = false;
						return;
					}
					
					if (!AreEqual(nameof(plans.items.Length), 1, plans.items.Length, ref errorMessage))
					{
						success = false;
						return;
					}

					success = true;
				},
				error =>
				{
					errorMessage = error?.errorMessage ?? "ERROR IS NULL";
					success = false;
				},
				planExternalId: new[]{
					"Zt0C7jHW"
				}
			);

			yield return new WaitUntil(() => success.HasValue);
			HandleResult(nameof(GetSubscriptionPlans_FilteredByExternalId_Success), success, errorMessage);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionPlans_MultipleExternalIds_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaSubscriptions.Instance.GetSubscriptionPlans(
				"77640",
				plans =>
				{
					if (!NotNull(nameof(plans.items), plans.items, ref errorMessage))
					{
						success = false;
						return;
					}
					
					if (!AreEqual(nameof(plans.items.Length), 2, plans.items.Length, ref errorMessage))
					{
						success = false;
						return;
					}

					success = true;
				},
				error =>
				{
					errorMessage = error?.errorMessage ?? "ERROR IS NULL";
					success = false;
				},
				planExternalId: new[]{
					"tNuy9WMo","IFIwBTne"
				}
			);

			yield return new WaitUntil(() => success.HasValue);
			HandleResult(nameof(GetSubscriptionPlans_FilteredByExternalId_Success), success, errorMessage);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionPlans_FilteredBNonExistentExternalId_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaSubscriptions.Instance.GetSubscriptionPlans(
				TEST_PROJECT_ID,
				plans =>
				{
					if (!NotNull(nameof(plans.items), plans.items, ref errorMessage))
					{
						success = false;
						return;
					}
					
					if (!AreEqual(nameof(plans.items.Length), 0, plans.items.Length, ref errorMessage))
					{
						success = false;
						return;
					}

					success = true;
				},
				error =>
				{
					errorMessage = error?.errorMessage ?? "ERROR IS NULL";
					success = false;
				},
				planExternalId: new[]{
					"123"
				}
			);

			yield return new WaitUntil(() => success.HasValue);
			HandleResult(nameof(GetSubscriptionPlans_FilteredBNonExistentExternalId_Success), success, errorMessage);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionPlans_Pagination_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaSubscriptions.Instance.GetSubscriptionPlans(
				TEST_PROJECT_ID,
				plans =>
				{
					if (!NotNull(nameof(plans.items), plans.items, ref errorMessage))
					{
						success = false;
						return;
					}
					
					if (!AreEqual(nameof(plans.items.Length), 1, plans.items.Length, ref errorMessage))
					{
						success = false;
						return;
					}

					if (!AreEqual(nameof(plans.has_more), true, plans.has_more, ref errorMessage))
					{
						success = false;
						return;
					}

					success = true;
				},
				error =>
				{
					errorMessage = error?.errorMessage ?? "ERROR IS NULL";
					success = false;
				},
				limit: 1
			);

			yield return new WaitUntil(() => success.HasValue);
			HandleResult(nameof(GetSubscriptionPlans_Pagination_Success), success, errorMessage);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionPlans_CheckFields_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaSubscriptions.Instance.GetSubscriptionPlans(
				TEST_PROJECT_ID,
				plans =>
				{
					var plan = plans.items[0];

					if (!AreEqual(nameof(plan.plan_id), 269503, plan.plan_id, ref errorMessage))
					{
						success = false;
						return;
					}

					if (!AreEqual(nameof(plan.plan_external_id), "WT4xCan8", plan.plan_external_id, ref errorMessage))
					{
						success = false;
						return;
					}

					if (!AreEqual(nameof(plan.PlanType), PlanType.All, plan.PlanType, ref errorMessage))
					{
						success = false;
						return;
					}

					if (!AreEqual(nameof(plan.period.value), 1, plan.period.value, ref errorMessage))
					{
						success = false;
						return;
					}

					if (!AreEqual(nameof(plan.period.Unit), PeriodUnit.Month, plan.period.Unit, ref errorMessage))
					{
						success = false;
						return;
					}

					if (!AreEqual(nameof(plan.charge.amount), 0.49f, plan.charge.amount, ref errorMessage))
					{
						success = false;
						return;
					}

					if (!AreEqual(nameof(plan.charge.currency), "USD", plan.charge.currency, ref errorMessage))
					{
						success = false;
						return;
					}

					success = true;
				},
				error =>
				{
					errorMessage = error?.errorMessage ?? "ERROR IS NULL";
					success = false;
				},
				planId: new[]{
					269503
				}
			);

			yield return new WaitUntil(() => success.HasValue);
			HandleResult(nameof(GetSubscriptionPlans_CheckFields_Success), success, errorMessage);
		}
	}
}