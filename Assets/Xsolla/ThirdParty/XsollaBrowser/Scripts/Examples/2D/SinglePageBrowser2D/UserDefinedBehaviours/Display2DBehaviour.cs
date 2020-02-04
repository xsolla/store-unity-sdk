using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Drawing;

[RequireComponent(typeof(XsollaBrowser))]
public class Display2DBehaviour : MonoBehaviour
{
	public event Action<Size> ViewportChangedEvent;
	public Image Image { get; set; }

	private IXsollaBrowserRender render;

	private Size viewportSize = new Size(0, 0);
	public  Size ViewportSize
	{
		get => viewportSize;
		set => render.SetResolution(value, (Size size) => {
			viewportSize = size;
			ViewportChangedEvent?.Invoke(viewportSize);
		});
	}
	
	private void Awake()
	{
		IXsollaBrowser xsollaBrowser = gameObject.GetComponent<XsollaBrowser>();
		render = xsollaBrowser.Render;
		ViewportChangedEvent += DisplayBehaviour_ViewportChangedEvent;
	}

	private void Start()
	{
		StartCoroutine(RedrawCoroutine());
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
		SetOpacity(Image, 0.0F);
	}

	public void SetViewPort(Vector2 viewport)
	{
		ViewportSize = new Size((int)viewport.x, (int)viewport.y);
	}

	private void DisplayBehaviour_ViewportChangedEvent(Size obj)
	{
		ResizeCollider();
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

	IEnumerator RedrawCoroutine()
	{
		while (true) {
			yield return new WaitWhile(ImageIsNull);
			yield return ActionExtensions.WaitMethod<Sprite>(render.To, ApplySpriteToImage);
		}
	}

	bool ImageIsNull()
	{
		return Image == null;
	}

	void ApplySpriteToImage(Sprite sprite)
	{
		if(Image != null && sprite != null) {
			SetOpacity(Image, 1.0F);
			Image.sprite = sprite;
			Image.SetNativeSize();
			ResizeCollider();
		}
	}
}
