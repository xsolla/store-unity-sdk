using UnityEngine;
using System.Collections;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(XsollaBrowser))]
public class MouseBehaviour2D : MonoBehaviour,
	IPointerEnterHandler,
	IPointerExitHandler,
	IPointerClickHandler
{
	private BoxCollider2D browserCollider;
    private IXsollaBrowserMouseInput browserMouse;

    Coroutine mouseMovementCoroutine;
    Canvas canvas;
    Camera canvasCamera;

	private void Awake()
	{
        canvas = FindObjectOfType<Canvas>();
        canvasCamera = canvas.worldCamera;

        browserCollider = this.GetOrAddComponent<BoxCollider2D>();
        browserMouse = gameObject.GetComponent<XsollaBrowser>().Input.Mouse;
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
        }
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        Vector2 mousePosition = GetMousePosition(eventData.position);
        browserMouse.Click(mousePosition, (Vector2 pos) => Debug.Log("Click handled by: " + pos.ToString()));
    }

    IEnumerator MouseCoroutine()
    {
        Vector2 lastPosition = Vector2.zero;
		while (true) {
            yield return Display2DInitializationCoroutine();
            Vector2 mousePosition = GetMousePosition(Input.mousePosition);
            yield return MouseMovementCoroutine(lastPosition, mousePosition, (Vector2 pos) => lastPosition = pos);
        }
    }

    IEnumerator Display2DInitializationCoroutine()
    {
        yield return new WaitWhile(() => gameObject.GetComponent<Display2DBehaviour>() == null);
        Display2DBehaviour display = gameObject.GetComponent<Display2DBehaviour>();
        yield return new WaitWhile(() => display.ViewportSize.IsEmpty);
        yield return new WaitWhile(() => display.Image == null);
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
        Display2DBehaviour display = gameObject.GetComponent<Display2DBehaviour>();
        if (!mousePositionOverScreen.Equals(Vector3.zero) && (display != null)) {
            PointF point = new PointF {
                X = (mousePositionOverScreen.x / browserCollider.size.x) * display.ViewportSize.Width,
				Y = (mousePositionOverScreen.y / browserCollider.size.y) * display.ViewportSize.Height
			};
            point.Y = display.ViewportSize.Height - point.Y; // Because browser's axis are differs from Unity's axis
            return point.ToVector();
        }
        return Vector2.zero;
    }
}
