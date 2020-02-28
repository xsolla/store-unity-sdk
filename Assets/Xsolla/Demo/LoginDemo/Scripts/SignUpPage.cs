using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla;
using Xsolla.Core;
using Xsolla.Login;

public class SignUpPage :  Page, ISignUp
{
    [SerializeField] private InputField login_InputField;
    [SerializeField] private InputField password_InputField;
    [SerializeField] private InputField email_InputField;
    [SerializeField] private Toggle showPassword_Toggle;
    [SerializeField] private Button create_Btn;

    private DateTime lastClick;
    private float rateLimitMs = Constants.LoginPageRateLimitMs;

    public string SignUpEmail
    {
        get
        {
            return email_InputField.text;
        }
    }

    public Action OnSuccessfulSignUp { get; set; }
    public Action<Error> OnUnsuccessfulSignUp { get; set; }

    void Awake()
    {
        login_InputField.onValueChanged.AddListener(delegate { UpdateButtonState(); });
        password_InputField.onValueChanged.AddListener(delegate { UpdateButtonState(); });
        email_InputField.onValueChanged.AddListener(delegate { UpdateButtonState(); });
        
        lastClick = DateTime.MinValue;
        create_Btn.onClick.AddListener(SignUp);

        showPassword_Toggle.onValueChanged.AddListener((mood) => 
        {
            password_InputField.contentType = mood ? InputField.ContentType.Password : InputField.ContentType.Standard;
            password_InputField.ForceLabelUpdate();
        });
        
        create_Btn.onClick.AddListener(SignUp);
    }
    
    private void Start()
    {
        LogInHotkeys hotkeys = gameObject.GetComponent<LogInHotkeys>();
        hotkeys.EnterKeyPressedEvent += SignUp;
        hotkeys.TabKeyPressedEvent += ChangeFocus;
        
        UpdateButtonState();
    }

    private void ChangeFocus()
    {
        if (login_InputField.isFocused) {
            email_InputField.Select();
        } else {
            if (email_InputField.isFocused) {
                password_InputField.Select();
            } else {
                login_InputField.Select();
            }
        }
    }
    
    void UpdateButtonState()
    {
        create_Btn.interactable = !string.IsNullOrEmpty(login_InputField.text) && !string.IsNullOrEmpty(email_InputField.text) && !string.IsNullOrEmpty(password_InputField.text) && password_InputField.text.Length > 5;
    }
    
    public void SignUp()
    {
        TimeSpan ts = DateTime.Now - lastClick;
        if (ts.TotalMilliseconds > rateLimitMs) {
            lastClick += ts;
            if (!string.IsNullOrEmpty(login_InputField.text) && !string.IsNullOrEmpty(email_InputField.text) && !string.IsNullOrEmpty(password_InputField.text) && password_InputField.text.Length > 5) {
                XsollaLogin.Instance.Registration(login_InputField.text, password_InputField.text, email_InputField.text, OnSuccessfulSignUp, OnUnsuccessfulSignUp);
            } else
                Debug.Log("Fill all fields");
        }
    }
}
