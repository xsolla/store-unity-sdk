using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class VisualizeToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] Image target_Image;
    Toggle target_Toggle;

    [SerializeField] Text terget_Text;

    [SerializeField] Sprite selected_Sprite;
    [SerializeField] Sprite deselected_Sprite;
    [SerializeField] Sprite hover_Sprite;
    
    [SerializeField] Color normal_Color;
    [SerializeField] Color hover_Color;

    private void Awake()
    {
        target_Toggle = GetComponent<Toggle>();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!target_Toggle.isOn)
            target_Image.sprite = deselected_Sprite;
        
        terget_Text.color = normal_Color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!target_Toggle.isOn)
            target_Image.sprite = hover_Sprite;
        
        terget_Text.color = hover_Color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (target_Toggle.isOn)
            target_Image.sprite = selected_Sprite;
        else
            target_Image.sprite = hover_Sprite;
    }
}
