using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Subscriptions;

namespace Xsolla.Tests.Subscriptions
{
	public class GetSubscriptionsTests : SubscriptionsTestsBase
	{
		[UnityTest]
		public IEnumerator GetSubscriptions_Default_Success()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaSubscriptions.GetSubscriptions(
				items => {
					isComplete = true;
					Assert.NotNull(items);
					Assert.NotNull(items.items);

					// Assert.Greater(items.items.Length, 0);
				}, error => {
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionDetails_Default_Success()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaSubscriptions.GetSubscriptionDetails(
				65045530,
				details => {
					isComplete = true;
					Assert.NotNull(details);
				}, error => {
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		[Explicit("Destructive test: cancels a real active subscription.")]
		public IEnumerator CancelSubscription_ManualDestructive_SuccessOrSkippedWhenNoActiveSubscriptions()
		{
			yield return CheckSession();

			SubscriptionItem activeSubscription = null;
			yield return GetFirstActiveSubscription(item => activeSubscription = item);

			if (activeSubscription == null)
			{
				Assert.Ignore("No active subscriptions are available for cancel smoke test.");
				yield break;
			}

			var isComplete = false;
			XsollaSubscriptions.CancelSubscription(
				activeSubscription.id,
				() => isComplete = true,
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionRenewalUrl_Smoke_SuccessOrSkippedWhenNoRenewableSubscriptions()
		{
			yield return CheckSession();

			SubscriptionItem renewableSubscription = null;
			yield return GetFirstRenewableSubscription(item => renewableSubscription = item);

			if (renewableSubscription == null)
			{
				Assert.Ignore("No renewable subscriptions are available for renew smoke test.");
				yield break;
			}

			var isComplete = false;
			XsollaSubscriptions.GetSubscriptionRenewalUrl(
				renewableSubscription.id,
				link =>
				{
					isComplete = true;
					Assert.NotNull(link);
					Assert.NotNull(link.link_to_ps);
				},
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionPurchaseUrl_Success()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaSubscriptions.GetSubscriptionPurchaseUrl(
				"tNuy9WMo",
				link => {
					isComplete = true;
					Assert.NotNull(link);
					Assert.NotNull(link.link_to_ps);
				},
				error => {
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				}
				);

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionPurchaseUrlWithCustomParams_Success()
		{
			yield return CheckSession();

			var customParams = new Dictionary<string, object> {
				{ "test_bool", true },
				{ "test_int", 123 },
				{ "test_string", "test" }
			};

			var isComplete = false;
			XsollaSubscriptions.GetSubscriptionPurchaseUrl(
				"tNuy9WMo",
				link => {
					isComplete = true;
					Assert.NotNull(link);
					Assert.NotNull(link.link_to_ps);
				},
				error => {
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				},
				customParameters: customParams);

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetSubscriptionManagementUrl_Success()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaSubscriptions.GetSubscriptionManagementUrl(
				link => {
					isComplete = true;
					Assert.NotNull(link);
					Assert.NotNull(link.link_to_ps);
				}, error => {
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		public IEnumerator GetUserAccountUrl_Success()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaSubscriptions.GetUserAccountUrl(
				link => {
					isComplete = true;
					Assert.NotNull(link);
					Assert.NotNull(link.redirect_url);
				},
				error => {
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}

		[UnityTest]
		[TestCase("en", ExpectedResult = null)]
		[TestCase("ru", ExpectedResult = null)]
		public IEnumerator GetUserAccountUrlWithLocale_Success(string locale)
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaSubscriptions.GetUserAccountUrl(
				link => {
					isComplete = true;
					Assert.NotNull(link);
					Assert.NotNull(link.redirect_url);
					Assert.IsTrue(link.redirect_url.Contains(locale));
				},
				error => {
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				},
				locale);

			yield return new WaitUntil(() => isComplete);
		}

		private static IEnumerator GetFirstActiveSubscription(System.Action<SubscriptionItem> onComplete)
		{
			var isComplete = false;
			SubscriptionItems subscriptions = null;

			XsollaSubscriptions.GetSubscriptions(
				items =>
				{
					subscriptions = items;
					isComplete = true;
				},
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
			onComplete?.Invoke(subscriptions?.items != null
				? System.Array.Find(subscriptions.items, item => item != null && item.status == "active")
				: null);
		}

		private static IEnumerator GetFirstRenewableSubscription(System.Action<SubscriptionItem> onComplete)
		{
			var isListComplete = false;
			SubscriptionItems subscriptions = null;

			XsollaSubscriptions.GetSubscriptions(
				items =>
				{
					subscriptions = items;
					isListComplete = true;
				},
				error =>
				{
					isListComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isListComplete);

			if (subscriptions?.items == null || subscriptions.items.Length == 0)
			{
				onComplete?.Invoke(null);
				yield break;
			}

			SubscriptionItem result = null;
			foreach (var subscription in subscriptions.items)
			{
				if (subscription == null)
					continue;

				var isDetailsComplete = false;
				SubscriptionDetails details = null;

				XsollaSubscriptions.GetSubscriptionDetails(
					subscription.id,
					response =>
					{
						details = response;
						isDetailsComplete = true;
					},
					error =>
					{
						isDetailsComplete = true;
						Assert.Fail(error?.errorMessage);
					});

				yield return new WaitUntil(() => isDetailsComplete);
				if (details?.is_renew_possible == true)
				{
					result = subscription;
					break;
				}
			}

			onComplete?.Invoke(result);
		}
	}
}