using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class InputFieldCursorEventProvider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public static event Action OnCursorEnter;
	public static event Action OnCursorExit;

	public void OnPointerEnter(PointerEventData eventData) => OnCursorEnter?.Invoke();
	public void OnPointerExit(PointerEventData eventData) => OnCursorExit?.Invoke();
}
