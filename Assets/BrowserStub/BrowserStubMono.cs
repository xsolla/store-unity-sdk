using System;
using UnityEngine;
using Xsolla.Core;

namespace BrowserStub
{
	public class BrowserStubMono : MonoBehaviour, IInAppBrowser
	{
		public event Action OpenEvent;
		public event Action<BrowserCloseInfo> CloseEvent;
		public event Action<string> UrlChangeEvent;
		public event Action<string, Action> AlertDialogEvent;
		public event Action<string, Action, Action> ConfirmDialogEvent;

		public bool IsOpened { get; private set; }
		public bool IsFullScreen { get; private set; }

		public void Open(string url)
		{
			Debug.Log("[BrowserStub] Open url: " + url);
		}

		public void Close(float delay = 0, bool isManually = false)
		{
			Debug.Log("[BrowserStub] Close");
		}

		public void AddInitHandler(Action callback)
		{
			Debug.Log("[BrowserStub] AddInitHandler");
		}

		public void AddCloseHandler(Action callback)
		{
			Debug.Log("[BrowserStub] AddCloseHandler");
		}

		public void AddUrlChangeHandler(Action<string> callback)
		{
			Debug.Log("[BrowserStub] AddUrlChangeHandler");
		}

		public void UpdateSize(int width, int height)
		{
			Debug.Log("[BrowserStub] UpdateSize: " + width + "x" + height);
		}

		public void SetFullscreenMode(bool isFullscreen)
		{
			Debug.Log("[BrowserStub] SetFullscreenMode: " + isFullscreen);
		}
	}
}