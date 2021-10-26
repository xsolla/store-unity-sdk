#if (UNITY_EDITOR || UNITY_STANDALONE)
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

namespace Xsolla.Core.Browser
{
	[RequireComponent(typeof(XsollaBrowser))]
	[RequireComponent(typeof(Image))]
	public class Display2DBehaviour : MonoBehaviour
	{
		private IXsollaBrowserRender render;
		private Canvas canvas;
		private RectTransform canvasTransform;
		private Image image;
		
		public Vector2Int CurrentRenderSize { get; private set; }

		public event Action<int, int> ViewportChangedEvent;
		public event Action RedrawFrameCompleteEvent;

		private void Awake()
		{
			canvas = GetComponentInParent<Canvas>();
			if (canvas == null)
			{
				Debug.LogAssertion("Canvas not found. This browser for 2D project.");
				Destroy(gameObject);
				return;
			}

			canvasTransform = canvas.transform as RectTransform;

			image = gameObject.GetComponent<Image>();
			SetOpacity(0.0f);

			StartCoroutine(InitializeCoroutine());
		}

		private void OnDestroy()
		{
			StopRedraw();
		}

		private void SetOpacity(float opacity)
		{
			if (image == null)
				return;

			var color = image.color;
			color.a = opacity;
			image.color = color;
		}

		public void StartRedraw(int width, int height)
		{
			if (render == null)
				return;

			StopRedraw();

			var canvasSize = canvasTransform.rect.size;
			var clampedSize = new Vector2Int(
				Mathf.Min(width, (int) canvasSize.x),
				Mathf.Min(height, (int) canvasSize.y)
			);

			if (CurrentRenderSize != clampedSize)
				render.SetResolution(width, height, ViewportCallback);
			else
				StartCoroutine(RedrawCoroutine());
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
			SetOpacity(0.0f);

			if (render != null)
				StopAllCoroutines();
		}

		private void ViewportCallback(int width, int height)
		{
			CurrentRenderSize = new Vector2Int(width, height);
			ViewportChangedEvent?.Invoke(width, height);
			StartCoroutine(RedrawCoroutine());
		}

		private IEnumerator RedrawCoroutine()
		{
			while (enabled)
			{
				yield return ActionExtensions.WaitMethod<Sprite>(render.To, sprite =>
				{
					ApplySpriteToImage(sprite);
					RedrawFrameCompleteEvent?.Invoke();
				});
			}
		}

		private void ApplySpriteToImage(Sprite sprite)
		{
			if (image == null || sprite == null)
				return;

			SetOpacity(1.0f);
			image.sprite = sprite;
			image.SetNativeSize();
		}
	}
}
#endif