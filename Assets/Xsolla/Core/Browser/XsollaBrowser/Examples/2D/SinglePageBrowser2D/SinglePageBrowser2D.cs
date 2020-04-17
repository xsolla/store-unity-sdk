using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SinglePageBrowser2D : MonoBehaviour
{
	public event Action<IXsollaBrowser> BrowserInitEvent;
	public event Action<IXsollaBrowser> BrowserClosedEvent;

	[SerializeField]
	public Vector2 Viewport = new Vector2(800.0F, 600.0F);
	[SerializeField]
	public GameObject PreloaderPrefab;

	public Button CloseButton;
#if (UNITY_EDITOR || UNITY_STANDALONE)
	XsollaBrowser xsollaBrowser;
	Display2DBehaviour display;
	KeyboardBehaviour2D keyboard;
	MouseBehaviour2D mouse;
	
	private void Awake()
	{
		xsollaBrowser = this.GetOrAddComponent<XsollaBrowser>();
		xsollaBrowser.LogEvent += XsollaBrowser_LogEvent;
		display = this.GetOrAddComponent<Display2DBehaviour>();
	}

	private void Start()
	{
		CloseButton.gameObject.SetActive(false);
		if (Camera.main != null)
			Camera.main.transform.Translate(transform.position - Camera.main.transform.position, Space.World);
		StartCoroutine(InitializationCoroutine(xsollaBrowser));
	}

	IEnumerator InitializationCoroutine(IXsollaBrowser browser)
	{
		yield return new WaitForEndOfFrame();
		yield return StartCoroutine(WaitPreloaderCoroutine());
		
		display.StartRedrawWith((int)Viewport.x, (int)Viewport.y);
		display.RedrawFrameCompleteEvent += () =>
		{
			CloseButton.gameObject.SetActive(true);
			CloseButton.onClick.AddListener(CloseButtonPressed);
		};
		InitializeInput();
		BrowserInitEvent?.Invoke(browser);
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
		keyboard.EscapePressed -= Keyboard_EscapePressed;
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
