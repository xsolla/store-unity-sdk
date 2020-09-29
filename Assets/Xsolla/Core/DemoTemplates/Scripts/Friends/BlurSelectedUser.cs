using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurSelectedUser : MonoBehaviour
{
    public static event Action OnActivated;
    public static event Action OnDeactivated;
    
    [SerializeField] private GameObject blurMaskObject;
    [SerializeField] private ItemSelection blurTrigger;

    private void Start()
    {
        if (blurTrigger == null || blurMaskObject == null) return;
        blurTrigger.OnPointerEnterEvent += () =>
        {
            blurMaskObject.SetActive(true);
            OnActivated?.Invoke();
        };
        blurTrigger.OnPointerExitEvent += () =>
        {
            blurMaskObject.SetActive(false);
            OnDeactivated?.Invoke();
        };
    }
}
