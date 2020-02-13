using NUnit.Framework;
using Xsolla.Core;

namespace Tests
{
    public class Test_XsollaSettings
    {
        // user: test123
        // password: 121212
        // const string XSOLLA_MERCHANT_ID_FOR_TESTS = "125664";
        const string XSOLLA_PROJECT_ID_FOR_TESTS = "52285";
        
        [Test]
        public void Test_XsollaSettingsSimplePasses()
        {
			bool condition = XsollaSettings.StoreProjectId == XSOLLA_PROJECT_ID_FOR_TESTS;
			string errorMessage = "For testing XsollaSettings.ProjecID must be = " + XSOLLA_PROJECT_ID_FOR_TESTS;
			Assert.True(condition, errorMessage);
		}
    }
}
