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

		[Space]
		[SerializeField] private SimpleSocialButton[] MainSocialLoginButtons = default;
		[SerializeField] private SimpleButton OtherSocialNetworksButton = default;
		[SerializeField] private SocialNetworksWidget SocialNetworksWidget = default;

		[Space]
		[SerializeField] private SimpleButton PasswordlessButton = default;
		[SerializeField] private PasswordlessWidget PasswordlessWidget = default;

		[Space]
		[SerializeField] InputField EmailAccessTokenAuthInputField = default;
		[SerializeField] SimpleButton LoginAccessTokenAuthButton = default;

		[Space]
		[SerializeField] SimpleButton DeviceIDAuthButton = default;

		[Space]
		[SerializeField] GameObject LoginAuthPage = default;
		[SerializeField] GameObject AccessTokenAuthPage = default;

		private void Awake()
		{
			LoginButton.onClick += PrepareAndRunBasicAuth;
			LoginAccessTokenAuthButton.onClick += PrepareAndRunAccessTokenAuth;

			SocialNetworksWidget.OnSocialButtonClick = RunSocialAuth;
			OtherSocialNetworksButton.onClick += () => SocialNetworksWidget.gameObject.SetActive(true);

			PasswordlessButton.onClick += () => PasswordlessWidget.gameObject.SetActive(true);

			foreach (var button in MainSocialLoginButtons)
			{
				button.onClick += () => RunSocialAuth(button.SocialProvider);
			}

			if (DeviceIDAuthButton)
				DeviceIDAuthButton.onClick += RunDeviceIDAuth;

			if (DemoController.Instance.IsAccessTokenAuth)
			{
				LoginAuthPage.SetActive(false);
				AccessTokenAuthPage.SetActive(true);

				DisableCommonButtons();
			}
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

			Action<string> onSuccessfulBasicAuth = token => SdkLoginLogic.Instance
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

			Action<string> onSuccessfulSocialAuth = token => SdkLoginLogic.Instance
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

			Action<string> onSuccessfulSteamAuth = token => SdkLoginLogic.Instance
				.ValidateToken(token, t => CompleteSuccessfulAuth(token, isSaveToken: true), ProcessError);
			Action<Error> onFailedSteamAuth = ProcessError;

			TryAuthBy<SteamAuth>(args: null, onSuccess: onSuccessfulSteamAuth, onFailed: onFailedSteamAuth);
		}

		private void PrepareAndRunAccessTokenAuth()
		{
			RunAccessTokenAuth(EmailAccessTokenAuthInputField.text);
		}

		public void RunAccessTokenAuth(string email)
		{
			if (IsAuthInProgress)
				return;

			var isEmailValid = ValidateEmail(email);

			if (isEmailValid)
			{
				IsAuthInProgress = true;
				PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => IsAuthInProgress == false);

				object[] args = { email };

				Action<string> onSuccessfulAccessTokenAuth = token => CompleteSuccessfulAuth(token, false, isPaystation: true);
				Action<Error> onFailedBasicAuth = ProcessError;

				TryAuthBy<AccessTokenAuth>(args, onSuccessfulAccessTokenAuth, onFailedBasicAuth);
			}
			else
			{
				Debug.Log($"Invalid email: {email}");
				Error error = new Error(errorType: ErrorType.RegistrationNotAllowedException, errorMessage: "Invalid email");
				base.OnError?.Invoke(error);
			}
		}

		public void RunDeviceIDAuth()
		{
			if (IsAuthInProgress)
				return;

			IsAuthInProgress = true;
			PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => IsAuthInProgress == false);

			Action<string> onSuccessfulDeviecIDAuth = token => SdkLoginLogic.Instance
				.ValidateToken(token, t => CompleteSuccessfulAuth(token, isSaveToken: true), ProcessError);
			Action<Error> onFailedDeviecIDAuth = ProcessError;

			TryAuthBy<DeviceIdAuth>(args: null, onSuccess: onSuccessfulDeviecIDAuth, onFailed: onFailedDeviecIDAuth);
		}

		private void DisableCommonButtons()
		{
			var buttonsProvider = GetComponent<LoginPageCommonButtonsProvider>();
			if (buttonsProvider != null)
			{
				if(buttonsProvider.DemoUserButton != null)
					buttonsProvider.DemoUserButton.gameObject.SetActive(false);
				if(buttonsProvider.LogInButton != null)
					buttonsProvider.LogInButton.gameObject.SetActive(false);
			}
		}
	}
}
