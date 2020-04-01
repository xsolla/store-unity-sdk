using System;
using Microsoft.IdentityModel.JsonWebTokens;
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

    public Action OnSuccessfulLogin { get; set; }
    public Action<Error> OnUnsuccessfulLogin { get; set; }

	public void Login() => basicAuth?.SoftwareAuth();

	void Awake()
	{
        XsollaLogin.Instance.Token = null;
    }

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
        TryAuthBy<SteamAuth>(SteamAuthFailed);
    }

    private void SteamAuthFailed()
    {
        TryAuthBy<ShadowAuth>(ShadowAuthFailed);
    }

    private void ShadowAuthFailed()
    {
		basicAuth = TryAuthBy<BasicAuth>().SetLoginButton(login_Btn);
        basicAuth.UserAuthEvent += () => OnSuccessfulLogin?.Invoke();
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
        ValidateToken(token, () => {
            XsollaLogin.Instance.Token = token;
            Debug.Log(string.Format("Your token: {0}", token));
            SceneManager.LoadScene("Store");
        }, OnUnsuccessfulLogin);
    }

	private void ValidateToken(string token, Action onSuccess, Action<Error> onFailed)
	{
        XsollaLogin.Instance.GetUserInfo(token, _ => {
            Debug.Log("Validation success");
            onSuccess?.Invoke();
        }, (Error error) => {
            Debug.LogWarning("Get UserInfo failed!");
            onFailed?.Invoke(error);
        });
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