using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SelectableArea : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    public event Action OnPointerEnterEvent;
    public event Action OnPointerExitEvent;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
		if (OnPointerEnterEvent != null)
			OnPointerEnterEvent.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
		if (OnPointerExitEvent != null)
			OnPointerExitEvent.Invoke();
    }
}
