#if (UNITY_EDITOR || UNITY_STANDALONE)
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace Xsolla.Core.Browser
{
	[RequireComponent(typeof(XsollaBrowser))]
	[RequireComponent(typeof(Display2DBehaviour))]
	public class MouseBehaviour2D : MonoBehaviour,
		IPointerEnterHandler,
		IPointerExitHandler,
		IPointerClickHandler,
		IScrollHandler
	{
		private BoxCollider2D browserCollider;
		private IXsollaBrowserMouseInput browserMouse;
		private Display2DBehaviour display;

		Coroutine mouseMovementCoroutine;
		Canvas canvas;
		Camera canvasCamera;
		
		public int ScrollSpeed { get; set; } = -75;

		private void Awake()
		{
			canvas = FindObjectOfType<Canvas>();
			canvasCamera = canvas.worldCamera;

			browserCollider = this.GetOrAddComponent<BoxCollider2D>();
			browserMouse = gameObject.GetComponent<XsollaBrowser>().Input.Mouse;
			display = gameObject.GetComponent<Display2DBehaviour>();
		}
	

		private void OnDestroy()
		{
			StopAllCoroutines();
		}
	
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			if (mouseMovementCoroutine != null) {
				StopCoroutine(mouseMovementCoroutine);
			}
			mouseMovementCoroutine = StartCoroutine(MouseCoroutine());
		}

		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			if (mouseMovementCoroutine != null) {
				StopCoroutine(mouseMovementCoroutine);
				mouseMovementCoroutine = null;
			}
		}

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			Vector2 mousePosition = GetMousePosition(eventData.position);
			browserMouse.Click(mousePosition, (Vector2 pos) => Debug.Log("Click handled by: " + pos.ToString()));
		}
		
		void IScrollHandler.OnScroll(PointerEventData eventData)
		{
			var scrollDelta = eventData.scrollDelta * ScrollSpeed;
			browserMouse.Scroll(scrollDelta);
		}

		IEnumerator MouseCoroutine()
		{
			Vector2 lastPosition = Vector2.zero;
			while (true) {
				yield return Display2DInitializationCoroutine();
				Vector2 mousePosition = GetMousePosition(InputProxy.MousePosition);
				yield return MouseMovementCoroutine(lastPosition, mousePosition, (Vector2 pos) => lastPosition = pos);
			}
		}

		IEnumerator Display2DInitializationCoroutine()
		{
			yield return new WaitWhile(() => (display.Width == 0) || (display.Height == 0)); 
		}

		IEnumerator MouseMovementCoroutine(Vector2 lastPosition, Vector2 mousePosition, Action<Vector2> callback = null)
		{
			if ((mousePosition - lastPosition).magnitude > 1.0F) {
				yield return ActionExtensions.WaitMethod(
					browserMouse.MoveTo, mousePosition, (Vector2 pos) => callback?.Invoke(pos)
				);
			} else {
				yield return new WaitForFixedUpdate();
			}
		}

		Vector2 GetMousePosition(Vector3 inputMousePosition)
		{
			RectTransform rect = (RectTransform)(canvas.transform);
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, inputMousePosition, canvasCamera, out Vector2 canvasPoint)) {
				rect = (RectTransform)transform;
				Vector2 offset = canvasPoint - ((Vector2)rect.localPosition);
				Vector2 leftUpperCorner = rect.sizeDelta / 2;
				Vector2 point = leftUpperCorner + offset;
				return ConvertCoordinatesToPixels(point);
			} else {
				Debug.LogWarning("You try get mouse position, but mouse not over canvas");
			}
			return Vector2.zero;
		}

		Vector2 ConvertCoordinatesToPixels(Vector3 mousePositionOverScreen)
		{
			if (!mousePositionOverScreen.Equals(Vector3.zero)) {
				float x = (mousePositionOverScreen.x / browserCollider.size.x) * display.Width;
				float y = (mousePositionOverScreen.y / browserCollider.size.y) * display.Height;
				y = display.Height - y; // Because browser's axis are differs from Unity's axis
				return new Vector2(x, y);
			}
			return Vector2.zero;
		}
	}
}
#endif