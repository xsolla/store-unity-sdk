using System;
using UnityEngine;

namespace Xsolla.Core.Browser
{
#if !(UNITY_EDITOR || UNITY_STANDALONE)
	internal class XsollaInAppBrowser : MonoBehaviour { }
#else
	internal class XsollaInAppBrowser : MonoBehaviour, IInAppBrowser
	{
		[SerializeField] private GameObject BrowserPrefab;
		[SerializeField] private bool IsDontDestroyOnLoad;
		private GameObject BrowserObject;
		private SinglePageBrowser2D SinglePageBrowser;
		private XsollaBrowser XsollaBrowser;

		public event Action OpenEvent;
		public event Action<BrowserCloseInfo> CloseEvent;
		public event Action<string> UrlChangeEvent;

		public event Action<string, Action> AlertDialogEvent;
		public event Action<string, Action, Action> ConfirmDialogEvent;

		public bool IsOpened => BrowserObject;

		public void Close(float delay = 0f, bool isManually = false)
		{
			if (SinglePageBrowser)
			{
				SinglePageBrowser.BrowserCloseRequest -= OnBrowserCloseRequest;
				SinglePageBrowser.AlertDialogEvent -= OnAlertDialogEvent;
				SinglePageBrowser.ConfirmDialogEvent -= OnConfirmDialogEvent;
			}

			if (XsollaBrowser && XsollaBrowser.Navigate != null)
				XsollaBrowser.Navigate.UrlChangedEvent -= OnUrlChanged;

			if (BrowserObject)
				Destroy(BrowserObject, delay);

			var info = new BrowserCloseInfo {
				isManually = isManually
			};
			CloseEvent?.Invoke(info);

			OpenEvent = null;
			CloseEvent = null;
			UrlChangeEvent = null;
			AlertDialogEvent = null;
			ConfirmDialogEvent = null;
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
				CloseEvent += _ => action?.Invoke();
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
				SinglePageBrowser.SetViewport(new Vector2Int(width, height));
		}

		private void CreateBrowser()
		{
			BrowserObject = Instantiate(BrowserPrefab);
			BrowserObject.name = "XsollaInAppBrowser";
			if (IsDontDestroyOnLoad)
				DontDestroyOnLoad(BrowserObject);

			SinglePageBrowser = BrowserObject.GetComponentInChildren<SinglePageBrowser2D>();
			SinglePageBrowser.BrowserCloseRequest += OnBrowserCloseRequest;
			SinglePageBrowser.AlertDialogEvent += OnAlertDialogEvent;
			SinglePageBrowser.ConfirmDialogEvent += OnConfirmDialogEvent;

			XsollaBrowser = BrowserObject.GetComponentInChildren<XsollaBrowser>();
			XsollaBrowser.Navigate.UrlChangedEvent += OnUrlChanged;
		}

		private void OnBrowserCloseRequest()
		{
			Close(0, true);
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
	}
#endif
}