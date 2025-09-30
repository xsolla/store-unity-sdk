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
	}
}