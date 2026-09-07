using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Xsolla.Demo
{
	public class WidgetEventsProvider : MonoBehaviour, IPointerDownHandler
	{
		public event Action<PointerEventData> PointerDown;

		public void OnPointerDown(PointerEventData eventData)
		{
			PointerDown?.Invoke(eventData);
		}
	}
}