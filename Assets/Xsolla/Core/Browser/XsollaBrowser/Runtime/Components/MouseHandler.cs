using System.Collections;
using System.Threading;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.XsollaBrowser
{
	public class MouseHandler : MonoBehaviour, IBrowserHandler
	{
		[SerializeField] private BrowserImage Image;
		[SerializeField] private RectTransform ViewportRect;
		private readonly float WheelScrollSpeed = 10;
		private BrowserPage Page;
		private bool IsPointerEntered;

		public void Run(BrowserPage page, CancellationToken cancellationToken)
		{
			Page = page;

			Image.PointerEnter += OnPointerEnter;
			Image.PointerExit += OnPointerExit;
			Image.PointerClick += OnPointerClick;

			StartCoroutine(TrackMousePositionLoop(cancellationToken));
			StartCoroutine(TrackScrollWheelLoop(cancellationToken));
		}

		public void Stop()
		{
			Image.PointerEnter -= OnPointerEnter;
			Image.PointerExit -= OnPointerExit;
			Image.PointerClick -= OnPointerClick;

			StopAllCoroutines();
		}

		private void OnPointerEnter()
		{
			IsPointerEntered = true;
		}

		private void OnPointerExit()
		{
			IsPointerEntered = false;
		}

		private void OnPointerClick()
		{
			var pointerPosition = InputProvider.GetCursorPosition();
			Page.Click(ConvertToBrowserCoordinate(pointerPosition));
		}

		private IEnumerator TrackMousePositionLoop(CancellationToken cancellationToken)
		{
			var lastMousePosition = Vector2.zero;

			while (!cancellationToken.IsCancellationRequested)
			{
				if (IsPointerEntered)
				{
					var mousePosition = InputProvider.GetCursorPosition();
					var positionDelta = mousePosition - lastMousePosition;
					if (Vector2.SqrMagnitude(positionDelta) > 0)
					{
						Page.MoveCursorAsync(ConvertToBrowserCoordinate(mousePosition));
						lastMousePosition = mousePosition;
					}
				}

				yield return null;
			}
		}

		private IEnumerator TrackScrollWheelLoop(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				var scroll = InputProvider.GetScrollDelta();
				if (scroll != Vector2.zero)
				{
					var offset = new Vector2(
						scroll.x * WheelScrollSpeed,
						-scroll.y * WheelScrollSpeed);

					Page.ScrollWheel(offset);
				}

				yield return null;
			}
		}

		private Vector2 ConvertToBrowserCoordinate(Vector2 mousePosition)
		{
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(ViewportRect, mousePosition, null, out var point))
				return Vector2.zero;

			point += ViewportRect.sizeDelta * 0.5f; // Transform's anchors in the center of the rect
			point.y = ViewportRect.sizeDelta.y - point.y; // Browser's y-axis is opposite to Unity's y-axis
			return new Vector2(point.x, point.y);
		}
	}
}