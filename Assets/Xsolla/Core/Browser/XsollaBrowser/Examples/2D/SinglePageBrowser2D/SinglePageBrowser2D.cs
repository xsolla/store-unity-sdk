using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Core.Browser
{
	[RequireComponent(typeof(Image))]
	public class SinglePageBrowser2D : MonoBehaviour
	{
#pragma warning disable
		[SerializeField] private Button CloseButton = default;
		[SerializeField] private Button BackButton = default;
		[SerializeField] private Vector2 Viewport = new Vector2(1920.0F, 1080.0F);
		[SerializeField] private GameObject PreloaderPrefab = default;

		public event Action<IXsollaBrowser> BrowserInitEvent;
		public event Action BrowserClosedEvent;

		public event Action<string, Action> AlertDialogEvent;
		public event Action<string, Action, Action> ConfirmDialogEvent;
#pragma warning restore

#if UNITY_EDITOR || UNITY_STANDALONE
		private XsollaBrowser xsollaBrowser;
		private Display2DBehaviour display;
		private Keyboard2DBehaviour keyboard;
		private Mouse2DBehaviour mouse;
		private string _urlBeforePopup;
		private Preloader2DBehaviour _preloader;
		
		public void SetViewport(Vector2 viewport)
		{
			Viewport = viewport;
		}

		private void Awake()
		{
			BackButton.onClick.AddListener(OnBackButtonPressed);

			CloseButton.gameObject.SetActive(false);
			BackButton.gameObject.SetActive(false);

			var canvasTrn = GetComponentInParent<Canvas>().transform;
			var canvasRect = ((RectTransform) canvasTrn).rect;

			if (Viewport.x > canvasRect.width)
				Viewport.x = canvasRect.width;
			if (Viewport.y > canvasRect.height)
				Viewport.y = canvasRect.height;

			xsollaBrowser = this.GetOrAddComponent<XsollaBrowser>();
			xsollaBrowser.LogEvent += Debug.Log;
			xsollaBrowser.Launch
			(
				(int) Viewport.x,
				(int) Viewport.y,
				GetBrowserPlatform(),
				GetBrowserPath()
			);

			xsollaBrowser.Navigate.SetOnPopupListener((popupUrl =>
			{
				xsollaBrowser.Navigate.GetUrl(currentUrl =>
				{
					if (string.IsNullOrEmpty(_urlBeforePopup))
					{
						_urlBeforePopup = currentUrl;
					}
				});
				xsollaBrowser.Navigate.To(popupUrl, newUrl => { BackButton.gameObject.SetActive(true); });
			}));

			xsollaBrowser.SetDialogHandler(HandleBrowserDialog);

			display = this.GetOrAddComponent<Display2DBehaviour>();
		}

		private IEnumerator Start()
		{
			yield return new WaitForEndOfFrame();
			_preloader = gameObject.AddComponent<Preloader2DBehaviour>();
			_preloader.Prefab = PreloaderPrefab;
			yield return new WaitWhile(() => xsollaBrowser.FetchingProgress < 100);

			display.StartRedraw((int) Viewport.x, (int) Viewport.y);
			display.RedrawFrameCompleteEvent += DestroyPreloader;
			display.RedrawFrameCompleteEvent += EnableCloseButton;
			display.ViewportChangedEvent += (width, height) => Viewport = new Vector2(width, height);

			InitializeInput();
			BrowserInitEvent?.Invoke(xsollaBrowser);
		}
		
		private string GetBrowserPlatform()
		{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
#if UNITY_64
			return "Win64";
#else
			return "Win32";
#endif
#endif

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
			return "MacOS";
#endif

#if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
			return "Linux";
#endif
		}

		private string GetBrowserPath()
		{
#if UNITY_EDITOR
			return Application.persistentDataPath;
#else
			if (XsollaSettings.PackInAppBrowserInBuild)
			{
				var path = Application.dataPath;
				path = Directory.GetParent(path).FullName;
				return Path.Combine(path, ".local-chromium");
			}
			else
			{
				return Application.persistentDataPath;
			}
#endif
		}

		private void DestroyPreloader()
		{
			display.RedrawFrameCompleteEvent -= DestroyPreloader;

			if (_preloader != null)
			{
				Destroy(_preloader, 0.001f);
				_preloader = null;
			}
		}

		private void EnableCloseButton()
		{
			display.RedrawFrameCompleteEvent -= EnableCloseButton;

			CloseButton.gameObject.SetActive(true);
			CloseButton.onClick.AddListener(OnCloseButtonPressed);
		}

		private void InitializeInput()
		{
			mouse = this.GetOrAddComponent<Mouse2DBehaviour>();
			keyboard = this.GetOrAddComponent<Keyboard2DBehaviour>();
			keyboard.EscapePressed += OnKeyboardEscapePressed;
		}

		private void OnCloseButtonPressed()
		{
			Debug.Log("`Close` button pressed");
			Destroy(gameObject, 0.001f);
		}

		private void OnBackButtonPressed()
		{
			Debug.Log("`Back` button pressed");
			xsollaBrowser.Navigate.Back((newUrl =>
			{
				if (newUrl.Equals(_urlBeforePopup))
				{
					BackButton.gameObject.SetActive(false);
					_urlBeforePopup = string.Empty;
				}
			}));
		}

		private void OnKeyboardEscapePressed()
		{
			Debug.Log("`Escape` button pressed");
			Destroy(gameObject, 0.001f);
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
			BrowserClosedEvent?.Invoke();

			if (mouse != null)
			{
				Destroy(mouse);
				mouse = null;
			}

			if (display != null)
			{
				Destroy(display);
				display = null;
			}

			if (keyboard != null)
			{
				keyboard.EscapePressed -= OnKeyboardEscapePressed;
				Destroy(keyboard);
				keyboard = null;
			}

			if (xsollaBrowser != null)
			{
				Destroy(xsollaBrowser);
				xsollaBrowser = null;
			}

			Destroy(transform.parent.gameObject);
		}

		private void HandleBrowserDialog(XsollaBrowserDialog dialog)
		{
			switch (dialog.Type)
			{
				case XsollaBrowserDialogType.Alert:
					ShowSimpleAlertPopup(dialog);
					break;
				case XsollaBrowserDialogType.Prompt:
					CloseAlert(dialog);
					break;
				case XsollaBrowserDialogType.Confirm:
					ShowConfirmAlertPopup(dialog);
					break;
				case XsollaBrowserDialogType.BeforeUnload:
					CloseAlert(dialog);
					break;
				default:
					CloseAlert(dialog);
					break;
			}
		}
		
		private void ShowSimpleAlertPopup(XsollaBrowserDialog dialog)
		{
			AlertDialogEvent?.Invoke(dialog.Message, dialog.Accept);
		}
		
		private void ShowConfirmAlertPopup(XsollaBrowserDialog dialog)
		{
			ConfirmDialogEvent?.Invoke(dialog.Message, dialog.Accept, dialog.Dismiss);
		}
		
		private void CloseAlert(XsollaBrowserDialog dialog)
		{
			Debug.Log("Browser alert was closed automatically");
			dialog.Accept();
		}
#endif
	}
}