using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla;
using Xsolla.Core;
using Xsolla.Login;

public class LoginPage : Page, ILogin
{
    [SerializeField] private InputField login_InputField;
    [SerializeField] private InputField password_InputField;
    [SerializeField] private Button login_Btn;
    [SerializeField] private Toggle rememberMe_ChkBox;
    [SerializeField] private Toggle showPassword_Toggle;
    [Header("Swap Button Images")]
    [SerializeField] private Image login_Image;
    [SerializeField] private Sprite disabled_Sprite;
    [SerializeField] private Sprite enabled_Sprite;

    public Action<User> OnSuccessfulLogin
    {
        get
        {
            return onSuccessfulLogin;
        }

        set
        {
            onSuccessfulLogin = value;
        }
    }

    public Action<ErrorDescription> OnUnsuccessfulLogin
    {
        get
        {
            return onUnsuccessfulLogin;
        }

        set
        {
            onUnsuccessfulLogin = value;
        }
    }

    private Action<User> onSuccessfulLogin;
    private Action<ErrorDescription> onUnsuccessfulLogin;

    private void Awake()
    {
        showPassword_Toggle.onValueChanged.AddListener((mood) => {
            password_InputField.contentType = mood ? InputField.ContentType.Password : InputField.ContentType.Standard;
            password_InputField.ForceLabelUpdate();
        });
        login_Btn.onClick.AddListener(Login);
        login_InputField.onValueChanged.AddListener(ChangeButtonImage);
        password_InputField.onValueChanged.AddListener(ChangeButtonImage);
    }

    private void ChangeButtonImage(string arg0)
    {
        if (!string.IsNullOrEmpty(login_InputField.text) && password_InputField.text.Length > 5)
        {
            if (login_Image.sprite != enabled_Sprite)
                login_Image.sprite = enabled_Sprite;
        }
        else if (login_Image.sprite == enabled_Sprite)
                login_Image.sprite = disabled_Sprite;
    }

    private void Start()
    {
        login_InputField.text = XsollaLogin.Instance.LastUserLogin;
        password_InputField.text = XsollaLogin.Instance.LastUserPassword;
    }

    private void OnLogin(User user)
    {
        if (XsollaLogin.Instance.IsTokenValid && XsollaSettings.UseJwtValidation)
        {
            Debug.Log(string.Format("Your token {0} is active", XsollaLogin.Instance.Token));
        }
        else if (!XsollaSettings.UseJwtValidation)
        {
            Debug.Log("Unsafe signed in");
        }
        if (onSuccessfulLogin != null)
            onSuccessfulLogin.Invoke(user);
    }

    public void Login()
    {
        if (!string.IsNullOrEmpty(login_InputField.text) && password_InputField.text.Length > 5)
        {
            XsollaLogin.Instance.SignIn(login_InputField.text, password_InputField.text, rememberMe_ChkBox.isOn, OnLogin, onUnsuccessfulLogin);
        }
        else
            Debug.Log("Fill all fields");
    }
}