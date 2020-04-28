using UnityEngine;
using UnityEngine.UI;

public class CloseBrowserButton : MonoBehaviour
{
    public Button CloseButton;
    
    void Start()
    {
        if (CloseButton != null)
        {
            CloseButton.onClick.AddListener(() => Destroy(gameObject, 0.01F));
        }
    }
}
