using NUnit.Framework;

namespace Xsolla.Tests.Inventory
{
	public class InventoryTestsBase : TestBase
	{
		[OneTimeSetUp]
		[OneTimeTearDown]
		public void Clear()
		{
			ClearEnv();
		}
	}
}