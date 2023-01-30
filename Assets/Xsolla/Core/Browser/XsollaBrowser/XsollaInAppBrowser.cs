using System;
using UnityEngine;
using Xsolla.Core.Browser;

namespace Xsolla.Core.Browser
{
#if (UNITY_EDITOR || UNITY_STANDALONE)
	public class XsollaInAppBrowser : MonoBehaviour, IInAppBrowser
#else
	public class XsollaInAppBrowser : MonoBehaviour
#endif
	{
#if (UNITY_EDITOR || UNITY_STANDALONE)
		[SerializeField] private GameObject BrowserPrefab = default;
		private GameObject BrowserObject;
		private SinglePageBrowser2D SinglePageBrowser;
		private XsollaBrowser XsollaBrowser;

		public event Action OpenEvent;
		public event Action CloseEvent;
		public event Action<string> UrlChangeEvent;
		
		public event Action<string, Action> AlertDialogEvent;
		public event Action<string, Action, Action> ConfirmDialogEvent;

		public bool IsOpened => BrowserObject;

		private void OnDestroy()
		{
			Close();
		}

		public void Close(float delay = 0f)
		{
			if (SinglePageBrowser)
			{
				SinglePageBrowser.BrowserClosedEvent -= OnBrowserClosed;
				SinglePageBrowser.AlertDialogEvent -= OnAlertDialogEvent;
				SinglePageBrowser.ConfirmDialogEvent -= OnConfirmDialogEvent;
			}
			
			if (XsollaBrowser)
				XsollaBrowser.Navigate.UrlChangedEvent -= OnUrlChanged;
			
			if (BrowserObject)
				Destroy(BrowserObject, delay);
			
			CloseEvent?.Invoke();
		}

		public void Open(string url)
		{
			if (!IsOpened)
				CreateBrowser();

			XsollaBrowser.Navigate.To(url);
			OpenEvent?.Invoke();
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
			{
				SinglePageBrowser.SetViewport(new Vector2(width, height));
				SinglePageBrowser.GetComponent<Display2DBehaviour>().StartRedraw(width, height);
			}
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
			SinglePageBrowser.BrowserClosedEvent += OnBrowserClosed;
			SinglePageBrowser.AlertDialogEvent += OnAlertDialogEvent;
			SinglePageBrowser.ConfirmDialogEvent += OnConfirmDialogEvent;
			
			XsollaBrowser = BrowserObject.GetComponentInChildren<XsollaBrowser>();
			XsollaBrowser.Navigate.UrlChangedEvent += OnUrlChanged;
		}
		
		private void OnBrowserClosed()
		{
			CloseEvent?.Invoke();
		}

		private void OnUrlChanged(IXsollaBrowser browser, string url)
		{
			UrlChangeEvent?.Invoke(url);
		}
		
		private void OnAlertDialogEvent(string message, Action acceptAction)
		{
			AlertDialogEvent?.Invoke(message, acceptAction);
		}
		
		private void OnConfirmDialogEvent(string message, Action acceptAction, Action cancelAction)
		{
			ConfirmDialogEvent?.Invoke(message, acceptAction, cancelAction);
		}
#endif
	}
}
