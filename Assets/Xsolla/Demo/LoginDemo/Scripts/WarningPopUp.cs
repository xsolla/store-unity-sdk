using UnityEngine;
using UnityEngine.UI;

public class WarningPopUp : PopUp
{
    [SerializeField] private Button documentation_Button;
    private new void Awake()
    {
        close_Button.onClick.AddListener(() => Application.OpenURL("https://publisher.xsolla.com/"));
        documentation_Button.onClick.AddListener(() => Application.OpenURL("https://github.com/xsolla/login-unity-sdk/blob/master/README.md"));
    }
}
