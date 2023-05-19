using System;
using UnityEngine;
#if UNITY_WEBGL || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
using System.Runtime.InteropServices;
#endif

namespace Xsolla.Core
{
	public class XsollaWebBrowser : MonoBehaviour
	{
		private static IInAppBrowser _inAppBrowser;

		public static IInAppBrowser InAppBrowser
		{
			get
			{
#if !(UNITY_EDITOR || UNITY_STANDALONE)
				return null;
#else
				if (!XsollaSettings.InAppBrowserEnabled)
					return null;

				if (_inAppBrowser == null)
				{
					var prefab = Resources.Load<GameObject>(Constants.WEB_BROWSER_RESOURCE_PATH);
					if (prefab == null)
					{
						XDebug.LogError("Prefab InAppBrowser not found in Resources folder.");
					}
					else
					{
						var go = Instantiate(prefab);
						go.name = "XsollaWebBrowser";
						DontDestroyOnLoad(go);
						_inAppBrowser = go.GetComponent<IInAppBrowser>();
					}
				}

				return _inAppBrowser;
#endif
			}
		}

		public static void OpenPurchaseUI(string paymentToken, bool forcePlatformBrowser = false, Action<BrowserCloseInfo> onBrowserClosed = null)
		{
#if UNITY_ANDROID
			if (!Application.isEditor && XsollaSettings.InAppBrowserEnabled)
			{
				new AndroidPayments().Perform(
					paymentToken,
					isClosedManually =>
					{
						var info = new BrowserCloseInfo {
							isManually = isClosedManually
						};
						onBrowserClosed?.Invoke(info);
					});

				return;
			}

#elif UNITY_IOS
			if (!Application.isEditor && XsollaSettings.InAppBrowserEnabled)
			{
				new IosPayments().Perform(
					paymentToken,
					isClosedManually =>
					{
						var info = new BrowserCloseInfo {
							isManually = isClosedManually
						};
						onBrowserClosed?.Invoke(info);
					});

				return;
			}

#elif UNITY_WEBGL && !UNITY_EDITOR
			if (XsollaSettings.InAppBrowserEnabled)
			{
				WebGLBrowserClosedCallback = isManually =>
				{
					var info = new BrowserCloseInfo {
						isManually = isManually
					};
					onBrowserClosed?.Invoke(info);
				};

				Screen.fullScreen = false;
				OpenPaystationWidget(paymentToken, XsollaSettings.IsSandbox);
				return;
			}
#endif

			var url = XsollaSettings.IsSandbox
				? Constants.PAYSTATION_SANDBOX_URL
				: Constants.PAYSTATION_URL;

			url = new UrlBuilder(url)
				.AddParam("access_token", paymentToken)
				.Build();

			Open(url, forcePlatformBrowser);

#if UNITY_STANDALONE || UNITY_EDITOR
			if (InAppBrowser != null && !forcePlatformBrowser)
			{
				UpdateBrowserSize();
				InAppBrowser.CloseEvent += onBrowserClosed;
			}
#endif
		}

		public static void Open(string url, bool forcePlatformBrowser = false)
		{
			XDebug.Log($"WebBrowser. Open url: {url}");
#if UNITY_EDITOR || UNITY_STANDALONE
			if (InAppBrowser != null && !forcePlatformBrowser)
				InAppBrowser.Open(url);
			else
				Application.OpenURL(url);
#elif UNITY_WEBGL
#pragma warning disable 0618
			Application.ExternalEval($"window.open(\"{url}\",\"_blank\")");
#pragma warning restore 0618
			return;
#else
			Application.OpenURL(url);
#endif
		}

		public static void Close(float delay = 0)
		{
			InAppBrowser?.Close(delay);
		}

#if UNITY_WEBGL
		[DllImport("__Internal")]
		private static extern void OpenPaystationWidget(string token, bool sandbox);

		[DllImport("__Internal")]
		private static extern void ClosePaystationWidget();

		private static Action<bool> WebGLBrowserClosedCallback;

		public static void ClosePaystationWidget(bool isManually)
		{
			WebGLBrowserClosedCallback?.Invoke(isManually);
			ClosePaystationWidget();
		}
#endif

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		[DllImport("XsollaSdkNative")]
		private static extern void OpenUrlInSafari(string url);

		public static void OpenSafari(string url)
		{
			OpenUrlInSafari(url);
		}
#endif

#if UNITY_STANDALONE || UNITY_EDITOR
		private static void UpdateBrowserSize()
		{
			InAppBrowser.AddInitHandler(() =>
			{
				var payStationSettings = XsollaSettings.DesktopPayStationUISettings;
				var payStationSize = payStationSettings.paystationSize != PayStationUISettings.PaystationSize.Auto
					? payStationSettings.paystationSize
					: PayStationUISettings.PaystationSize.Medium;

				var viewportSize = GetBrowserSize(payStationSize);
				InAppBrowser.UpdateSize((int) viewportSize.x, (int) viewportSize.y);
			});
		}

		private static Vector2 GetBrowserSize(PayStationUISettings.PaystationSize paystationSize)
		{
			switch (paystationSize)
			{
				case PayStationUISettings.PaystationSize.Small: return new Vector2(620, 630);
				case PayStationUISettings.PaystationSize.Medium: return new Vector2(740, 760);
				case PayStationUISettings.PaystationSize.Large: return new Vector2(820, 840);
				default:
					throw new ArgumentOutOfRangeException(nameof(paystationSize), paystationSize, null);
			}
		}
#endif
	}
}