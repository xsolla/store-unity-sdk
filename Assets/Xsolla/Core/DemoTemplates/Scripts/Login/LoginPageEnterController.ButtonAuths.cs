using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Core.Popup;

namespace Xsolla.Demo
{
	public partial class LoginPageEnterController : LoginPageController
	{
		[SerializeField] InputField EmailInputField = default;
		[SerializeField] InputField PasswordInputField = default;
		[SerializeField] Toggle RememberMeCheckbox = default;

		[SerializeField] SimpleButton LoginButton = default;

		[SerializeField] SimpleSocialButton[] SocialLoginButtons = default;
		[SerializeField] SimpleButton SteamLoginButton = default;

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
			PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => IsAuthInProgress == false);

			object[] args = { username, password, rememberMe };

			Action<string> onSuccessfulBasicAuth = token => DemoController.Instance.GetImplementation()
				.ValidateToken(token, t => CompleteSuccessfulAuth(token, true, isSaveToken: rememberMe), ProcessError);
			Action<Error> onFailedBasicAuth = ProcessError;

			TryAuthBy<BasicAuth>(args, onSuccessfulBasicAuth, onFailedBasicAuth);
		}

		public void RunSocialAuth(SocialProvider socialProvider)
		{
			if (IsAuthInProgress)
				return;

			IsAuthInProgress = true;
			object[] args = { socialProvider };

			Action<string> onSuccessfulSocialAuth = token => DemoController.Instance.GetImplementation()
				.ValidateToken(token, t => CompleteSuccessfulAuth(token, isSaveToken: true), ProcessError);
			Action<Error> onFailedSocialAuth = ProcessError;

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
			PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => IsAuthInProgress == false);

			Action<string> onSuccessfulSteamAuth = token => DemoController.Instance.GetImplementation()
				.ValidateToken(token, t => CompleteSuccessfulAuth(token, isSaveToken: true), ProcessError);
			Action<Error> onFailedSteamAuth = ProcessError;

			TryAuthBy<SteamAuth>(args: null, onSuccess: onSuccessfulSteamAuth, onFailed: onFailedSteamAuth);
		}
	}
}
