using System;
using System.Collections;
using PuppeteerSharp;
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
				new LaunchBrowserOptions{
					Width = (int) Viewport.x,
					Height = (int) Viewport.y,
				},
				new BrowserFetcherOptions{
#if UNITY_STANDALONE && !UNITY_EDITOR
					Path = !XsollaSettings.PackInAppBrowserInBuild ? Application.persistentDataPath : String.Empty,
#endif

#if UNITY_EDITOR
					Path = Application.persistentDataPath,
#endif

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
#if UNITY_64
					Platform = Platform.Win64
#else
					Platform = Platform.Win32
#endif
#endif

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
					Platform = Platform.MacOS
#endif

#if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
					Platform = Platform.Linux
#endif
				}
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

			xsollaBrowser.Navigate.SetOnAlertListener(HandleBrowserAlert);

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

		private void HandleBrowserAlert(Dialog alert)
		{
			switch (alert.DialogType)
			{
				case DialogType.Alert:
					ShowSimpleAlertPopup(alert);
					break;
				case DialogType.Prompt:
					CloseAlert(alert);
					break;
				case DialogType.Confirm:
					ShowConfirmAlertPopup(alert);
					break;
				case DialogType.BeforeUnload:
					CloseAlert(alert);
					break;
				default:
					CloseAlert(alert);
					break;
			}
		}

		private void ShowSimpleAlertPopup(Dialog dialog)
		{
			AlertDialogEvent?.Invoke(dialog.Message, () => dialog.Accept());
		}

		private void ShowConfirmAlertPopup(Dialog dialog)
		{
			ConfirmDialogEvent?.Invoke(dialog.Message, () => dialog.Accept(), () => dialog.Dismiss());
		}

		private void CloseAlert(Dialog dialog)
		{
			Debug.Log("Browser alert was closed automatically");
			dialog.Accept();
		}
#endif
	}
}