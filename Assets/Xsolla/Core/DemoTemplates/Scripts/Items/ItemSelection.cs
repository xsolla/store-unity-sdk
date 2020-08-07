using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSelection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite itemSelectionSprite;
    [SerializeField] private Image itemSelectionImage;

    void Start()
    {
        if (itemSelectionImage != null)
            itemSelectionImage.gameObject.SetActive(false);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemSelectionImage == null || itemSelectionSprite == null) return;
        itemSelectionImage.gameObject.SetActive(true);
        itemSelectionImage.sprite = itemSelectionSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (itemSelectionImage != null)
            itemSelectionImage.gameObject.SetActive(false);
    }
}
