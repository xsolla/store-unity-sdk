using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Tests.WebRequests
{
	public class HeaderAnalyticsParametersTests : WebRequestTestBase
	{
		[Test]
		public void Headers_ShouldContain_Engine_Param()
		{
			var headers = GetWebRequestHeadersCollection();
			var value = headers.FirstOrDefault(x => x.Name == "X-ENGINE")?.Value;

			Assert.IsNotNull(value, "Engine parameter should not be null");
			Assert.AreEqual("UNITY", value, "Engine parameter should be 'UNITY'");
		}

		[Test]
		public void Headers_ShouldContain_EngineVersion_Param()
		{
			var headers = GetWebRequestHeadersCollection();
			var value = headers.FirstOrDefault(x => x.Name == "X-ENGINE-V")?.Value;

			Assert.IsNotNull(value, "Engine version parameter should not be null");
			Assert.AreEqual(Application.unityVersion.ToUpperInvariant(), value, "Engine version parameter should match Unity version");
		}

		[Test]
		[TestCase(SdkType.Login)]
		[TestCase(SdkType.Store)]
		[TestCase(SdkType.Subscriptions)]
		public void Headers_ShouldContain_SdkType_Param(SdkType sdkType)
		{
			var headers = GetWebRequestHeadersCollection(sdkType);
			var value = headers.FirstOrDefault(x => x.Name == "X-SDK")?.Value;

			Assert.IsNotNull(value, "SDK type parameter should not be null");
			Assert.AreEqual(sdkType.ToString().ToUpperInvariant(), value, "SDK type parameter should match expected value");
		}

		[Test]
		public void Headers_ShouldContain_SdkVersion_Param()
		{
			var headers = GetWebRequestHeadersCollection();
			var value = headers.FirstOrDefault(x => x.Name == "X-SDK-V")?.Value;

			Assert.IsNotNull(value, "SDK version parameter should not be null");
			Assert.AreEqual(Constants.SDK_VERSION.ToUpperInvariant(), value, "SDK version parameter should match SDK version");
		}

		[Test]
		public void Headers_ShouldContain_BuildPlatform_Param()
		{
			var headers = GetWebRequestHeadersCollection();
			var value = headers.FirstOrDefault(x => x.Name == "X-BUILD-PLATFORM")?.Value;

			Assert.IsNotNull(value, "Build platform parameter should not be null");
			Assert.AreEqual(EditorUserBuildSettings.activeBuildTarget.ToString().ToUpperInvariant(), value, "Build platform parameter should match Application.platform");
		}
	}
}