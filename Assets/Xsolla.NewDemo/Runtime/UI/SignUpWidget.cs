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

		private void TrySignUp()
		{
			var username = UsernameInputField.text;
			var email = EmailInputField.text;
			var password = PasswordInputField.text;

			var loadingOverlay = ScreenService.OpenLoadingOverlay();

			AuthService.Register(
				username,
				email,
				password,
				() => {
					ScreenService.Close(loadingOverlay);
					ScreenService
						.OpenInfoPopup()
						.SetTitle("Sign-up Successful")
						.SetMessage("Your account has been created successfully. Please check your email to verify your account.")
						.SetCloseCallback(() => ScreenService.OpenUserAuthScreen().ToggleMode(UserAuthScreen.EMode.SignIn));
				},
				errorMessage => {
					ScreenService.Close(loadingOverlay);
					ScreenService
						.OpenInfoPopup()
						.SetTitle("Sign-up Error")
						.SetMessage(errorMessage);
				});
		}
	}
}