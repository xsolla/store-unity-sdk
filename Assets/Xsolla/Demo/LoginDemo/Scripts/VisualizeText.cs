using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class VisualizeText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color normal_Color;
    [SerializeField] private Color hover_Color;
    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
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
