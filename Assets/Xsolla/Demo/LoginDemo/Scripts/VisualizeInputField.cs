using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(InputField), typeof(Image))]
public class VisualizeInputField : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Image target_Image;
    private InputField target_InputField;

    [SerializeField] private Sprite selected_Sprite;
    [SerializeField] private Sprite deselected_Sprite;
    [SerializeField] private Sprite hover_Sprite;

    private void Awake()
    {
        target_Image = GetComponent<Image>();
        target_InputField = GetComponent<InputField>();
    }
    public void OnSelect(BaseEventData data)
    {
        target_Image.sprite = selected_Sprite;
    }
    public void OnDeselect(BaseEventData data)
    {
        target_Image.sprite = deselected_Sprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!target_InputField.isFocused)
            target_Image.sprite = deselected_Sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!target_InputField.isFocused)
            target_Image.sprite = hover_Sprite;
    }
}
