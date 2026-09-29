using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace Xsolla.Tests.Subscriptions
{
	public class SubscriptionRoutesContractTests
	{
		private string _source;

		[OneTimeSetUp]
		public void LoadSource()
		{
			var path = Path.Combine(Application.dataPath, "Xsolla/Subscriptions/XsollaSubscriptions.cs");
			_source = File.ReadAllText(path);
		}

		[Test]
		public void GetSubscriptions_UsesExpectedRouteAndMethod()
		{
			AssertMethodContract(
				"GetSubscriptions",
				"\"/user/v1/projects/{StoreProjectId}/subscriptions\"",
				"WebRequestHelper.Instance.GetRequest(");
		}

		[Test]
		public void GetSubscriptionDetails_UsesExpectedRouteAndMethod()
		{
			AssertMethodContract(
				"GetSubscriptionDetails",
				"\"/user/v1/projects/{StoreProjectId}/subscriptions/{subscriptionId}\"",
				"WebRequestHelper.Instance.GetRequest(");
		}

		[Test]
		public void CancelSubscription_UsesExpectedRouteAndMethod()
		{
			AssertMethodContract(
				"CancelSubscription",
				"\"/user/v1/projects/{StoreProjectId}/subscriptions/{subscriptionId}/cancel\"",
				"WebRequestHelper.Instance.PutRequest(");
		}

		[Test]
		public void GetSubscriptionRenewalUrl_UsesExpectedRouteAndMethod()
		{
			AssertMethodContract(
				"GetSubscriptionRenewalUrl",
				"\"/user/v1/projects/{StoreProjectId}/subscriptions/{subscriptionId}/renew\"",
				"WebRequestHelper.Instance.PostRequest(");
		}

		[Test]
		public void GetSubscriptionPurchaseUrl_UsesExpectedRouteAndMethod()
		{
			AssertMethodContract(
				"GetSubscriptionPurchaseUrl",
				"\"/user/v1/projects/{StoreProjectId}/subscriptions/buy\"",
				"WebRequestHelper.Instance.PostRequest(");
		}

		[Test]
		public void GetSubscriptionManagementUrl_UsesExpectedRouteAndMethod()
		{
			AssertMethodContract(
				"GetSubscriptionManagementUrl",
				"\"/user/v1/projects/{StoreProjectId}/subscriptions/manage\"",
				"WebRequestHelper.Instance.PostRequest(");
		}

		[Test]
		public void GetUserAccountUrl_UsesExpectedRouteAndMethod()
		{
			AssertMethodContract(
				"GetUserAccountUrl",
				"\"/user/v1/projects/{StoreProjectId}/subscriptions/user_account\"",
				"WebRequestHelper.Instance.GetRequest(");
		}

		private void AssertMethodContract(string methodName, string routeSegment, string expectedRequestCall)
		{
			var methodBody = ExtractMethodBody(methodName);

			StringAssert.Contains(routeSegment, methodBody, $"Route mismatch for {methodName}");
			StringAssert.Contains(expectedRequestCall, methodBody, $"HTTP method mismatch for {methodName}");
		}

		private string ExtractMethodBody(string methodName)
		{
			var methodStart = _source.IndexOf($"public static void {methodName}(", System.StringComparison.Ordinal);
			Assert.GreaterOrEqual(methodStart, 0, $"Method not found: {methodName}");

			var bodyStart = _source.IndexOf('{', methodStart);
			Assert.GreaterOrEqual(bodyStart, 0, $"Method body start not found: {methodName}");

			var depth = 0;
			for (var i = bodyStart; i < _source.Length; i++)
			{
				var ch = _source[i];
				if (ch == '{')
					depth++;
				else if (ch == '}')
					depth--;

				if (depth == 0)
					return _source.Substring(bodyStart, i - bodyStart + 1);
			}

			Assert.Fail($"Method body end not found: {methodName}");
			return null;
		}
	}
}
