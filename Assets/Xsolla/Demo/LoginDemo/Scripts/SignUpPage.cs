using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Login;

public class SignUpPage : Page, ISignUp
{
    [SerializeField] private InputField login_InputField;
    [SerializeField] private InputField password_InputField;
    [SerializeField] private InputField email_InputField;
    [SerializeField] private Toggle showPassword_Toggle;
    [SerializeField] private Button create_Btn;

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

    public Action<Xsolla.Core.Error> OnUnsuccessfulSignUp
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
    private Action<Xsolla.Core.Error> onUnsuccessfulSignUp;

    void Awake()
    {
        login_InputField.onValueChanged.AddListener(delegate { UpdateButtonState(); });
        password_InputField.onValueChanged.AddListener(delegate { UpdateButtonState(); });
        email_InputField.onValueChanged.AddListener(delegate { UpdateButtonState(); });
        
        showPassword_Toggle.onValueChanged.AddListener((mood) => 
        {
            password_InputField.contentType = mood ? InputField.ContentType.Password : InputField.ContentType.Standard;
            password_InputField.ForceLabelUpdate();
        });
        
        create_Btn.onClick.AddListener(SignUp);
    }
    
    void Start()
    {
        UpdateButtonState();
    }

    void UpdateButtonState()
    {
        create_Btn.interactable = !string.IsNullOrEmpty(login_InputField.text) && !string.IsNullOrEmpty(email_InputField.text) && !string.IsNullOrEmpty(password_InputField.text) && password_InputField.text.Length > 5;
    }
    
    public void SignUp()
    {
        XsollaLogin.Instance.Registration(login_InputField.text, password_InputField.text, email_InputField.text, onSuccessfulSignUp, onUnsuccessfulSignUp);
    }
}
