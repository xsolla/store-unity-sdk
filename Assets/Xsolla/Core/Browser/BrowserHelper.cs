using System.Runtime.InteropServices;
using UnityEngine;

namespace Xsolla.Core
{
	[AddComponentMenu("Scripts/Xsolla.Core/Browser/BrowserHelper")]
	public class BrowserHelper : MonoSingleton<BrowserHelper>
	{
		[SerializeField]
		private GameObject InAppBrowserPrefab;
		private GameObject InAppBrowserObject;

		[DllImport("__Internal")]
		private static extern void Purchase(string token, bool sandbox);

		private void OnDestroy()
		{
			if(InAppBrowserObject != null) {
				Destroy(InAppBrowserObject);
				InAppBrowserObject = null;
			}
		}

		public void OpenPurchase(string url, string token)
		{
			if((Application.platform == RuntimePlatform.WebGLPlayer) && XsollaSettings.InAppBrowserEnabled) {
				Purchase(token, XsollaSettings.IsSandbox);
			} else {
				Open(url + token);
			}
		}

		public void Open(string url, bool ignoreSettings = false)
		{
			switch (Application.platform) {
				case RuntimePlatform.WebGLPlayer: {
						url = string.Format("window.open(\"{0}\",\"_blank\")", url);
						Application.ExternalEval(url);
						break;
					}
				default: {
#if (UNITY_EDITOR || UNITY_STANDALONE)
						if (!ignoreSettings && XsollaSettings.InAppBrowserEnabled && (InAppBrowserPrefab != null)) {
							OpenInAppBrowser(url);
						} else
#endif
							Application.OpenURL(url);
						break;
					}
			}
		}

#if (UNITY_EDITOR || UNITY_STANDALONE)
		private void OpenInAppBrowser(string url)
		{
			if (InAppBrowserObject == null) {
				Canvas canvas = FindObjectOfType<Canvas>();
				if(canvas == null) {
					Debug.LogError("Can not find canvas! So can not draw 2D browser!");
					return;
				}
				InAppBrowserObject = Instantiate(InAppBrowserPrefab, canvas.transform);
				XsollaBrowser xsollaBrowser = InAppBrowserObject.GetComponent<XsollaBrowser>();
				xsollaBrowser.Navigate.To(url);
			} else {
				Debug.LogError("You try create browser instance, but it already created!");
			}
		}
#endif
	}
}
