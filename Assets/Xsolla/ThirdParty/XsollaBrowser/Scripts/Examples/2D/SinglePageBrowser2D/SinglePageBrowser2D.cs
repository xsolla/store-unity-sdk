using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class SinglePageBrowser2D : MonoBehaviour
{
	public event Action<SinglePageBrowser2D> BrowserClosedEvent;

	[SerializeField]
	private string url = "https://unity3d.com";
	[SerializeField]
	private Image image;
	public Image Image {
		get => image;
		set {
			image = value;
			if (display) {
				display.Image = image;
			}
		}
	}
	[SerializeField]
	public Vector2 Viewport = new Vector2(800.0F, 600.0F);

	XsollaBrowser xsollaBrowser;
	Display2DBehaviour display;
	KeyboardBehaviour2D keyboard;
	MouseBehaviour2D mouse;

	int lastProgress = 0;
	object progressLocker;

	private void Awake()
	{
		progressLocker = new object();
		lastProgress = 0;
		Display2DBehaviour.SetOpacity(image, 0.0F);
		xsollaBrowser = this.GetOrAddComponent<XsollaBrowser>();
		xsollaBrowser.LogEvent += XsollaBrowser_LogEvent;
		xsollaBrowser.FetchingBrowserEvent += XsollaBrowser_FetchingBrowserEvent;
		xsollaBrowser.Navigate.To(url, (string url) => InitializeRender());
	}

	private void XsollaBrowser_FetchingBrowserEvent(int progress)
	{
		lock (progressLocker) {
			if (lastProgress < progress) {
				Debug.Log(string.Format("Update[%]: {0} => {1}", lastProgress, progress));
				lastProgress = progress;
			}
		}
	}

	private void Start()
	{
		Camera.main.transform.Translate(transform.position - Camera.main.transform.position, Space.World);
	}

	void InitializeRender()
	{
		xsollaBrowser.FetchingBrowserEvent -= XsollaBrowser_FetchingBrowserEvent;
		display = this.GetOrAddComponent<Display2DBehaviour>();
		display.ViewportChangedEvent += Display2D_ViewportChangedEvent;
		display.SetViewPort(Viewport);
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

	private void Display2D_ViewportChangedEvent(Size obj)
	{
		display.ViewportChangedEvent -= Display2D_ViewportChangedEvent;
		if (image) {
			display.Image = image;
		}
		InitializeInput();
	}

	private void OnDestroy()
	{
		if (display != null) {
			Destroy(display);
			display = null;
		}
		if (mouse != null) {
			Destroy(mouse);
			mouse = null;
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
}
