using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class TextColorDecorator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField] private Text text = default;

		[SerializeField] private Color normalStateColor = Color.white;

		[SerializeField] private Color hoverStateColor = new Color32(255, 0, 91, 255);

		[ContextMenu("Toggle normal state")]
		private void ToggleNormalState()
		{
			text.color = normalStateColor;
		}

		[ContextMenu("Toggle hover state")]
		private void ToggleHoverState()
		{
			text.color = hoverStateColor;
		}

		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			ToggleHoverState();
		}

		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			ToggleNormalState();
		}
	}
}