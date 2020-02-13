using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Login;

public class LoginPage : Page, ILogin
{
    [SerializeField] private InputField login_InputField;
    [SerializeField] private InputField password_InputField;
    [SerializeField] private Button login_Btn;
    [SerializeField] private Toggle rememberMe_ChkBox;
    [SerializeField] private Toggle showPassword_Toggle;

    const string DefaultLoginId = "e6dfaac6-78a8-11e9-9244-42010aa80004";
    const string DefaultStoreProjectId = "44056";

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

    public Action<Xsolla.Core.Error> OnUnsuccessfulLogin
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
    private Action<Xsolla.Core.Error> onUnsuccessfulLogin;

    void Awake()
    {
        login_InputField.onValueChanged.AddListener(delegate { UpdateButtonState(); });
        password_InputField.onValueChanged.AddListener(delegate { UpdateButtonState(); });
        
        showPassword_Toggle.onValueChanged.AddListener((mood) => {
            password_InputField.contentType = mood ? InputField.ContentType.Password : InputField.ContentType.Standard;
            password_InputField.ForceLabelUpdate();
        });
        
        login_Btn.onClick.AddListener(Login);
    }
    
    void Start()
    {
        login_InputField.text = XsollaLogin.Instance.LastUserLogin;
        password_InputField.text = XsollaLogin.Instance.LastUserPassword;

        UpdateButtonState();
    }

    void UpdateButtonState()
    {
        login_Btn.interactable = !string.IsNullOrEmpty(login_InputField.text) && password_InputField.text.Length > 5;
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

        if (XsollaSettings.LoginId == DefaultLoginId)
        {
	        SceneManager.LoadScene("Store");
        }
        else
        {
	        if (XsollaSettings.StoreProjectId == DefaultStoreProjectId)
	        {
		        if (onSuccessfulLogin != null)
			        onSuccessfulLogin.Invoke(user);
	        }
	        else
	        {
		        SceneManager.LoadScene("Store");
	        }
        }
    }

    public void Login()
    {
        XsollaLogin.Instance.SignIn(login_InputField.text, password_InputField.text, rememberMe_ChkBox.isOn, OnLogin, onUnsuccessfulLogin);
    }
}