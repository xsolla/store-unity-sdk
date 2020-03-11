using System;
using System.Collections;
using UnityEngine;

public class SinglePageBrowser2D : MonoBehaviour
{
	public event Action<SinglePageBrowser2D> BrowserClosedEvent;

	[SerializeField]
	private string url = "https://unity3d.com";
	[SerializeField]
	public Vector2 Viewport = new Vector2(800.0F, 600.0F);
	[SerializeField]
	public GameObject PreloaderPrefab;
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

		StartCoroutine(InitializationCoroutine());
	}

	private void Start()
	{
		Camera.main.transform.Translate(transform.position - Camera.main.transform.position, Space.World);
	}

	IEnumerator InitializationCoroutine()
	{
		yield return StartCoroutine(WaitPreloaderCoroutine());

		display.StartRedrawWith((int)Viewport.x, (int)Viewport.y);
		InitializeInput();
	}

	IEnumerator WaitPreloaderCoroutine()
	{
		gameObject.AddComponent<Preloader2DBehaviour>().SetPreloaderPrefab(PreloaderPrefab);
		yield return new WaitWhile(() => gameObject.GetComponent<Preloader2DBehaviour>() != null);
	}

	void InitializeInput()
	{
		mouse = this.GetOrAddComponent<MouseBehaviour2D>();
		keyboard = this.GetOrAddComponent<KeyboardBehaviour2D>();
		keyboard.EscapePressed += Keyboard_EscapePressed;
	}

	private void Keyboard_EscapePressed()
	{
		keyboard.EscapePressed -= Keyboard_EscapePressed;
		Debug.Log("`Escape` button pressed");
		BrowserClosedEvent?.Invoke(this);
		Destroy(gameObject, 0.001F);
	}

	private void OnDestroy()
	{
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
