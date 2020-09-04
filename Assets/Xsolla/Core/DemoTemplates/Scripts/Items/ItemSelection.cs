using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSelection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event Action OnPointerEnterEvent;
    public event Action OnPointerExitEvent;
    
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
        if (itemSelectionImage == null || _counter > 1) return;
        itemSelectionImage.SetActive(true);
        OnPointerEnterEvent?.Invoke();
    }

    private void DisableSelection()
    {
        _counter--;
        if (itemSelectionImage != null && _counter <= 0)
        {
            OnPointerExitEvent?.Invoke();
            itemSelectionImage.SetActive(false);
        }
    }
}
