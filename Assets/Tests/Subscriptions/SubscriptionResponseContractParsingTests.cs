using NUnit.Framework;
using Xsolla.Core;
using Xsolla.Subscriptions;

namespace Xsolla.Tests.Subscriptions
{
	public class SubscriptionResponseContractParsingTests
	{
		[Test]
		public void ParseSubscriptionItems_WithNullAndMissingOptionalFields_ParsesSuccessfully()
		{
			const string json = "{\"items\":[{\"id\":12345,\"plan_id\":54321,\"plan_external_id\":\"PlanExternalId\",\"plan_name\":\"Name\",\"plan_description\":\"Description\",\"status\":\"active\",\"is_in_trial\":false,\"date_create\":\"2031-04-11T13:51:02+03:00\",\"charge\":{\"amount\":4.99,\"currency\":\"USD\"},\"period\":{}}],\"has_more\":false}";

			var result = ParseUtils.FromJson<SubscriptionItems>(json);

			Assert.NotNull(result);
			Assert.NotNull(result.items);
			Assert.AreEqual(1, result.items.Length);
			Assert.AreEqual(12345, result.items[0].id);
			Assert.IsNull(result.items[0].product_id);
			Assert.IsNull(result.items[0].product_external_id);
			Assert.IsNull(result.items[0].product_name);
			Assert.IsNull(result.items[0].product_description);
			Assert.NotNull(result.items[0].charge);
			Assert.IsNull(result.items[0].charge.amount_with_promotion);
			Assert.NotNull(result.items[0].period);
			Assert.AreEqual(0, result.items[0].period.value);
			Assert.IsNull(result.items[0].period.unit);
		}

		[Test]
		public void ParseSubscriptionDetails_WithMissingOptionalFields_ParsesSuccessfully()
		{
			const string json = "{\"id\":12345,\"plan_id\":54321,\"plan_external_id\":\"PlanExternalId\",\"plan_name\":\"Name\",\"plan_description\":\"Description\",\"status\":\"active\",\"is_in_trial\":false,\"date_create\":\"2031-04-11T13:51:02+03:00\",\"charge\":{\"amount\":4.99,\"currency\":\"USD\"},\"period\":{},\"is_renew_possible\":true,\"is_change_to_non_renew_possible\":true,\"is_change_plan_allowed\":false}";

			var result = ParseUtils.FromJson<SubscriptionDetails>(json);

			Assert.NotNull(result);
			Assert.IsNull(result.product_id);
			Assert.IsNull(result.product_external_id);
			Assert.IsNull(result.product_name);
			Assert.IsNull(result.product_description);
			Assert.IsNull(result.date_end);
			Assert.NotNull(result.charge);
			Assert.IsNull(result.charge.setup_fee);
			Assert.NotNull(result.period);
			Assert.AreEqual(0, result.period.value);
			Assert.IsNull(result.period.unit);
			Assert.IsTrue(result.is_renew_possible);
			Assert.IsTrue(result.is_change_to_non_renew_possible);
			Assert.IsFalse(result.is_change_plan_allowed);
		}

		[Test]
		public void ParsePaymentLink_WithRequiredField_ParsesLink()
		{
			const string json = "{\"link_to_ps\":\"https://secure.xsolla.com/paystation2/?access_token=test\"}";

			var link = ParseUtils.FromJson<PaymentLink>(json);

			Assert.NotNull(link);
			Assert.AreEqual("https://secure.xsolla.com/paystation2/?access_token=test", link.link_to_ps);
		}

		[Test]
		public void ParsePaymentLink_WithMissingField_ReturnsObjectWithNullLink()
		{
			const string json = "{}";

			var link = ParseUtils.FromJson<PaymentLink>(json);

			Assert.NotNull(link);
			Assert.IsNull(link.link_to_ps);
		}

		[Test]
		public void ParsePaymentLink_WithNullField_ReturnsNullLink()
		{
			const string json = "{\"link_to_ps\":null}";

			var link = ParseUtils.FromJson<PaymentLink>(json);

			Assert.NotNull(link);
			Assert.IsNull(link.link_to_ps);
		}

		[Test]
		public void ParseUserAccountLink_WithRequiredField_ParsesRedirectUrl()
		{
			const string json = "{\"redirect_url\":\"https://subscriptions-user-account.xsolla.com/?token=test\"}";

			var link = ParseUtils.FromJson<UserAccountLink>(json);

			Assert.NotNull(link);
			Assert.AreEqual("https://subscriptions-user-account.xsolla.com/?token=test", link.redirect_url);
		}

		[Test]
		public void ParseUserAccountLink_WithMissingField_ReturnsObjectWithNullRedirectUrl()
		{
			const string json = "{}";

			var link = ParseUtils.FromJson<UserAccountLink>(json);

			Assert.NotNull(link);
			Assert.IsNull(link.redirect_url);
		}

		[Test]
		public void ParseUserAccountLink_WithNullField_ReturnsNullRedirectUrl()
		{
			const string json = "{\"redirect_url\":null}";

			var link = ParseUtils.FromJson<UserAccountLink>(json);

			Assert.NotNull(link);
			Assert.IsNull(link.redirect_url);
		}

		[Test]
		public void ParseCancelResponse_WithNullBody_ReturnsNullObject()
		{
			const string json = "null";

			var body = ParseUtils.FromJson<object>(json);

			Assert.IsNull(body);
		}
	}
}
