using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Core;

namespace Tests
{
    public class Test_XsollaSettings
    {
		const string XSOLLA_PROJECT_ID_FOR_TESTS = "49999";
        
        [Test]
        public void Test_XsollaSettingsSimplePasses()
        {
			bool condition = XsollaSettings.StoreProjectId == XSOLLA_PROJECT_ID_FOR_TESTS;
			string errorMessage = "For testing XsollaSettings.ProjecID must be = " + XSOLLA_PROJECT_ID_FOR_TESTS;
			Assert.True(condition, errorMessage);
		}
    }
}
