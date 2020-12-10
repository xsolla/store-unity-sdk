using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Xsolla.Demo
{
    public class ImageColorDecorator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image image = default;

        [SerializeField] private Color normalStateColor = Color.white;

        [SerializeField] private Color hoverStateColor = Color.white;

        [ContextMenu("Toggle normal state")]
        private void ToggleNormalState()
        {
            image.color = normalStateColor;
        }

        [ContextMenu("Toggle hover state")]
        private void ToggleHoverState()
        {
            image.color = hoverStateColor;
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