using NUnit.Framework;

namespace Xsolla.Tests.Auth
{
	public class AuthTestsBase : TestBase
	{
		[OneTimeSetUp]
		[OneTimeTearDown]
		public void Clear()
		{
			ClearEnv();
		}
	}
}