using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core.Browser;

namespace Xsolla.Core
{
	[AddComponentMenu("Scripts/Xsolla.Core/Browser/BrowserHelper")]
	public class BrowserHelper : MonoSingleton<BrowserHelper>
	{
		[SerializeField] private GameObject InAppBrowserPrefab = default;

		private readonly Dictionary<PayStationUISettings.PaystationSize, Vector2> _payStationSizes = new Dictionary<PayStationUISettings.PaystationSize, Vector2>
		{
			{PayStationUISettings.PaystationSize.Small, new Vector2(620, 630)},
			{PayStationUISettings.PaystationSize.Medium, new Vector2(740, 760)},
			{PayStationUISettings.PaystationSize.Large, new Vector2(820, 840)}
		};

		private readonly Dictionary<string, int> _restrictedPaymentMethods = new Dictionary<string, int>
		{
			{"https://secure.xsolla.com/pages/paywithgoogle", 3431},
			{"https://sandbox-secure.xsolla.com/pages/paywithgoogle", 3431},
			{"https://secure.xsolla.com/pages/vkpay", 3496},
			{"https://sandbox-secure.xsolla.com/pages/vkpay", 3496}
		};

		private GameObject _inAppBrowserObject = default;

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

		protected override void OnDestroy()
		{
			if (_inAppBrowserObject != null)
			{
				Destroy(_inAppBrowserObject);
				_inAppBrowserObject = null;
			}
		}

		public bool IsOpened => (GetLastBrowser() != null);

		public void OpenPurchase(string url, string token, bool isSandbox, bool inAppBrowserEnabled = false, Action<int> onRestrictedPaymentMethod = null)
		{
#if UNITY_WEBGL
			if((Application.platform == RuntimePlatform.WebGLPlayer) && inAppBrowserEnabled)
			{
				Screen.fullScreen = false;
				OpenPaystationWidget(token, isSandbox);
				return;
			}
#endif
			Open(url + token, inAppBrowserEnabled);

			if (inAppBrowserEnabled)
			{
				TrackRestrictedPaymentMethod(onRestrictedPaymentMethod);
				UpdateBrowserSize();
			}
		}

		public void Open(string url, bool inAppBrowserEnabled = false)
		{
			if (EnvironmentDefiner.IsStandaloneOrEditor && inAppBrowserEnabled)
			{
				OpenInAppBrowser(url);
			}
			else if (EnvironmentDefiner.IsWebGL)
			{
#pragma warning disable 0618
				Application.ExternalEval($"window.open(\"{url}\",\"_blank\")");
#pragma warning restore 0618
			}
			else
			{
				Application.OpenURL(url);
			}
		}

		public void OpenInAppBrowser(string url, Action onClosed, Action<string> onParameter, ParseParameter parameterToLook)
		{
			if (!EnvironmentDefiner.IsStandaloneOrEditor)
			{
				var errorMessage = "OpenInAppBrowser: This functionality is not supported elswere except Editor and Standalone build";
				Debug.LogError(errorMessage);
				onClosed?.Invoke();
				return;
			}

			Open(url, XsollaSettings.InAppBrowserEnabled);
			var _browser = GetLastBrowser();
			_browser.BrowserClosedEvent += () => onClosed?.Invoke();
			_browser.BrowserInitEvent += activeBrowser =>
			{
				activeBrowser.Navigate.UrlChangedEvent += (browser, newUrl) =>
				{
					Debug.Log($"URL = {newUrl}");

					if (ParseUtils.TryGetValueFromUrl(newUrl, parameterToLook, out string parameter))
					{
						StartCoroutine(CloseBrowserCoroutine());
						onParameter?.Invoke(parameter);
					}
				};
			};
		}

		private void OpenInAppBrowser(string url)
		{
			if (_inAppBrowserObject == null)
			{
				Canvas canvas = FindObjectOfType<Canvas>();
				if(canvas == null)
				{
					Debug.LogError("Can not find canvas! So can not draw 2D browser!");
					return;
				}

				_inAppBrowserObject = Instantiate(InAppBrowserPrefab, canvas.transform);
				XsollaBrowser xsollaBrowser = _inAppBrowserObject.GetComponentInChildren<XsollaBrowser>();
				xsollaBrowser.Navigate.To(url);
			}
			else
				Debug.LogError("Attempt to create secondary browser instance");
		}

		public SinglePageBrowser2D GetLastBrowser()
		{
			return _inAppBrowserObject?.GetComponentInChildren<SinglePageBrowser2D>();
		}

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		void OpenSafari(string url)
		{
			OpenUrlInSafari(url);
		}
#endif
		public void CloseIfExists()
		{
			if (BrowserHelper.Instance.GetLastBrowser() != null)
				if (EnvironmentDefiner.IsStandaloneOrEditor)
					StartCoroutine(CloseBrowserCoroutine());
		}

		public bool AddOnBrowserClose(Action callback)
		{
			var browser = GetLastBrowser();
			if (browser != null)
			{
				browser.BrowserClosedEvent += callback;
				return true;
			}
			else
			{
				//TEXTREVIEW
				Debug.LogWarning("Attempt to add OnClose callback while no browser present");
				return false;
			}
		}

		private IEnumerator CloseBrowserCoroutine()
		{
			yield return new WaitForEndOfFrame();
			Destroy(gameObject);
		}

		private void UpdateBrowserSize()
		{
#if UNITY_STANDALONE || UNITY_EDITOR
			BrowserHelper.Instance.GetLastBrowser().BrowserInitEvent += activeBrowser =>
			{
				var browserRender = BrowserHelper.Instance.GetLastBrowser().GetComponent<Display2DBehaviour>();
				if (browserRender == null)
					return;

				var payStationSettings = XsollaSettings.DesktopPayStationUISettings;
				var payStationSize = payStationSettings.paystationSize != PayStationUISettings.PaystationSize.Auto
					? payStationSettings.paystationSize
					: PayStationUISettings.PaystationSize.Medium;

				var viewportSize = _payStationSizes[payStationSize];
				browserRender.StartRedrawWith((int)viewportSize.x, (int)viewportSize.y);
			};
#endif
		}

		private void TrackRestrictedPaymentMethod(Action<int> onRestrictedPaymentMethod)
		{
			Instance.GetLastBrowser().BrowserInitEvent += activeBrowser =>
			{
				activeBrowser.Navigate.UrlChangedEvent += (browser, newUrl) =>
				{
					if (_restrictedPaymentMethods.ContainsKey(newUrl))
					{
						onRestrictedPaymentMethod?.Invoke(_restrictedPaymentMethods[newUrl]);
					}
				};
			};
		}
	}
}
