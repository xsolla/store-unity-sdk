using System;
using System.Collections;
using PuppeteerSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Core.Browser
{
	[RequireComponent(typeof(Image))]
	[RequireComponent(typeof(BoxCollider2D))]
	public class SinglePageBrowser2D : MonoBehaviour
	{
#if UNITY_EDITOR || UNITY_STANDALONE
		[SerializeField] private Vector2 Viewport = new Vector2(1920.0F, 1080.0F);
		[SerializeField] private GameObject PreloaderPrefab = default;

		public Button CloseButton;
		public Button BackButton;
		public event Action<IXsollaBrowser> BrowserInitEvent;
		public event Action<IXsollaBrowser> BrowserClosedEvent;

		XsollaBrowser xsollaBrowser;
		Display2DBehaviour display;
		KeyboardBehaviour2D keyboard;
		MouseBehaviour2D mouse;

		string _urlBeforePopup;

		private void Awake()
		{
			BackButton.onClick.AddListener(BackButtonPressed);

			CloseButton.gameObject.SetActive(false);
			BackButton.gameObject.SetActive(false);

			Canvas canvas = FindObjectOfType<Canvas>();
			Rect canvasRect = (canvas.transform as RectTransform).rect; //canvas.pixelRect;

			if (Viewport.x > canvasRect.width)
				Viewport.x = canvasRect.width * 0.9F;
			if (Viewport.y > canvasRect.height)
				Viewport.y = canvasRect.height * 0.9F;

			xsollaBrowser = this.GetOrAddComponent<XsollaBrowser>();
			xsollaBrowser.LogEvent += XsollaBrowser_LogEvent;
			xsollaBrowser.Launch
			(
				new LaunchBrowserOptions
				{
					Width = (int) Viewport.x,
					Height = (int) Viewport.y,
				},
				new BrowserFetcherOptions
				{
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
				xsollaBrowser.Navigate.GetUrl(currentUrl => { _urlBeforePopup = currentUrl; });
				xsollaBrowser.Navigate.To(popupUrl, newUrl => { BackButton.gameObject.SetActive(true); });
			}));

			display = this.GetOrAddComponent<Display2DBehaviour>();
		}

		private void Start()
		{
			if (Camera.main != null)
				Camera.main.transform.Translate(transform.position - Camera.main.transform.position, Space.World);
			StartCoroutine(InitializationCoroutine(xsollaBrowser));
		}

		IEnumerator InitializationCoroutine(IXsollaBrowser browser)
		{
			yield return new WaitForEndOfFrame();
			yield return StartCoroutine(WaitPreloaderCoroutine());

			display.StartRedrawWith((int) Viewport.x, (int) Viewport.y);
			display.RedrawFrameCompleteEvent += EnableButtons;
			display.ViewportChangedEvent += (width, height) => Viewport = new Vector2(width, height);
			InitializeInput();
			BrowserInitEvent?.Invoke(browser);
		}

		private void EnableButtons()
		{
			display.RedrawFrameCompleteEvent -= EnableButtons;

			CloseButton.gameObject.SetActive(true);
			CloseButton.onClick.AddListener(CloseButtonPressed);
		}

		private IEnumerator WaitPreloaderCoroutine()
		{
			gameObject.AddComponent<Preloader2DBehaviour>().SetPreloaderPrefab(PreloaderPrefab);
			yield return new WaitWhile(() => gameObject.GetComponent<Preloader2DBehaviour>() != null);
		}

		private void InitializeInput()
		{
			mouse = this.GetOrAddComponent<MouseBehaviour2D>();
			keyboard = this.GetOrAddComponent<KeyboardBehaviour2D>();
			keyboard.EscapePressed += Keyboard_EscapePressed;
		}

		private void CloseButtonPressed()
		{
			Debug.Log("`Close` button pressed");
			Destroy(gameObject, 0.001F);
		}

		private void BackButtonPressed()
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

		private void Keyboard_EscapePressed()
		{
			Debug.Log("`Escape` button pressed");
			Destroy(gameObject, 0.001F);
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
			BrowserClosedEvent?.Invoke(xsollaBrowser);
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
				keyboard.EscapePressed -= Keyboard_EscapePressed;
				Destroy(keyboard);
				keyboard = null;
			}

			if (xsollaBrowser != null)
			{
				Destroy(xsollaBrowser);
				xsollaBrowser = null;
			}

			Destroy(this.transform.parent.gameObject);
		}

		private void XsollaBrowser_LogEvent(string obj)
		{
			Debug.Log(obj);
		}
#endif
	}
}