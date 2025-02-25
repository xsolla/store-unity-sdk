#if UNITY_WEBGL
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Xsolla.Core
{
	internal static class XsollaWebBrowserHandlerWebGL
	{
		[DllImport("__Internal")]
		private static extern void OpenPayStationWidget(string token, bool sandbox, string engineVersion, string sdkVersion, string applePayMerchantDomain);

		[DllImport("__Internal")]
		private static extern void ClosePayStationWidget();

		private static Action<bool> BrowserClosedCallback;

		public static void OpenPayStation(string token, Action<BrowserCloseInfo> onBrowserClosed)
		{
			BrowserClosedCallback = isManually => {
				var info = new BrowserCloseInfo {
					isManually = isManually
				};
				onBrowserClosed?.Invoke(info);
			};

			Screen.fullScreen = false;

			OpenPayStationWidget(
				token,
				XsollaSettings.IsSandbox,
				Application.unityVersion,
				Constants.SDK_VERSION,
				XsollaSettings.ApplePayMerchantDomain);
		}

		public static void ClosePayStation(bool isManually)
		{
			BrowserClosedCallback?.Invoke(isManually);
			ClosePayStationWidget();
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