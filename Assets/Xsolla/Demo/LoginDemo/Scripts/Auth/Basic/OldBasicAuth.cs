using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Login;

public class OldBasicAuth : MonoBehaviour, OldILoginAuthorization
{
    public Action<string> OnSuccess { get; set; }
	public Action OnFailed { get; set; }

	public event Action UserAuthEvent;
    public event Action<Error> UserAuthErrorEvent;

    private OldBasicAuthButton loginButton;
    private string username = "";
    private string password = "";
    private bool rememberMe = false;

	private void OnDestroy()
	{
		if(loginButton != null) {
            Destroy(loginButton);
        }
	}

	private bool IsValidCredentials()
	{
        return
			!string.IsNullOrEmpty(username) &&
			!string.IsNullOrEmpty(password) &&
			(password.Length > 5);
    }

	public void SetUserName(string username)
	{
        this.username = username;
    }

    public void SetPassword(string password)
	{
        this.password = password;
	}

	public void SetRememberMe(bool value)
	{
        rememberMe = value;
	}

    public OldBasicAuth SetLoginButton(Button button)
	{
		loginButton = gameObject.AddComponent<OldBasicAuthButton>().
			SetButton(button).
			SetActiveCondition(IsValidCredentials).
			SetHandler(Login);
		return this;
	}

	public void SoftwareAuth()
	{
        loginButton.SoftwareClick();
	}

    private void Login()
    {
        if (IsValidCredentials()) {
            XsollaLogin.Instance.SignIn(username, password, rememberMe, BasicAuthSuccess, BasicAuthFailed);
        } else {
            Debug.LogWarning("Fill all fields. Password must be at least 6 symbols.");
        }
    }

	private void BasicAuthFailed(Error error)
	{
        Debug.LogWarning("Basic auth failed! " + error.errorMessage);
        UserAuthErrorEvent?.Invoke(error);
        OnFailed?.Invoke();
	}

    private void BasicAuthSuccess()
    {
        OnSuccess?.Invoke(XsollaLogin.Instance.Token);
    }
}
