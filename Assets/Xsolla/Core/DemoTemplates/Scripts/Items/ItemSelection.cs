using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSelection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject itemSelectionImage;
    [SerializeField] private List<SelectableArea> someArea;
    private int _counter;

    void Start()
    {
        if (itemSelectionImage != null)
            itemSelectionImage.SetActive(false);
        if (someArea != null && someArea.Any())
        {
            someArea.ForEach(s => s.OnPointerEnterEvent += EnableSelection);
            someArea.ForEach(s => s.OnPointerExitEvent += DisableSelection);
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        EnableSelection();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DisableSelection();
    }

    private void EnableSelection()
    {
        _counter++;
        if (_counter <= 0) _counter = 1;
        if (itemSelectionImage == null) return;
        itemSelectionImage.SetActive(true);
    }

    private void DisableSelection()
    {
        _counter--;
        if (itemSelectionImage != null && _counter <= 0)
            itemSelectionImage.SetActive(false);
    }
}
