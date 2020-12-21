using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Core.Browser
{
	[RequireComponent(typeof(Image))]
	[RequireComponent(typeof(BoxCollider2D))]
	public class SinglePageBrowser2D : MonoBehaviour
	{
		[SerializeField] private Vector2 Viewport = new Vector2(1920.0F, 1080.0F);
		[SerializeField] private GameObject PreloaderPrefab = default;

		public Button CloseButton;
#if UNITY_EDITOR || UNITY_STANDALONE
		public event Action<IXsollaBrowser> BrowserInitEvent;
		public event Action<IXsollaBrowser> BrowserClosedEvent;

		XsollaBrowser xsollaBrowser;
		Display2DBehaviour display;
		KeyboardBehaviour2D keyboard;
		MouseBehaviour2D mouse;
	
		private void Awake()
		{
			CloseButton.gameObject.SetActive(false);
		
			Canvas canvas = FindObjectOfType<Canvas>();
			Rect canvasRect = (canvas.transform as RectTransform).rect;//canvas.pixelRect;
		
			if (Viewport.x > canvasRect.width)
				Viewport.x = canvasRect.width * 0.9F;
			if (Viewport.y > canvasRect.height)
				Viewport.y = canvasRect.height * 0.9F;
		
			xsollaBrowser = this.GetOrAddComponent<XsollaBrowser>();
			xsollaBrowser.LogEvent += XsollaBrowser_LogEvent;
			xsollaBrowser.Launch(new LaunchBrowserOptions()
			{
				Width = (int)Viewport.x,
				Height = (int)Viewport.y,
			});
		
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
		
			display.StartRedrawWith((int)Viewport.x, (int)Viewport.y);
			display.RedrawFrameCompleteEvent += EnableCloseButton;
			display.ViewportChangedEvent += (width, height) => Viewport = new Vector2(width, height);
			InitializeInput();
			BrowserInitEvent?.Invoke(browser);
		}

		private void EnableCloseButton()
		{
			display.RedrawFrameCompleteEvent -= EnableCloseButton;
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

		private void Keyboard_EscapePressed()
		{
			Debug.Log("`Escape` button pressed");
			Destroy(gameObject, 0.001F);
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
			BrowserClosedEvent?.Invoke(xsollaBrowser);
			if (mouse != null) {
				Destroy(mouse);
				mouse = null;
			}
			if (display != null) {
				Destroy(display);
				display = null;
			}
			if (keyboard != null) {
				keyboard.EscapePressed -= Keyboard_EscapePressed;
				Destroy(keyboard);
				keyboard = null;
			}
			if (xsollaBrowser != null) {
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
