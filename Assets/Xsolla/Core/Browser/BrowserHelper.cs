using System.Runtime.InteropServices;
using UnityEngine;
using Xsolla.Core.Browser;

namespace Xsolla.Core
{
	[AddComponentMenu("Scripts/Xsolla.Core/Browser/BrowserHelper")]
	public class BrowserHelper : MonoSingleton<BrowserHelper>
	{
#if UNITY_EDITOR || UNITY_STANDALONE
		[SerializeField] private GameObject InAppBrowserPrefab = default;
#endif

		private GameObject _inAppBrowserObject = default;

#if UNITY_WEBGL
		[DllImport("__Internal")]
		private static extern void OpenPaystationWidget(string token, bool sandbox);
		
		[DllImport("__Internal")]
		public static extern void ClosePaystationWidget();
#endif

		protected override void OnDestroy()
		{
			if (_inAppBrowserObject == null)
				return;

			Destroy(_inAppBrowserObject);
			_inAppBrowserObject = null;
		}

		public void OpenPurchase(string url, string token, bool isSandbox, bool inAppBrowserEnabled = false)
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
		}

		public void Open(string url, bool inAppBrowserEnabled = false)
		{
			switch (Application.platform)
			{
				case RuntimePlatform.WebGLPlayer:
#pragma warning disable 0618
					Application.ExternalEval($"window.open(\"{url}\",\"_blank\")");
#pragma warning restore 0618
					break;
				default:
#if UNITY_EDITOR || UNITY_STANDALONE
					if (inAppBrowserEnabled && InAppBrowserPrefab != null)
					{
						OpenInAppBrowser(url);
						break;
					}
#endif
					Application.OpenURL(url);
					break;
			}
		}

#if UNITY_EDITOR || UNITY_STANDALONE
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
#endif
	}
}
