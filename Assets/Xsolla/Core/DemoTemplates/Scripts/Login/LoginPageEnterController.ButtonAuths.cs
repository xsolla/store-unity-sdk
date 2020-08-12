using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Login;

public partial class LoginPageEnterController : LoginPageController
{
#pragma warning disable 0649
	[SerializeField] InputField EmailInputField;
	[SerializeField] InputField PasswordInputField;
	[SerializeField] Toggle RememberMeCheckbox;

	[SerializeField] SimpleButton LoginButton;

	[SerializeField] SimpleSocialButton[] SocialLoginButtons;
	[SerializeField] SimpleButton SteamLoginButton;
#pragma warning restore 0649

	private void Awake()
	{
		if (LoginButton != null)
			LoginButton.onClick += PrepareAndRunBasicAuth;

		if (SocialLoginButtons != null)
		{
			foreach (var socialButton in SocialLoginButtons)
			{
				if (socialButton != null)
					socialButton.onClick += () => RunSocialAuth(socialButton.SocialProvider);
			}
		}

		if (SteamLoginButton != null)
			SteamLoginButton.onClick += RunManualSteamAuth;
	}

	private void PrepareAndRunBasicAuth()
	{
		RunBasicAuth(EmailInputField.text, PasswordInputField.text, RememberMeCheckbox.isOn);
	}

	public void RunBasicAuth(string username, string password, bool rememberMe)
	{
		if(IsAuthInProgress)
			return;

		IsAuthInProgress = true;

		object[] args = { username, password, rememberMe };

		Action<string> onSuccessfulBasicAuth = token =>
		{
			ValidateToken(token,
				onSuccess: () => CompleteSuccessfulAuth(token, isBasicAuth: true, isSaveToken: rememberMe),
				onFailed: error => ProcessError(error));
		};

		Action<Error> onFailedBasicAuth = error =>
		{
			ProcessError(error);
		};

		TryAuthBy<BasicAuth>(args, onSuccessfulBasicAuth, onFailedBasicAuth);
	}

	public void RunSocialAuth(SocialProvider socialProvider)
	{
		if (IsAuthInProgress)
			return;

		IsAuthInProgress = true;

		object[] args = { socialProvider };

		Action<string> onSuccessfulSocialAuth = token =>
		{
			ValidateToken(token,
				onSuccess: () => CompleteSuccessfulAuth(token, isSaveToken: true),
				onFailed: error => ProcessError(error));
		};

		Action<Error> onFailedSocialAuth = error =>
		{
			ProcessError(error);
		};

#if UNITY_EDITOR || UNITY_STANDALONE
		TryAuthBy<SocialAuth>(args, onSuccessfulSocialAuth, onFailedSocialAuth);
#elif UNITY_ANDROID
		TryAuthBy<AndroidSocialAuth>(args, onSuccessfulSocialAuth, onFailedSocialAuth);
#endif
	}

	public void RunManualSteamAuth()
	{
		if (IsAuthInProgress)
			return;

		IsAuthInProgress = true;

		Action<string> onSuccessfulSteamAuth = token =>
		{
			ValidateToken(token,
				onSuccess: () => CompleteSuccessfulAuth(token: token, isSteam: true, isSaveToken: true),
				onFailed: error => ProcessError(error));
		};

		Action<Error> onFailedSteamAuth = error =>
		{
			ProcessError(error);
		};

		TryAuthBy<SteamAuth>(args: null, onSuccess: onSuccessfulSteamAuth, onFailed: onFailedSteamAuth);
	}
}