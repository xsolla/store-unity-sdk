using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class VisualizeToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image target_Image;
    private Toggle target_Toggle;

    [SerializeField] private Sprite selected_Sprite;
    [SerializeField] private Sprite deselected_Sprite;
    [SerializeField] private Sprite hover_Sprite;

    private void Awake()
    {
        target_Toggle = GetComponent<Toggle>();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!target_Toggle.isOn)
            target_Image.sprite = deselected_Sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!target_Toggle.isOn)
            target_Image.sprite = hover_Sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (target_Toggle.isOn)
            target_Image.sprite = selected_Sprite;
        else
            target_Image.sprite = hover_Sprite;
    }
}
