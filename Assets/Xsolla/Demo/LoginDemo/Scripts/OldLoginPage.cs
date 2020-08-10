using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Login;

public class OldLoginPage : Page, OldILogin
{
    [SerializeField] private InputField loginInputField;
    [SerializeField] private InputField passwordInputField;
    [SerializeField] private Button loginButton;
    [SerializeField] private Toggle rememberMeChkBox;
    [SerializeField] private Toggle showPasswordToggle;

    private OldBasicAuth _basicAuth;

    public Action OnSuccessfulLogin { get; set; }
    public Action<Error> OnUnsuccessfulLogin { get; set; }

    public void Login()
    {
        if (_basicAuth != null)
        {
            _basicAuth.SoftwareAuth();
        }
    }
    
    private void Awake()
	{
        XsollaLogin.Instance.Token = null;
    }

    private void Start()
    {
        showPasswordToggle.onValueChanged.AddListener((isPasswordHidden) => {
            passwordInputField.contentType = isPasswordHidden ? InputField.ContentType.Password : InputField.ContentType.Standard;
            passwordInputField.ForceLabelUpdate();
        });

        TryAuthBy<OldSavedTokenAuth>(SavedTokenAuthFailed);
    }

    private void SavedTokenAuthFailed()
    {
        TryAuthBy<OldLauncherAuth>(LauncherAuthFailed);
    }
    
	private void LauncherAuthFailed()
    {
		if (XsollaSettings.UseSteamAuth) {
            TryAuthBy<OldSteamAuth>(SteamAuthFailed, token => token.FromSteam = true);
        } else {
            TryAuthBy<OldConsolePlatformAuth>(ConsoleAuthFailed);
        }        
    }

    private void SteamAuthFailed()
    {
        TryAuthBy<OldConsolePlatformAuth>(ConsoleAuthFailed);
    }

    private void ConsoleAuthFailed()
    {
        TryBasicAuth();

        TryAuthBy<OldSocialAuth>(onFailed: () => OnUnsuccessfulLogin?.Invoke(new Error(errorMessage:"Social auth failed")),
                            success: token => XsollaLogin.Instance.SaveToken(Constants.LAST_SUCCESS_AUTH_TOKEN, token));
    }

    private void TryBasicAuth()
    {
        _basicAuth = TryAuthBy<OldBasicAuth>(null, token =>
        {
            if(rememberMeChkBox.isOn)
                XsollaLogin.Instance.SaveToken(Constants.LAST_SUCCESS_AUTH_TOKEN, token);
            else
                XsollaLogin.Instance.DeleteToken(Constants.LAST_SUCCESS_AUTH_TOKEN);
        }).SetLoginButton(loginButton);
        _basicAuth.UserAuthEvent += () => OnSuccessfulLogin?.Invoke();
        _basicAuth.UserAuthErrorEvent += error => OnUnsuccessfulLogin?.Invoke(error);

        ConfigBaseAuth();
    }

    private T TryAuthBy<T>(Action onFailed = null, Action<Token> success = null) where T: MonoBehaviour, OldILoginAuthorization
	{
        T auth = gameObject.AddComponent<T>();
        auth.OnSuccess = token => SuccessAuthorization(token, success);
        auth.OnFailed = onFailed;
        return auth;
    }

    private void SuccessAuthorization(string token, Action<Token> success = null)
	{
        token = token.Split('&').First();
        ValidateToken(token, () => {
            XsollaLogin.Instance.Token = token;
            success?.Invoke(XsollaLogin.Instance.Token);
            Debug.Log($"Your token: {token}");
            SceneManager.LoadScene("Store");
        }, OnUnsuccessfulLogin);
    }

	private void ValidateToken(string token, Action onSuccess, Action<Error> onFailed)
    {
        XsollaLogin.Instance.GetUserInfo(token, _ => {
            Debug.Log("Validation success");
            onSuccess?.Invoke();
        }, error => {
            Debug.LogWarning("Get UserInfo failed!");
            onFailed?.Invoke(error);
        });
    }

	private void ConfigBaseAuth()
	{
        _basicAuth.SetUserName(loginInputField.text);
        _basicAuth.SetPassword(passwordInputField.text);
        _basicAuth.SetRememberMe(rememberMeChkBox.isOn);

        loginInputField.onValueChanged.AddListener(_basicAuth.SetUserName);
        passwordInputField.onValueChanged.AddListener(_basicAuth.SetPassword);
        rememberMeChkBox.onValueChanged.AddListener(_basicAuth.SetRememberMe);

        LogInHotkeys hotKeys = gameObject.GetComponent<LogInHotkeys>();
        hotKeys.EnterKeyPressedEvent += _basicAuth.SoftwareAuth;
        hotKeys.TabKeyPressedEvent += ChangeFocus;
    }

    private void ChangeFocus()
    {
        if (loginInputField.isFocused)
            passwordInputField.Select();
        else
            loginInputField.Select();
    }
}