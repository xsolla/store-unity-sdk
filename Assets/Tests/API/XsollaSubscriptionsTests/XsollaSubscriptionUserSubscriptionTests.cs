using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Core;
using Xsolla.Subscriptions;

namespace Xsolla.Tests
{
	public class XsollaSubscriptionUserSubscriptionTests : XsollaSubscriptionTestsBase
	{
		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			if (XsollaSubscriptions.IsExist)
				Object.DestroyImmediate(XsollaSubscriptions.Instance.gameObject);
		}
		
		[UnityTest]
		public IEnumerator GetSubscriptions_Default_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaSubscriptions.Instance.GetSubscriptions(
				XsollaSettings.StoreProjectId,
				subscriptions =>
				{
					if (!NotNull(nameof(subscriptions.items), subscriptions.items, ref errorMessage))
					{
						success = false;
						return;
					}

					// if (!AreEqual(nameof(subscriptions.items.Length), 1, subscriptions.items.Length, ref errorMessage))
					// {
					// 	success = false;
					// 	return;
					// }

					if (!AreEqual(nameof(subscriptions.has_more), false, subscriptions.has_more, ref errorMessage))
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
			HandleResult(nameof(GetSubscriptions_Default_Success), success, errorMessage);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionDetails_Default_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaSubscriptions.Instance.GetSubscriptionDetails(
				XsollaSettings.StoreProjectId,
				65045530,
				subscription =>
				{
					if (!AreEqual(nameof(subscription.id), 65045530, subscription.id, ref errorMessage))
					{
						success = false;
						return;
					}

					if (!AreEqual(nameof(subscription.plan_id), 268799, subscription.plan_id, ref errorMessage))
					{
						success = false;
						return;
					}

					if (!AreEqual(nameof(subscription.plan_external_id), "IFIwBTne", subscription.plan_external_id, ref errorMessage))
					{
						success = false;
						return;
					}

					if (!NotNull(nameof(subscription.date_create), subscription.date_create, ref errorMessage))
					{
						success = false;
						return;
					}

					if (!NotNull(nameof(subscription.date_next_charge), subscription.date_next_charge, ref errorMessage))
					{
						success = false;
						return;
					}

					if (!NotNull(nameof(subscription.date_last_charge), subscription.date_last_charge, ref errorMessage))
					{
						success = false;
						return;
					}

					// if (!IsNull(nameof(subscription.date_end), subscription.date_end, ref errorMessage))
					// {
					// 	success = false;
					// 	return;
					// }

					if (!NotNull(nameof(subscription.charge), subscription.charge, ref errorMessage))
					{
						success = false;
						return;
					}

					if (!AreEqual(nameof(subscription.charge.amount), 0.01f, subscription.charge.amount, ref errorMessage))
					{
						success = false;
						return;
					}

					if (!AreEqual(nameof(subscription.charge.currency), "USD", subscription.charge.currency, ref errorMessage))
					{
						success = false;
						return;
					}

					if (!NotNull(nameof(subscription.period), subscription.period, ref errorMessage))
					{
						success = false;
						return;
					}

					if (!AreEqual(nameof(subscription.period.value), 1, subscription.period.value, ref errorMessage))
					{
						success = false;
						return;
					}

					if (!AreEqual(nameof(subscription.period.Unit), PeriodUnit.Month, subscription.period.Unit, ref errorMessage))
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
			HandleResult(nameof(GetSubscriptionDetails_Default_Success), success, errorMessage);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionDetails_NonExistentId_Failure()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaSubscriptions.Instance.GetSubscriptionDetails(
				XsollaSettings.StoreProjectId,
				123456,
				subscription => { success = false; },
				error =>
				{
					if (!NotNull(nameof(error), error, ref errorMessage))
					{
						success = false;
						return;
					}

					if (!NotNull(nameof(error.errorMessage), error.errorMessage, ref errorMessage))
					{
						success = false;
						return;
					}

					success = true;
				}
			);

			yield return new WaitUntil(() => success.HasValue);
			HandleResult(nameof(GetSubscriptionDetails_NonExistentId_Failure), success, errorMessage);
		}

		[UnityTest]
		public IEnumerator CancelSubscription_NonExistentSubscriptionId_Failure()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaSubscriptions.Instance.CancelSubscription(
				XsollaSettings.StoreProjectId,
				12345,
				() => success = false,
				error =>
				{
					if (!NotNull(nameof(error), error, ref errorMessage))
					{
						success = false;
						return;
					}

					if (!NotNull(nameof(error.errorMessage), error.errorMessage, ref errorMessage))
					{
						success = false;
						return;
					}

					success = true;
				}
			);

			yield return new WaitUntil(() => success.HasValue);
			HandleResult(nameof(CancelSubscription_NonExistentSubscriptionId_Failure), success, errorMessage);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionRenewalUrl_NonExistentSubscriptionId_Failure()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaSubscriptions.Instance.GetSubscriptionRenewalUrl(
				XsollaSettings.StoreProjectId,
				12345,
				link => { success = false; },
				error =>
				{
					if (!NotNull(nameof(error), error, ref errorMessage))
					{
						success = false;
						return;
					}

					if (!NotNull(nameof(error.errorMessage), error.errorMessage, ref errorMessage))
					{
						success = false;
						return;
					}

					success = true;
				}
			);

			yield return new WaitUntil(() => success.HasValue);
			HandleResult(nameof(GetSubscriptionRenewalUrl_NonExistentSubscriptionId_Failure), success, errorMessage);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionPurchaseUrl_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaSubscriptions.Instance.GetSubscriptionPurchaseUrl(
				XsollaSettings.StoreProjectId,
				"tNuy9WMo",
				link =>
				{
					if (!NotNull(nameof(link.link_to_ps), link.link_to_ps, ref errorMessage))
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
			HandleResult(nameof(GetSubscriptionPurchaseUrl_Success), success, errorMessage);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionPurchaseUrl_NonExistentPlanExternalId_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaSubscriptions.Instance.GetSubscriptionPurchaseUrl(
				XsollaSettings.StoreProjectId,
				"abc",
				link => { success = false; },
				error =>
				{
					if (!NotNull(nameof(error), error, ref errorMessage))
					{
						success = false;
						return;
					}

					if (!NotNull(nameof(error.errorMessage), error.errorMessage, ref errorMessage))
					{
						success = false;
						return;
					}

					success = true;
				}
			);

			yield return new WaitUntil(() => success.HasValue);
			HandleResult(nameof(GetSubscriptionPurchaseUrl_NonExistentPlanExternalId_Success), success, errorMessage);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionManagementUrl_Success()
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			XsollaSubscriptions.Instance.GetSubscriptionManagementUrl(
				XsollaSettings.StoreProjectId,
				link =>
				{
					if (!NotNull(nameof(link.link_to_ps), link.link_to_ps, ref errorMessage))
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
			HandleResult(nameof(GetSubscriptionManagementUrl_Success), success, errorMessage);
		}
	}
}