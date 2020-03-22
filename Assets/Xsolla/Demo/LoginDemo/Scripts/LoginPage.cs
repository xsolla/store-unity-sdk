using System;
using System.Collections;
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

    public Action<User> OnSuccessfulLogin { get; set; }
    public Action<Error> OnUnsuccessfulLogin { get; set; }

    private DateTime lastClick;
    private float rateLimitMs = Constants.LoginPageRateLimitMs;

    void Awake()
    {
        lastClick = DateTime.MinValue;
        
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
        
        LogInHotkeys hotkeys = gameObject.GetComponent<LogInHotkeys>();
        hotkeys.EnterKeyPressedEvent += Login;
        hotkeys.TabKeyPressedEvent += ChangeFocus;

        StartCoroutine(TryAuthWithShadowToken());
    }

    IEnumerator TryAuthWithShadowToken()
    {
        if (XsollaSettings.IsShadow) {
            int datetime = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            int r = new System.Random().Next();
            XsollaLogin.Instance.ShadowAccountUserID = "sdk_temp_user_id_" + r.ToString() + "_" + datetime.ToString();

            bool busy = true;
            XsollaLogin.Instance.SignInShadowAccount(XsollaLogin.Instance.ShadowAccountUserID, (string token) => {
                XsollaLogin.Instance.Token = token;
                SceneManager.LoadScene("Store");
                busy = false;
            }, (Error error) => {
                OnUnsuccessfulLogin?.Invoke(error);
                busy = false;
            });
            yield return new WaitWhile(() => busy);
        } else {
            StartCoroutine(TryAuthWithLauncherToken());
        }
    }

    IEnumerator TryAuthWithLauncherToken()
    {
        string launcherToken = LauncherArguments.Instance.GetToken();
        if (!string.IsNullOrEmpty(launcherToken)) {
            XsollaLogin.Instance.Token = launcherToken;
            SceneManager.LoadScene("Store");
        }
        yield break;
    }

    private void ChangeFocus()
    {
        if (login_InputField.isFocused)
            password_InputField.Select();
        else
            login_InputField.Select();
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
			    OnSuccessfulLogin?.Invoke(user);
	        }
	        else
	        {
		        SceneManager.LoadScene("Store");
	        }
        }
    }

    public void Login()
    {
        TimeSpan ts = DateTime.Now - lastClick;
        if (ts.TotalMilliseconds > rateLimitMs) {
            lastClick += ts;
            if (!string.IsNullOrEmpty(login_InputField.text) && password_InputField.text.Length > 5) {
                XsollaLogin.Instance.SignIn(login_InputField.text, password_InputField.text, rememberMe_ChkBox.isOn, OnLogin, OnUnsuccessfulLogin);
            } else
                Debug.Log("Fill all fields");
        }
    }
}