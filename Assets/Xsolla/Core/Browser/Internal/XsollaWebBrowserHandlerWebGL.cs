#if UNITY_WEBGL
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Xsolla.Core
{
	internal static class XsollaWebBrowserHandlerWebGL
	{
		[DllImport("__Internal")]
		private static extern void OpenPayStationWidget(string token, bool sandbox, string sdkType, string engineVersion, string sdkVersion, string applePayMerchantDomain, string appearanceJson);

		[DllImport("__Internal")]
		private static extern void ShowPopupAndOpenPayStation(string url, string popupMessage, string continueButtonText, string cancelButtonText);

		[DllImport("__Internal")]
		private static extern void ClosePayStationWidget();

		private static Action<bool> BrowserClosedCallback;

		public static void OpenPayStation(string token, Action<BrowserCloseInfo> onBrowserClosed, WebGlAppearance appearance, SdkType sdkType)
		{
			Screen.fullScreen = false;

			var isForcedIframeOnly = appearance != null
				&& appearance.iframeOnly.HasValue
				&& appearance.iframeOnly.Value;

			if (WebHelper.IsBrowserSafari() && !isForcedIframeOnly)
				ConfirmAndOpenPayStationPage(token, sdkType);
			else
				OpenPayStationWidgetImmediately(token, onBrowserClosed, appearance, sdkType);
		}

		public static void ClosePayStation(bool isManually)
		{
			BrowserClosedCallback?.Invoke(isManually);
			ClosePayStationWidget();
		}

		private static void OpenPayStationWidgetImmediately(string token, Action<BrowserCloseInfo> onBrowserClosed, WebGlAppearance appearance, SdkType sdkType)
		{
			if (appearance == null)
				appearance = new WebGlAppearance();

			BrowserClosedCallback = isManually => {
				var info = new BrowserCloseInfo {
					isManually = isManually
				};
				onBrowserClosed?.Invoke(info);
			};

			OpenPayStationWidget(
				token,
				XsollaSettings.IsSandbox,
				WebRequestHelper.GetSdkType(sdkType),
				Application.unityVersion,
				Constants.SDK_VERSION,
				XsollaSettings.ApplePayMerchantDomain,
				ParseUtils.ToJson(appearance));
		}

		private static void ConfirmAndOpenPayStationPage(string token, SdkType sdkType)
		{
			var url = new PayStationUrlBuilder(token, sdkType).Build();

			var browserLocale = WebHelper.GetBrowserLanguage().ToLowerInvariant();
			var popupMessage = XsollaWebBrowserLocalizationDataProvider.GetMessageText(browserLocale);
			var continueButtonText = XsollaWebBrowserLocalizationDataProvider.GetContinueButtonText(browserLocale);
			var cancelButtonText = XsollaWebBrowserLocalizationDataProvider.GetCancelButtonText(browserLocale);

			ShowPopupAndOpenPayStation(url, popupMessage, continueButtonText, cancelButtonText);
		}

		public static void OpenUrlInNewTab(string url)
		{
#pragma warning disable CS0618 // Type or member is obsolete
			Application.ExternalEval($"window.open('{url}', '_blank')");
#pragma warning restore CS0618 // Type or member is obsolete
		}
	}
}
#endif