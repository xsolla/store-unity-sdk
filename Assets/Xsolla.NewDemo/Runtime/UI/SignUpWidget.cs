using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	internal class SignUpWidget : MonoBehaviour
	{
		[field: SerializeField] private TMP_InputField UsernameInputField { get; set; }
		[field: SerializeField] private TMP_InputField EmailInputField { get; set; }
		[field: SerializeField] private TMP_InputField PasswordInputField { get; set; }
		[field: SerializeField] private Button SignUpButton { get; set; }

		private IAuthService AuthService => ServiceLocator.Resolve<IAuthService>();
		private ScreenService ScreenService => ServiceLocator.Resolve<ScreenService>();

		private void Start()
		{
			SignUpButton.onClick.AddListener(TrySignUp);
		}

		private void OnDestroy()
		{
			SignUpButton.onClick.RemoveAllListeners();
		}

		private void TrySignUp()
		{
			var username = UsernameInputField.text;
			var email = EmailInputField.text;
			var password = PasswordInputField.text;

			var loadingOverlay = ScreenService.OpenLoadingOverlay();
			SignUpButton.interactable = false;

			AuthService.Register(
				username,
				email,
				password,
				() => {
					SignUpButton.interactable = true;
					ScreenService.Close(loadingOverlay);
					ScreenService
						.OpenInfoPopup()
						.SetTitle("Sign-up Successful")
						.SetMessage("Your account has been created successfully. Please check your email to verify your account.")
						.SetCloseCallback(() => {
							SignUpButton.interactable = true;
							ScreenService.OpenUserAuthScreen().ToggleMode(UserAuthScreen.EMode.SignIn);
						});
				},
				errorMessage => {
					ScreenService.Close(loadingOverlay);
					ScreenService
						.OpenInfoPopup()
						.SetTitle("Sign-up Error")
						.SetMessage(errorMessage)
						.SetCloseCallback(() => SignUpButton.interactable = true);
				});
		}
	}
}