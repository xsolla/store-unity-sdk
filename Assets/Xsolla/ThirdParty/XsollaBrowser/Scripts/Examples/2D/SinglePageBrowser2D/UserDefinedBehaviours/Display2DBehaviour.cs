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

	private IXsollaBrowserRender render;
	private Image image;

	public int Width = 0;
	public int Height = 0;

	private void Awake()
	{
		image = gameObject.GetComponent<Image>();
		SetOpacity(image, 0.0F);

		IXsollaBrowser xsollaBrowser = gameObject.GetComponent<XsollaBrowser>();
		render = xsollaBrowser.Render;
	}

	private void OnDestroy()
	{
		StopRedraw();
	}

	public void StartRedrawWith(int width, int height)
	{
		StopRedraw();
		if (Width != width || Height != height) {
			render.SetResolution(width, height, ViewportCallback);
		} else {
			StartCoroutine(RedrawCoroutine(image));
		}
	}

	private void ViewportCallback(int width, int height)
	{
		Width = width;
		Height = height;
		ResizeCollider();
		ViewportChangedEvent?.Invoke(width, height);
		StartCoroutine(RedrawCoroutine(image));
	}

	public void StopRedraw()
	{
		SetOpacity(image, 0.0F);
		StopAllCoroutines();
	}

	public static void SetOpacity(Image image, float opacity)
	{
		if (image != null) {
			Color color = image.color;
			color.a = opacity;
			image.color = color;
		}
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

	IEnumerator RedrawCoroutine(Image image)
	{
		while (true) {
			yield return ActionExtensions.WaitMethod<Sprite>(
				render.To,
				(Sprite sprite) => ApplySpriteToImage(image, sprite)
			);
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