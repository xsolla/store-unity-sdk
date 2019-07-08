using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla;
using Xsolla.Login;

public class ResetPasswordPage : Page, IResetPassword
{
    [SerializeField] private InputField email_InputField;
    [SerializeField] private Button change_Btn;
    [Header("Swap Button Images")]
    [SerializeField] private Image change_Image;
    [SerializeField] private Sprite disabled_Sprite;
    [SerializeField] private Sprite enabled_Sprite;

    public Action OnSuccessfulResetPassword
    {
        get
        {
            return onSuccessfulResetPassword;
        }

        set
        {
            onSuccessfulResetPassword = value;
        }
    }

    public Action<ErrorDescription> OnUnsuccessfulResetPassword
    {
        get
        {
            return onUnsuccessfulResetPassword;
        }

        set
        {
            onUnsuccessfulResetPassword = value;
        }
    }

    private Action onSuccessfulResetPassword;
    private Action<ErrorDescription> onUnsuccessfulResetPassword;

    private void Awake()
    {
        change_Btn.onClick.AddListener(ResetPassword);
        email_InputField.onValueChanged.AddListener(ChangeButtonImage);
    }

    private void ChangeButtonImage(string arg0)
    {
        if (!string.IsNullOrEmpty(email_InputField.text))
        {
            if (change_Image.sprite != enabled_Sprite)
                change_Image.sprite = enabled_Sprite;
        }
        else if (change_Image.sprite == enabled_Sprite)
            change_Image.sprite = disabled_Sprite;
    }

    private void SuccessfulResetPassword()
    {
        Debug.Log("Successfull reseted password");
        if (onSuccessfulResetPassword != null)
            onSuccessfulResetPassword.Invoke();
    }
    public void ResetPassword()
    {
        if (!string.IsNullOrEmpty(email_InputField.text))
            XsollaLogin.Instance.ResetPassword(email_InputField.text, SuccessfulResetPassword, onUnsuccessfulResetPassword);
        else
            Debug.Log("Fill all fields");
    }
}
