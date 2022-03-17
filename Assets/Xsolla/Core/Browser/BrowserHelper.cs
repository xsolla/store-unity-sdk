using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_WEBGL || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
using System.Runtime.InteropServices;
#endif

namespace Xsolla.Core
{
	public class BrowserHelper : MonoSingleton<BrowserHelper>
	{
		private IInAppBrowser _inAppBrowser;

		public IInAppBrowser InAppBrowser
		{
			get
			{
#if UNITY_EDITOR || UNITY_STANDALONE
				if (_inAppBrowser != null)
					return _inAppBrowser;

				if (XsollaSettings.InAppBrowserEnabled)
					_inAppBrowser = GetComponent<IInAppBrowser>();
#endif

				return _inAppBrowser;
			}
		}

		private readonly Dictionary<PayStationUISettings.PaystationSize, Vector2> _payStationSizes = new Dictionary<PayStationUISettings.PaystationSize, Vector2>{
			{PayStationUISettings.PaystationSize.Small, new Vector2(620, 630)},
			{PayStationUISettings.PaystationSize.Medium, new Vector2(740, 760)},
			{PayStationUISettings.PaystationSize.Large, new Vector2(820, 840)}
		};

		private readonly Dictionary<string, int> _restrictedPaymentMethods = new Dictionary<string, int>{
			{"https://secure.xsolla.com/pages/paywithgoogle", 3431},
			{"https://sandbox-secure.xsolla.com/pages/paywithgoogle", 3431},
			{"https://secure.xsolla.com/pages/vkpay", 3496},
			{"https://sandbox-secure.xsolla.com/pages/vkpay", 3496}
		};

#if UNITY_WEBGL
		[DllImport("__Internal")]
		public static extern void ClosePaystationWidget();
		
		[DllImport("__Internal")]
		private static extern void OpenPaystationWidget(string token, bool sandbox);
#endif

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		[DllImport("XsollaSdkNative")]
		private static extern void OpenUrlInSafari(string url);
#endif

		public void OpenPurchase(string url, string token, bool forcePlatformBrowser = false, Action<int> onRestrictedPaymentMethod = null)
		{
			var isEditor = Application.isEditor;
			var inAppBrowserEnabled = XsollaSettings.InAppBrowserEnabled;
			var isSandbox = XsollaSettings.IsSandbox;
			
#if UNITY_ANDROID
			if (!isEditor && inAppBrowserEnabled)
			{
				using (var sdkHelper = new AndroidSDKPaymentsHelper())
				{
					sdkHelper.PerformPayment(token, isSandbox);
				}
				return;
			}
#elif UNITY_IOS
			if (!isEditor && inAppBrowserEnabled)
			{
				new IosSDKPaymentsHelper().PerformPayment(token, isSandbox);
				return;
			}
#elif UNITY_WEBGL
			if((Application.platform == RuntimePlatform.WebGLPlayer) && inAppBrowserEnabled)
			{
				Screen.fullScreen = false;
				OpenPaystationWidget(token, isSandbox);
				return;
			}
#endif

			Open(url + token, forcePlatformBrowser);

#if UNITY_STANDALONE || UNITY_EDITOR
			if (InAppBrowser != null && !forcePlatformBrowser)
			{
				TrackRestrictedPaymentMethod(onRestrictedPaymentMethod);
				UpdateBrowserSize();
			}
#endif
		}

		public void Open(string url, bool forcePlatformBrowser = false)
		{
			if (EnvironmentDefiner.IsStandaloneOrEditor && InAppBrowser != null && !forcePlatformBrowser)
			{
				OpenInAppBrowser(url);
			}
			else if (EnvironmentDefiner.IsWebGL)
			{
				OpenWebGlBrowser(url);
			}
			else
			{
				OpenPlatformBrowser(url);
			}
		}

		public void Close(float delay = 0)
		{
			InAppBrowser?.Close(delay);
		}

		private void OpenInAppBrowser(string url)
		{
			InAppBrowser.Open(url);
		}

		private void OpenWebGlBrowser(string url)
		{
#pragma warning disable 0618
			Application.ExternalEval($"window.open(\"{url}\",\"_blank\")");
#pragma warning restore 0618
		}

		private void OpenPlatformBrowser(string url)
		{
			Application.OpenURL(url);
		}

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		private void OpenSafari(string url)
		{
			OpenUrlInSafari(url);
		}
#endif

#if UNITY_STANDALONE || UNITY_EDITOR
		private void UpdateBrowserSize()
		{
			InAppBrowser.AddInitHandler(() =>
			{
				var payStationSettings = XsollaSettings.DesktopPayStationUISettings;
				var payStationSize = payStationSettings.paystationSize != PayStationUISettings.PaystationSize.Auto
					? payStationSettings.paystationSize
					: PayStationUISettings.PaystationSize.Medium;

				var viewportSize = _payStationSizes[payStationSize];
				InAppBrowser.UpdateSize((int) viewportSize.x, (int) viewportSize.y);
			});
		}

		private void TrackRestrictedPaymentMethod(Action<int> onRestrictedPaymentMethod)
		{
			InAppBrowser.AddInitHandler(() =>
			{
				InAppBrowser.AddUrlChangeHandler(url =>
				{
					if (_restrictedPaymentMethods.ContainsKey(url))
					{
						onRestrictedPaymentMethod?.Invoke(_restrictedPaymentMethods[url]);
					}
				});
			});
		}
#endif
	}
}
