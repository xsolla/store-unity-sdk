using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	internal class SignInWidget : MonoBehaviour
	{
		[field: SerializeField] private TMP_InputField EmailInputField { get; set; }
		[field: SerializeField] private TMP_InputField PasswordInputField { get; set; }
		[field: SerializeField] private Button SignInButton { get; set; }
		[field: SerializeField] private Button TouristModeButton { get; set; }

		private IAuthService AuthService => ServiceLocator.Resolve<IAuthService>();
		private ScreenService ScreenService => ServiceLocator.Resolve<ScreenService>();
		private GameStateMachine GameStateMachine => ServiceLocator.Resolve<GameStateMachine>();

		private void Start()
		{
			SignInButton.onClick.AddListener(StartSignIn);
			TouristModeButton.onClick.AddListener(SignInTouristMode);
		}

		private void StartSignIn()
		{
			var email = EmailInputField.text;
			var password = PasswordInputField.text;

			var loadingOverlay = ScreenService.OpenLoadingOverlay();

			AuthService.SignIn(
				email,
				password,
				() => GameStateMachine.SwitchToUserAuthFinish(),
				errorMessage => {
					ScreenService.Close(loadingOverlay);
					ScreenService
						.OpenInfoPopup()
						.SetTitle("Sign-in Error")
						.SetMessage(errorMessage);
				});
		}

		private void SignInTouristMode()
		{
			// TODO Implement tourist mode
			var popup = ScreenService
				.OpenInfoPopup()
				.SetTitle("NOT IMPLEMENTED YET")
				.SetMessage("NOT IMPLEMENTED YET");
			popup.SetCloseCallback(() => ScreenService.Close(popup));
		}
	}
}