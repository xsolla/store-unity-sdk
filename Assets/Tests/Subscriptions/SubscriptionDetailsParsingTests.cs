using NUnit.Framework;
using Xsolla.Core;
using Xsolla.Subscriptions;

namespace Xsolla.Tests.Subscriptions
{
	public class SubscriptionDetailsParsingTests
	{
		[Test]
		public void ParseSubscriptionDetails_WithSetupFee_ParsesChargeFields()
		{
			const string json = "{\"charge\":{\"amount\":4.99,\"setup_fee\":0.99,\"currency\":\"USD\"}}";

			var details = ParseUtils.FromJson<SubscriptionDetails>(json);

			Assert.NotNull(details);
			Assert.NotNull(details.charge);
			Assert.AreEqual(4.99f, details.charge.amount);
			Assert.AreEqual("USD", details.charge.currency);
			Assert.AreEqual(0.99f, details.charge.setup_fee);
		}

		[Test]
		public void ParseSubscriptionDetails_WithoutSetupFee_ParsesWithoutErrors()
		{
			const string json = "{\"charge\":{\"amount\":4.99,\"currency\":\"USD\"}}";

			var details = ParseUtils.FromJson<SubscriptionDetails>(json);

			Assert.NotNull(details);
			Assert.NotNull(details.charge);
			Assert.AreEqual(4.99f, details.charge.amount);
			Assert.AreEqual("USD", details.charge.currency);
			Assert.IsNull(details.charge.setup_fee);
		}
	}
}
