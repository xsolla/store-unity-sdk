using System;
using System.Collections;
using UnityEngine;

public class SinglePageBrowser2D : MonoBehaviour
{
	public event Action<IXsollaBrowser> BrowserInitEvent;
	public event Action<IXsollaBrowser> BrowserClosedEvent;
	public event Action<IXsollaBrowser, string> UrlChangedEvent;
	
	[SerializeField]
	public Vector2 Viewport = new Vector2(800.0F, 600.0F);
	[SerializeField]
	public GameObject PreloaderPrefab;
#if (UNITY_EDITOR || UNITY_STANDALONE)
	XsollaBrowser xsollaBrowser;
	Display2DBehaviour display;
	KeyboardBehaviour2D keyboard;
	MouseBehaviour2D mouse;

	/// <summary>
	/// Url variable for UrlChangeCoroutine
	/// </summary>
	private string url;

	private void Awake()
	{
		xsollaBrowser = this.GetOrAddComponent<XsollaBrowser>();
		xsollaBrowser.LogEvent += XsollaBrowser_LogEvent;
		display = this.GetOrAddComponent<Display2DBehaviour>();		

		StartCoroutine(InitializationCoroutine(xsollaBrowser));
	}

	private void Start()
	{
		Camera.main.transform.Translate(transform.position - Camera.main.transform.position, Space.World);
	}

	IEnumerator InitializationCoroutine(IXsollaBrowser browser)
	{
		yield return StartCoroutine(WaitPreloaderCoroutine());
		
		display.StartRedrawWith((int)Viewport.x, (int)Viewport.y);
		InitializeInput();
		StartCoroutine(UrlChangeCoroutine(browser));
		BrowserInitEvent?.Invoke(browser);
	}

	private IEnumerator WaitPreloaderCoroutine()
	{
		gameObject.AddComponent<Preloader2DBehaviour>().SetPreloaderPrefab(PreloaderPrefab);
		yield return new WaitWhile(() => gameObject.GetComponent<Preloader2DBehaviour>() != null);
	}
	
	private IEnumerator UrlChangeCoroutine(IXsollaBrowser browser)
	{
		while (true)
		{
			yield return ActionExtensions.WaitMethod<string>(browser.Navigate.GetUrl, newUrl =>
			{
				if (url.Equals(newUrl)) return;
				url = newUrl;
				UrlChangedEvent?.Invoke(browser, url);
			});
			yield return new WaitForSeconds(0.2F);
		}
	}

	private void InitializeInput()
	{
		mouse = this.GetOrAddComponent<MouseBehaviour2D>();
		keyboard = this.GetOrAddComponent<KeyboardBehaviour2D>();
		keyboard.EscapePressed += Keyboard_EscapePressed;
	}

	private void Keyboard_EscapePressed()
	{
		keyboard.EscapePressed -= Keyboard_EscapePressed;
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
			Destroy(keyboard);
			keyboard = null;
		}
		if (xsollaBrowser != null) {
			Destroy(xsollaBrowser);
			xsollaBrowser = null;
		}
	}

	private void XsollaBrowser_LogEvent(string obj)
	{
		Debug.Log(obj);
	}
#endif
}
