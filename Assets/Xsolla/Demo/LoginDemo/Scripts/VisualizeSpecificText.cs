using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VisualizeSpecificText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color normal_Color;
    [SerializeField] private Color hover_Color;
    [SerializeField] private Text text;

    private void OnEnable()
    {
        text.color = normal_Color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = hover_Color;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = normal_Color;
    }
}
