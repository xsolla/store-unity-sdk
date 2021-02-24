using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Xsolla.UIBuilder
{
	[AddComponentMenu("Xsolla/UI Builder/Decorator Interactive Events", 200)]
	public class DecoratorPointerEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
	{
		public event Action PointerEnter;

		public event Action PointerExit;

		public event Action PointerDown;

		public event Action PointerUp;

		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => PointerEnter?.Invoke();

		void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => PointerExit?.Invoke();

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData) => PointerDown?.Invoke();

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData) => PointerUp?.Invoke();
	}
}