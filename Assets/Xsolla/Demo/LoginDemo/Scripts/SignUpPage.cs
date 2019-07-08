using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla;
using Xsolla.Login;

public class SignUpPage :  Page, ISignUp
{
    [SerializeField] private InputField login_InputField;
    [SerializeField] private InputField password_InputField;
    [SerializeField] private InputField email_InputField;
    [SerializeField] private Toggle showPassword_Toggle;
    [SerializeField] private Button create_Btn;
    [Header("Swap Button Images")]
    [SerializeField] private Image signUp_Image;
    [SerializeField] private Sprite disabled_Sprite;
    [SerializeField] private Sprite enabled_Sprite;

    public string SignUpEmail
    {
        get
        {
            return email_InputField.text;
        }
    }

    public Action OnSuccessfulSignUp
    {
        get
        {
            return onSuccessfulSignUp;
        }

        set
        {
            onSuccessfulSignUp = value;
        }
    }

    public Action<ErrorDescription> OnUnsuccessfulSignUp
    {
        get
        {
            return onUnsuccessfulSignUp;
        }

        set
        {
            onUnsuccessfulSignUp = value;
        }
    }

    private Action onSuccessfulSignUp;
    private Action<ErrorDescription> onUnsuccessfulSignUp;

    private void Awake()
    {
        create_Btn.onClick.AddListener(SignUp);
        showPassword_Toggle.onValueChanged.AddListener((mood) => 
        {
            password_InputField.contentType = mood ? InputField.ContentType.Password : InputField.ContentType.Standard;
            password_InputField.ForceLabelUpdate();
        });
        login_InputField.onValueChanged.AddListener(ChangeButtonImage);
        password_InputField.onValueChanged.AddListener(ChangeButtonImage);
        email_InputField.onValueChanged.AddListener(ChangeButtonImage);
    }

    private void ChangeButtonImage(string arg0)
    {
        if (!string.IsNullOrEmpty(login_InputField.text) && !string.IsNullOrEmpty(email_InputField.text) && !string.IsNullOrEmpty(password_InputField.text) && password_InputField.text.Length > 5)
        {
            if (signUp_Image.sprite != enabled_Sprite)
                signUp_Image.sprite = enabled_Sprite;
        }
        else if (signUp_Image.sprite == enabled_Sprite)
            signUp_Image.sprite = disabled_Sprite;
    }
    
    public void SignUp()
    {
        if (!string.IsNullOrEmpty(login_InputField.text) && !string.IsNullOrEmpty(email_InputField.text) && !string.IsNullOrEmpty(password_InputField.text) && password_InputField.text.Length > 5)
        {
            XsollaLogin.Instance.Registration(login_InputField.text, password_InputField.text, email_InputField.text, onSuccessfulSignUp, onUnsuccessfulSignUp);
        }
        else
            Debug.Log("Fill all fields");
    }

}
