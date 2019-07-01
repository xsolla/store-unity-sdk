using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelElementVisualize : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPanelVisualElement
{
    [SerializeField] private Text text;
    [SerializeField] private Image line_Image;
    [SerializeField] private Color selected_Color;
    [SerializeField] private Color hover_Color;
    [SerializeField] private Color normal_Color;
    [SerializeField] private Sprite selected_Sprite;
    [SerializeField] private Sprite hover_Sprite;
    [SerializeField] private Sprite normal_Sprite;

    private bool isSelected;

    public void Select()
    {
        line_Image.sprite = selected_Sprite;
        text.color = selected_Color;
        isSelected = true;
    }
    public void Deselect()
    {
        line_Image.sprite = normal_Sprite;
        text.color = normal_Color;
        isSelected = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
        {
            text.color = normal_Color;
            line_Image.sprite = normal_Sprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected)
        {
            text.color = hover_Color;
            line_Image.sprite = hover_Sprite;
        }
    }
}
