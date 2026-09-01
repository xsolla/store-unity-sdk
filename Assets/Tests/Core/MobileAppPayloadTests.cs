using System;
using System.Reflection;
using NUnit.Framework;
using Xsolla.Core;

namespace Xsolla.Tests.Core
{
	public class MobileAppPayloadTests
	{
		private const string GeneratorTypeName = "Xsolla.Core.PurchaseParamsGenerator, Xsolla";
		private const string OrdersTypeName = "Xsolla.Orders.XsollaOrders, Xsolla";

		[TearDown]
		public void TearDown()
		{
			ResetForcedPlatform();
		}

		[Test]
		public void GenerateUser_Android_DefaultInstallSource_AndExistingFields()
		{
			SetForcedPlatform("android");
			var purchaseParams = new PurchaseParams {
				country = "US",
				tracking_id = "tracking-1"
			};

			var user = GenerateUser(purchaseParams);

			Assert.AreEqual("US", GetFieldValue<string>(user, "country"));
			var trackingId = GetFieldValue<object>(user, "tracking_id");
			Assert.AreEqual("tracking-1", GetFieldValue<string>(trackingId, "value"));

			var mobileApp = GetFieldValue<object>(user, "mobile_app");
			Assert.NotNull(mobileApp);
			Assert.AreEqual("android", GetFieldValue<string>(mobileApp, "platform"));
			Assert.AreEqual("unknown", GetFieldValue<string>(mobileApp, "install_source"));
		}

		[Test]
		public void GenerateUser_Ios_CustomInstallSource()
		{
			SetForcedPlatform("ios");
			var purchaseParams = new PurchaseParams {
				install_source = "app_store"
			};

			var user = GenerateUser(purchaseParams);
			var mobileApp = GetFieldValue<object>(user, "mobile_app");

			Assert.NotNull(mobileApp);
			Assert.AreEqual("ios", GetFieldValue<string>(mobileApp, "platform"));
			Assert.AreEqual("app_store", GetFieldValue<string>(mobileApp, "install_source"));
		}

		[Test]
		public void GenerateUser_NonMobile_OmitsMobileApp()
		{
			ResetForcedPlatform();
			var purchaseParams = new PurchaseParams {
				country = "US"
			};

			var user = GenerateUser(purchaseParams);

			Assert.NotNull(user);
			Assert.AreEqual("US", GetFieldValue<string>(user, "country"));
			Assert.IsNull(GetFieldValue<object>(user, "mobile_app"));
		}

		[Test]
		public void GeneratePaymentTokenUser_ContainsMobileAppPayload()
		{
			SetForcedPlatform("android");
			var purchaseParams = new PurchaseParams {
				install_source = "google_play"
			};

			var user = GeneratePaymentTokenUser(purchaseParams);
			var json = ParseUtils.ToJson(user);

			StringAssert.Contains("\"mobile_app\"", json);
			StringAssert.Contains("\"platform\":\"android\"", json);
			StringAssert.Contains("\"install_source\":\"google_play\"", json);
		}

		private static object GenerateUser(PurchaseParams purchaseParams)
		{
			var generatorType = Type.GetType(GeneratorTypeName);
			var method = generatorType.GetMethod("GenerateUser", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

			return method?.Invoke(null, new object[] { purchaseParams });
		}

		private static object GeneratePaymentTokenUser(PurchaseParams purchaseParams)
		{
			var ordersType = Type.GetType(OrdersTypeName);
			var method = ordersType.GetMethod("GeneratePaymentTokenUser", BindingFlags.Static | BindingFlags.NonPublic);
			return method?.Invoke(null, new object[] { purchaseParams });
		}

		private static void SetForcedPlatform(string platform)
		{
			var generatorType = Type.GetType(GeneratorTypeName);
			var setMethod = generatorType.GetMethod("SetForcedMobilePlatformForTests", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
			setMethod?.Invoke(null, new object[] { platform });
		}

		private static void ResetForcedPlatform()
		{
			var generatorType = Type.GetType(GeneratorTypeName);
			var resetMethod = generatorType.GetMethod("ResetForcedMobilePlatformForTests", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
			resetMethod?.Invoke(null, Array.Empty<object>());
		}

		private static T GetFieldValue<T>(object instance, string fieldName)
		{
			if (instance == null)
				return default;

			var field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			return field != null ? (T)field.GetValue(instance) : default;
		}
	}
}
