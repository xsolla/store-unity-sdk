#if (UNITY_EDITOR || UNITY_STANDALONE)
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(XsollaBrowser))]
[RequireComponent(typeof(Image))]
public class Display2DBehaviour : MonoBehaviour
{
	public event Action<int, int> ViewportChangedEvent;
	public event Action RedrawFrameCompleteEvent;

	private IXsollaBrowserRender render;
	private Image image;

	private Canvas canvas;
	private int CanvasWidth => (int) ((canvas.transform as RectTransform).rect.width);//(int)canvas.pixelRect.width;
	private int CanvasHeight => (int) ((canvas.transform as RectTransform).rect.height);//(int)canvas.pixelRect.height;
	
	private Vector2 imageSize;

	public int Width = 0;
	public int Height = 0;

	private void Awake()
	{
		image = gameObject.GetComponent<Image>();
		SetOpacity(image, 0.0F);
		imageSize = Vector2.zero;
		
		canvas = FindObjectOfType<Canvas>();
		if (canvas == null)
		{
			Debug.LogAssertion("Canvas not found. This browser for 2D project.");
			Destroy(gameObject);
			return;
		}
		
		StartCoroutine(InitializeCoroutine());
	}
	
	private void OnDestroy()
	{
		StopRedraw();
	}
	
	public static void SetOpacity(Image image, float opacity)
	{
		if (image != null) {
			Color color = image.color;
			color.a = opacity;
			image.color = color;
		}
	}
	
	public bool StartRedrawWith(int width, int height)
	{
		if (render == null) return false;
		
		StopRedraw();
		
		width = GetActualWidth(width);
		height = GetActualHeight(height);
		
		if (Width != width || Height != height) {
			render.SetResolution(width, height, ViewportCallback);
		} else {
			StartCoroutine(RedrawCoroutine(image));
		}

		return true;
	}

	private IEnumerator InitializeCoroutine()
	{
		yield return new WaitWhile(() => GetComponent<XsollaBrowser>() == null);
		IXsollaBrowser xsollaBrowser = GetComponent<XsollaBrowser>();
		yield return new WaitWhile(() => xsollaBrowser.Render == null);
		render = xsollaBrowser.Render;
	}

	private void StopRedraw()
	{
		SetOpacity(image, 0.0F);
		
		if (render == null) return;
		StopAllCoroutines();
	}
	
	private int GetActualWidth(int width)
	{
		return Mathf.Min(width, (int)(CanvasWidth * 0.9F));
	}
	
	private int GetActualHeight(int height)
	{
		return Mathf.Min(height, (int)(CanvasHeight * 0.9F));
	}

	private void ViewportCallback(int width, int height)
	{
		Width = width;
		Height = height;
		ResizeCollider();
		ViewportChangedEvent?.Invoke(width, height);
		StartCoroutine(RedrawCoroutine(image));
	}
	
	void ResizeCollider()
	{
		BoxCollider2D collider = gameObject.GetComponent<BoxCollider2D>();
		if (collider) {
			RectTransform rectTransform = (RectTransform)(collider.transform);
			collider.size = rectTransform.sizeDelta;
			collider.offset = Vector2.zero;
		}
	}

	private IEnumerator RedrawCoroutine(Image image)
	{
		while (true) {
			yield return ActionExtensions.WaitMethod<Sprite>(render.To, sprite =>
			{
				ApplySpriteToImage(image, sprite);
				RedrawFrameCompleteEvent?.Invoke();
			});
		}
	}

	void ApplySpriteToImage(Image image, Sprite sprite)
	{
		if((image != null) && (sprite != null)) {
			SetOpacity(image, 1.0F);
			image.sprite = sprite;
			image.SetNativeSize();
			ResizeCollider();
		}
	}
}
#endif