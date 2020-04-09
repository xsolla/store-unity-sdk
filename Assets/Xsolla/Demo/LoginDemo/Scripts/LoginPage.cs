using System;
using System.Collections;
using Microsoft.IdentityModel.JsonWebTokens;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Login;

public class LoginPage : Page, ILogin
{
    [SerializeField] private InputField loginInputField;
    [SerializeField] private InputField passwordInputField;
    [SerializeField] private Button loginButton;
    [SerializeField] private Toggle rememberMeChkBox;
    [SerializeField] private Toggle showPasswordToggle;

    private BasicAuth _basicAuth;

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

        loginInputField.text = XsollaLogin.Instance.LastUserLogin;
        passwordInputField.text = XsollaLogin.Instance.LastUserPassword;

        TryAuthBy<LauncherAuth>(LauncherAuthFailed);
    }

	private void LauncherAuthFailed()
    {
		if (XsollaSettings.UseSteamAuth) {
            TryAuthBy<SteamAuth>(SteamAuthFailed, (Token token) => token.FromSteam = true);
        } else {
            TryAuthBy<ConsolePlatformAuth>(ConsoleAuthFailed);
        }        
    }

    private void SteamAuthFailed()
    {
        TryAuthBy<ConsolePlatformAuth>(ConsoleAuthFailed);
    }

    private void ConsoleAuthFailed()
    {
		_basicAuth = TryAuthBy<BasicAuth>().SetLoginButton(loginButton);
        _basicAuth.UserAuthEvent += () => OnSuccessfulLogin?.Invoke();
        _basicAuth.UserAuthErrorEvent += (Error error) => OnUnsuccessfulLogin?.Invoke(error);

        ConfigBaseAuth();
    }

    private T TryAuthBy<T>(Action onFailed = null, Action<Token> success = null) where T: MonoBehaviour, ILoginAuthorization
	{
        T auth = gameObject.AddComponent<T>();
        auth.OnSuccess = token => SuccessAuthorization(token, success);
        auth.OnFailed = onFailed;
        return auth;
    }

    private void SuccessAuthorization(string token, Action<Token> success = null)
	{
        ValidateToken(token, () => {
            XsollaLogin.Instance.Token = token;
            success?.Invoke(XsollaLogin.Instance.Token);
            Debug.Log($"Your token: {token}");
            SceneManager.LoadScene("Store");
        }, OnUnsuccessfulLogin);
    }

	private void ValidateToken(string token, Action onSuccess, Action<Error> onFailed)
    {
        // This is temporary block of code.
        Func<IEnumerator> success = () => { 
            onSuccess?.Invoke();
            return null;
        };
        StartCoroutine(success.Invoke());
        // TODO: this API method works not correct. So it is will be later. Do not use it yet.
        // XsollaLogin.Instance.GetUserInfo(token, _ => {
        //     Debug.Log("Validation success");
        //     onSuccess?.Invoke();
        // }, (Error error) => {
        //     Debug.LogWarning("Get UserInfo failed!");
        //     onFailed?.Invoke(error);
        // });
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