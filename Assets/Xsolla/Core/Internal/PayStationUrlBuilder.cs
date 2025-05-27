using System;
using UnityEngine;

namespace Xsolla.Core
{
	internal class PayStationUrlBuilder
	{
		private readonly string PaymentToken;
		private readonly SdkType SdkType;
		private readonly bool IsSandBox;
		private readonly int PayStationVersion;

		public PayStationUrlBuilder(string paymentToken, SdkType sdkType = SdkType.Store)
		{
			PaymentToken = paymentToken;
			SdkType = sdkType;
			IsSandBox = XsollaSettings.IsSandbox;
			PayStationVersion = XsollaSettings.PaystationVersion;
		}

		public string Build()
		{
			var url = GetPaystationBasePath() + GetPaystationVersionPath();

			return new UrlBuilder(url)
				.AddParam(GetTokenQueryKey(), PaymentToken)
				.AddParam("engine", "unity")
				.AddParam("engine_v", Application.unityVersion)
				.AddParam("sdk", WebRequestHelper.GetSdkType(SdkType))
				.AddParam("sdk_v", Constants.SDK_VERSION)
				.AddParam("browser_type", GetBrowserType())
				.AddParam("build_platform", GetBuildPlatform())
				.Build();
		}

		private string GetPaystationBasePath()
		{
			return IsSandBox
				? "https://sandbox-secure.xsolla.com/"
				: "https://secure.xsolla.com/";
		}

		private string GetPaystationVersionPath()
		{
			switch (PayStationVersion)
			{
				case 3:  return "paystation3";
				case 4:  return "paystation4";
				default: throw new Exception($"Unknown Paystation version: {PayStationVersion}");
			}
		}

		private string GetTokenQueryKey()
		{
			switch (PayStationVersion)
			{
				case 3:  return "access_token";
				case 4:  return "token";
				default: throw new Exception($"Unknown PayStation version: {PayStationVersion}");
			}
		}

		private string GetBrowserType()
		{
			return XsollaSettings.InAppBrowserEnabled
				? "inapp"
				: "system";
		}

		private string GetBuildPlatform()
		{
			return Application.platform.ToString().ToLowerInvariant();
		}
	}
}