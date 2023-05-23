using NUnit.Framework;

namespace Xsolla.Tests.Subscriptions
{
	public class SubscriptionsTestsBase : TestBase
	{
		[OneTimeSetUp]
		[OneTimeTearDown]
		public void Clear()
		{
			ClearEnv();
		}
	}
}