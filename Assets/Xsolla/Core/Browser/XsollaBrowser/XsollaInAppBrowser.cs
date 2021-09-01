#if (UNITY_EDITOR || UNITY_STANDALONE)
using System;
using UnityEngine;
using Xsolla.Core.Browser;

namespace Xsolla.Core
{
	public class XsollaInAppBrowser : MonoBehaviour, IInAppBrowser
	{
		[SerializeField] private GameObject BrowserPrefab;

		private GameObject BrowserObject;

		private SinglePageBrowser2D SinglePageBrowser;

		private XsollaBrowser XsollaBrowser;

		public bool IsOpened => BrowserObject;

		private void OnDestroy()
		{
			Close();
		}

		public void Close(float delay = 0f)
		{
			if (BrowserObject)
				Destroy(BrowserObject, delay);
		}

		public void Open(string url)
		{
			if (!IsOpened)
				CreateBrowser();
			
			XsollaBrowser.Navigate.To(url);
		}

		public void AddCloseHandler(Action action)
		{
			if (IsOpened)
				SinglePageBrowser.BrowserClosedEvent += action;
		}

		public void AddUrlChangeHandler(Action<string> callback)
		{
			if (IsOpened)
				XsollaBrowser.Navigate.UrlChangedEvent += (browser, url) => callback?.Invoke(url);
		}

		public void AddInitHandler(Action callback)
		{
			if (IsOpened)
				SinglePageBrowser.BrowserInitEvent += browser => callback?.Invoke();
		}

		public void UpdateSize(int width, int height)
		{
			if (IsOpened)
				SinglePageBrowser.GetComponent<Display2DBehaviour>().StartRedrawWith(width, height);
		}

		private void CreateBrowser()
		{
			var canvas = FindObjectOfType<Canvas>();
			if (canvas == null)
			{
				Debug.LogError("Can not find canvas! So can not draw 2D browser!");
				return;
			}

			BrowserObject = Instantiate(BrowserPrefab, canvas.transform);
			SinglePageBrowser = BrowserObject.GetComponentInChildren<SinglePageBrowser2D>();
			XsollaBrowser = BrowserObject.GetComponentInChildren<XsollaBrowser>();
		}
	}
}
#endif