using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Drawing;

[RequireComponent(typeof(XsollaBrowser))]
[RequireComponent(typeof(Image))]
public class Display2DBehaviour : MonoBehaviour
{
	public event Action<Size> ViewportChangedEvent;

	private IXsollaBrowserRender render;
	private Image image;

	private Size viewportSize = new Size(0, 0);
	public Size ViewportSize { get => viewportSize; set => viewportSize = value; }

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

	public void StartRedrawTo(Vector2 viewport)
	{
		StopRedraw();
		if (!viewportSize.Equals(viewport)) {
			Size newSize = new Size((int)viewport.x, (int)viewport.y);
			render.SetResolution(newSize, ViewportCallback);
		} else {
			StartCoroutine(RedrawCoroutine(image));
		}
	}

	private void ViewportCallback(Size newSize)
	{
		viewportSize = newSize;
		ResizeCollider();
		ViewportChangedEvent?.Invoke(viewportSize);
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
			UnityEngine.Color color = image.color;
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
