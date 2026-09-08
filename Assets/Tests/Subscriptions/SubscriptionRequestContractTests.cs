using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Xsolla.Core;
using Xsolla.Subscriptions;

namespace Xsolla.Tests.Subscriptions
{
	public class SubscriptionRequestContractTests
	{
		private const string BuyRequestTypeName = "Xsolla.Subscriptions.BuySubscriptionRequest, Xsolla";
		private const string ManageRequestTypeName = "Xsolla.Subscriptions.ManageSubscriptionRequest, Xsolla";
		private const string RenewRequestTypeName = "Xsolla.Subscriptions.RenewalSubscriptionRequest, Xsolla";

		[Test]
		public void BuyRequest_WithOptionalFields_SerializesExpectedFields()
		{
			var request = CreateInternalRequest(BuyRequestTypeName);
			SetField(request, "plan_external_id", "PlanExternalId");
			SetField(request, "product_id", 123);
			SetField(request, "available_plans", new[] { "plan_a", "plan_b" });
			SetField(request, "custom_parameters", new Dictionary<string, object> { { "flag", true }, { "count", 7 } });
			SetField(request, "settings", new PaymentSettings { sandbox = true });

			var json = ParseUtils.ToJson(request);

			StringAssert.Contains("\"plan_external_id\":\"PlanExternalId\"", json);
			StringAssert.Contains("\"product_id\":123", json);
			StringAssert.Contains("\"available_plans\":[\"plan_a\",\"plan_b\"]", json);
			StringAssert.Contains("\"custom_parameters\":{\"flag\":true,\"count\":7}", json);
			StringAssert.Contains("\"settings\"", json);
			StringAssert.DoesNotContain("steam_user_id", json);
		}

		[Test]
		public void BuyRequest_WithNullOptionals_OmitsOptionalFields()
		{
			var request = CreateInternalRequest(BuyRequestTypeName);
			SetField(request, "plan_external_id", "PlanExternalId");

			var json = ParseUtils.ToJson(request);

			StringAssert.Contains("\"plan_external_id\":\"PlanExternalId\"", json);
			StringAssert.DoesNotContain("product_id", json);
			StringAssert.DoesNotContain("available_plans", json);
			StringAssert.DoesNotContain("custom_parameters", json);
			StringAssert.DoesNotContain("steam_user_id", json);
		}

		[Test]
		public void ManageRequest_WithOptionalFields_SerializesExpectedFields()
		{
			var request = CreateInternalRequest(ManageRequestTypeName);
			SetField(request, "product_id", 456);
			SetField(request, "available_plans", new[] { "plan_x", "plan_y" });
			SetField(request, "custom_parameters", new Dictionary<string, object> { { "region", "EU" } });
			SetField(request, "steam_user_id", "steam_123");
			SetField(request, "settings", new PaymentSettings { sandbox = false });

			var json = ParseUtils.ToJson(request);

			StringAssert.Contains("\"product_id\":456", json);
			StringAssert.Contains("\"available_plans\":[\"plan_x\",\"plan_y\"]", json);
			StringAssert.Contains("\"custom_parameters\":{\"region\":\"EU\"}", json);
			StringAssert.Contains("\"steam_user_id\":\"steam_123\"", json);
			StringAssert.Contains("\"settings\"", json);
		}

		[Test]
		public void ManageRequest_WithNullOptionals_OmitsOptionalFields()
		{
			var request = CreateInternalRequest(ManageRequestTypeName);

			var json = ParseUtils.ToJson(request);

			StringAssert.DoesNotContain("product_id", json);
			StringAssert.DoesNotContain("available_plans", json);
			StringAssert.DoesNotContain("custom_parameters", json);
			StringAssert.DoesNotContain("steam_user_id", json);
		}

		[Test]
		public void RenewRequest_WithOptionalFields_SerializesExpectedFields()
		{
			var request = CreateInternalRequest(RenewRequestTypeName);
			SetField(request, "custom_parameters", new Dictionary<string, object> { { "device", "pc" } });
			SetField(request, "steam_user_id", "steam_789");
			SetField(request, "settings", new PaymentSettings { sandbox = true });

			var json = ParseUtils.ToJson(request);

			StringAssert.Contains("\"custom_parameters\":{\"device\":\"pc\"}", json);
			StringAssert.Contains("\"steam_user_id\":\"steam_789\"", json);
			StringAssert.Contains("\"settings\"", json);
		}

		[Test]
		public void RenewRequest_WithNullOptionals_OmitsOptionalFields()
		{
			var request = CreateInternalRequest(RenewRequestTypeName);

			var json = ParseUtils.ToJson(request);

			StringAssert.DoesNotContain("custom_parameters", json);
			StringAssert.DoesNotContain("steam_user_id", json);
		}

		private static object CreateInternalRequest(string typeName)
		{
			var type = Type.GetType(typeName);
			Assert.NotNull(type, $"Unable to load type: {typeName}");
			return Activator.CreateInstance(type, true);
		}

		private static void SetField(object instance, string fieldName, object value)
		{
			var field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			Assert.NotNull(field, $"Field not found: {fieldName}");
			field.SetValue(instance, value);
		}
	}
}
