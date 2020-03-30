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

    private BasicAuth basicAuth;

    public Action<User> OnSuccessfulLogin { get; set; }
    public Action<Error> OnUnsuccessfulLogin { get; set; }

	public void Login() => basicAuth?.SoftwareAuth();

    void Start()
    {
        showPassword_Toggle.onValueChanged.AddListener((isPasswordHidden) => {
            password_InputField.contentType = isPasswordHidden ? InputField.ContentType.Password : InputField.ContentType.Standard;
            password_InputField.ForceLabelUpdate();
        });

        login_InputField.text = XsollaLogin.Instance.LastUserLogin;
        password_InputField.text = XsollaLogin.Instance.LastUserPassword;

        TryAuthBy<LauncherAuth>(LauncherAuthFailed);
    }

	private void LauncherAuthFailed()
    {
		if (XsollaSettings.UseSteamAuth) {
            TryAuthBy<SteamAuth>(SteamAuthFailed);
        } else {
            TryAuthBy<ShadowAuth>(ShadowAuthFailed);
        }        
    }

    private void SteamAuthFailed()
    {
        TryAuthBy<ShadowAuth>(ShadowAuthFailed);
    }

    private void ShadowAuthFailed()
    {
		basicAuth = TryAuthBy<BasicAuth>().SetLoginButton(login_Btn);
        basicAuth.UserAuthEvent += (User user) => OnSuccessfulLogin?.Invoke(user);
        basicAuth.UserAuthErrorEvent += (Error error) => OnUnsuccessfulLogin?.Invoke(error);

        ConfigBaseAuth();
    }

    private T TryAuthBy<T>(Action onFailed = null) where T: MonoBehaviour, ILoginAuthorization
	{
        T auth = gameObject.AddComponent<T>();
        auth.OnSuccess = SuccessAuthorization;
        auth.OnFailed = onFailed;
        return auth;
    }

    private void SuccessAuthorization(string token)
	{
        XsollaLogin.Instance.Token = token;
        SceneManager.LoadScene("Store");
    }

	private void ConfigBaseAuth()
	{
        basicAuth.SetUserName(login_InputField.text);
        basicAuth.SetPassword(password_InputField.text);
        basicAuth.SetRememberMe(rememberMe_ChkBox.isOn);

        login_InputField.onValueChanged.AddListener(basicAuth.SetUserName);
        password_InputField.onValueChanged.AddListener(basicAuth.SetPassword);
        rememberMe_ChkBox.onValueChanged.AddListener(basicAuth.SetRememberMe);

        LogInHotkeys hotkeys = gameObject.GetComponent<LogInHotkeys>();
        hotkeys.EnterKeyPressedEvent += basicAuth.SoftwareAuth;
        hotkeys.TabKeyPressedEvent += ChangeFocus;
    }

    private void ChangeFocus()
    {
        if (login_InputField.isFocused)
            password_InputField.Select();
        else
            login_InputField.Select();
    }
}