using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ExtendedPopUp : PopUp, IExtendedPopUp
{
    [SerializeField] private Text header_Text;
    [SerializeField] private Button returnToLogin_Button;

    public UnityAction OnReturnToLogin
    {
        set
        {
            returnToLogin_Button.onClick.AddListener(value);
        }
    }
    protected override void Awake()
    {
        base.Awake();
        returnToLogin_Button.onClick.AddListener(Close);
    }
    public void ShowPopUp(string header, string message)
    {
        header_Text.text = header;
        ShowPopUp(message);
    }
}
