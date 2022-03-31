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
		[SerializeField] SimpleButton DeviceIDAuthButton = default;

		private void Awake()
		{
			LoginButton.onClick += PrepareAndRunBasicAuth;

			SocialNetworksWidget.OnSocialButtonClick = RunSocialAuth;
			OtherSocialNetworksButton.onClick += () => SocialNetworksWidget.gameObject.SetActive(true);

			if (PasswordlessWidget)
			{
				PasswordlessButton.onClick += () => PasswordlessWidget.gameObject.SetActive(true);
				PasswordlessWidget.OnPhoneAccessRequest += phone => RunPasswordlessAuth<PasswordlessPhoneAuth>(phone);
				PasswordlessWidget.OnEmailAccessRequest += email => RunPasswordlessAuth<PasswordlessEmailAuth>(email);
			}

			foreach (var button in MainSocialLoginButtons)
			{
				button.onClick += () => RunSocialAuth(button.SocialProvider);
			}

			if (DeviceIDAuthButton)
				DeviceIDAuthButton.onClick += RunDeviceIDAuth;
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

			Action<string> onSuccessfulBasicAuth = token => SdkAuthLogic.Instance
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

			Action<string> onSuccessfulSocialAuth = token => SdkAuthLogic.Instance
				.ValidateToken(token, t => CompleteSuccessfulAuth(token, isSaveToken: true), ProcessError);
			Action<Error> onFailedSocialAuth = ProcessError;

	#if UNITY_EDITOR || UNITY_STANDALONE
			TryAuthBy<SocialAuth>(args, onSuccessfulSocialAuth, onFailedSocialAuth);
	#elif UNITY_ANDROID
			TryAuthBy<AndroidSocialAuth>(args, onSuccessfulSocialAuth, onFailedSocialAuth);
	#elif UNITY_IOS
			TryAuthBy<IosSocialAuth>(args, onSuccessfulSocialAuth, onFailedSocialAuth);
	#endif
		}

		public void RunManualSteamAuth()
		{
			if (IsAuthInProgress)
				return;

			IsAuthInProgress = true;
			PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => IsAuthInProgress == false);

			Action<string> onSuccessfulSteamAuth = token => SdkAuthLogic.Instance
				.ValidateToken(token, t => CompleteSuccessfulAuth(token, isSaveToken: true), ProcessError);
			Action<Error> onFailedSteamAuth = ProcessError;

			TryAuthBy<SteamAuth>(args: null, onSuccess: onSuccessfulSteamAuth, onFailed: onFailedSteamAuth);
		}

		public void RunDeviceIDAuth()
		{
			if (IsAuthInProgress)
				return;

			IsAuthInProgress = true;
			PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => IsAuthInProgress == false);

			Action<string> onSuccessfulDeviecIDAuth = token => SdkAuthLogic.Instance
				.ValidateToken(token, t => CompleteSuccessfulAuth(token, isSaveToken: true), ProcessError);
			Action<Error> onFailedDeviecIDAuth = ProcessError;

			TryAuthBy<DeviceIdAuth>(args: null, onSuccess: onSuccessfulDeviecIDAuth, onFailed: onFailedDeviecIDAuth);
		}

		public void RunPasswordlessAuth<T>(string value) where T : LoginAuthorization
		{
			var passwordlessAuth = GetComponent<T>();
			if (passwordlessAuth != null)
			{
				passwordlessAuth.TryAuth(PasswordlessWidget, value);
				return;
			}

			Action<string> onSuccessfulPasswordlessAuth = token => SdkAuthLogic.Instance
				.ValidateToken(token, t => CompleteSuccessfulAuth(token, isSaveToken: true), ProcessError);
			Action<Error> onFailedPasswordlessAuth = ProcessError;

			TryAuthBy<T>(args: new object[] { PasswordlessWidget, value }, onSuccess: onSuccessfulPasswordlessAuth, onFailed: onFailedPasswordlessAuth);
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
