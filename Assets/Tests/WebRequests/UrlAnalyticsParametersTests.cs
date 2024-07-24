using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Tests.WebRequests
{
	public class UrlAnalyticsParametersTests : WebRequestTestBase
	{
		[Test]
		public void Url_ShouldContain_Engine_Param()
		{
			var url = CreateUrlWithAnalytics();
			var queryParams = ParseQueryParams(url);
			var value = queryParams.Get("engine");

			Assert.IsNotNull(value, $"Engine parameter should not be null. Url: {url}");
			Assert.AreEqual("unity", value, $"Engine parameter should be 'unity'. Url: {url}");
		}

		[Test]
		public void Url_ShouldContain_EngineVersion_Param()
		{
			var url = CreateUrlWithAnalytics();
			var queryParams = ParseQueryParams(url);
			var value = queryParams.Get("engine_v");

			Assert.IsNotNull(value, $"Engine version parameter should not be null. Url: {url}");
			Assert.AreEqual(Application.unityVersion.ToLowerInvariant(), value, $"Engine version parameter should match Unity version. Url: {url}");
		}

		[Test]
		[TestCase(SdkType.Login)]
		[TestCase(SdkType.Store)]
		[TestCase(SdkType.Subscriptions)]
		public void Url_ShouldContain_SdkType_Param(SdkType sdkType)
		{
			var url = CreateUrlWithAnalytics(sdkType);
			var queryParams = ParseQueryParams(url);
			var value = queryParams.Get("sdk");

			Assert.IsNotNull(value, $"SDK type parameter should not be null. Url: {url}");
			Assert.AreEqual(sdkType.ToString().ToLowerInvariant(), value, $"SDK type parameter should match expected value. Url: {url}");
		}

		[Test]
		public void Url_ShouldContain_SdkVersion_Param()
		{
			var url = CreateUrlWithAnalytics();
			var queryParams = ParseQueryParams(url);
			var value = queryParams.Get("sdk_v");

			Assert.IsNotNull(value, $"SDK version parameter should not be null. Url: {url}");
			Assert.AreEqual(Constants.SDK_VERSION.ToLowerInvariant(), value, $"SDK version parameter should match SDK version. Url: {url}");
		}

		[Test]
		public void Url_ShouldContain_BuildPlatform_Param()
		{
			var url = CreateUrlWithAnalytics();
			var queryParams = ParseQueryParams(url);
			var value = queryParams.Get("build_platform");

			Assert.IsNotNull(value, $"Build platform parameter should not be null. Url: {url}");
			Assert.AreEqual(EditorUserBuildSettings.activeBuildTarget.ToString().ToLowerInvariant(), value, $"Build platform parameter should match active build target. Url: {url}");
		}
	}
}