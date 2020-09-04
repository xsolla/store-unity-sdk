using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurSelectedUser : MonoBehaviour
{
    [SerializeField] private GameObject blurMaskObject;
    [SerializeField] private ItemSelection blurTrigger;

    private void Start()
    {
        if (blurTrigger == null || blurMaskObject == null) return;
        blurTrigger.OnPointerEnterEvent += () => blurMaskObject.SetActive(true);
        blurTrigger.OnPointerExitEvent += () => blurMaskObject.SetActive(false);
    }
}
