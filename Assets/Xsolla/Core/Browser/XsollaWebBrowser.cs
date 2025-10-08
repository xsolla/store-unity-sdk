using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Xsolla.Core
{
	public static class XsollaWebBrowser
	{
		private static IInAppBrowser _inAppBrowser;
		private static GameObject _inAppBrowserGameObject;

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
					if (!prefab)
					{
						XDebug.LogError("Prefab InAppBrowser not found in Resources folder.");
					}
					else
					{
						_inAppBrowserGameObject = Object.Instantiate(prefab);
						_inAppBrowserGameObject.name = "XsollaWebBrowser";
						Object.DontDestroyOnLoad(_inAppBrowserGameObject);
						_inAppBrowser = _inAppBrowserGameObject.GetComponent<IInAppBrowser>();
					}
				}

				return _inAppBrowser;
#endif
			}
		}

		public static void OpenPurchaseUI(string paymentToken, bool forcePlatformBrowser = false, Action<BrowserCloseInfo> onBrowserClosed = null, PlatformSpecificAppearance platformSpecificAppearance = null, SdkType sdkType = SdkType.Store)
		{
			if (!Application.isEditor && XsollaSettings.InAppBrowserEnabled)
			{
#if UNITY_ANDROID
				XsollaWebBrowserHandlerAndroid.OpenPayStation(paymentToken, onBrowserClosed, platformSpecificAppearance);
				return;
#elif UNITY_IOS
				XsollaWebBrowserHandlerIos.OpenPayStation(paymentToken, onBrowserClosed);
				return;
#elif UNITY_WEBGL
				XsollaWebBrowserHandlerWebGL.OpenPayStation(paymentToken, onBrowserClosed, platformSpecificAppearance?.WebGlAppearance, sdkType);
				return;
#endif
			}

			var url = new PayStationUrlBuilder(paymentToken, sdkType).Build();
			XDebug.Log($"Purchase url: {url}");
			Open(url, forcePlatformBrowser);

#if UNITY_STANDALONE || UNITY_EDITOR
			if (InAppBrowser != null && !forcePlatformBrowser)
			{
				InAppBrowser.CloseEvent += onBrowserClosed;
			}
#endif
		}

		public static void Open(string url, bool forcePlatformBrowser = false)
		{
			XDebug.Log($"Open url: {url}");

#if UNITY_EDITOR || UNITY_STANDALONE
			if (InAppBrowser != null && !forcePlatformBrowser)
				InAppBrowser.Open(url);
			else
				Application.OpenURL(url);
#elif UNITY_WEBGL
			XsollaWebBrowserHandlerWebGL.OpenUrlInNewTab(url);
#else
			Application.OpenURL(url);
#endif
		}

		public static void Close(float delay = 0, bool isManually = false)
		{
			_inAppBrowser?.Close(delay, isManually);

			if (_inAppBrowserGameObject)
				Object.Destroy(_inAppBrowserGameObject);

			_inAppBrowser = null;
		}
	}
}