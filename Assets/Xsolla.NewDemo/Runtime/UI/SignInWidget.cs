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
			SignInButton.onClick.AddListener(SignIn);
			TouristModeButton.onClick.AddListener(SignInTouristMode);
		}

		private void OnDestroy()
		{
			SignInButton.onClick.RemoveAllListeners();
			TouristModeButton.onClick.RemoveAllListeners();
		}

		private void SignIn()
		{
			var loadingOverlay = ScreenService.OpenLoadingOverlay();
			SignInButton.interactable = false;

			AuthService.SignIn(
				EmailInputField.text,
				PasswordInputField.text,
				() => GameStateMachine.SwitchToUserAuthFinish(),
				errorMessage => OnAuthError(errorMessage, loadingOverlay));
		}

		private void SignInTouristMode()
		{
			var loadingOverlay = ScreenService.OpenLoadingOverlay();

			AuthService.SignInTouristMode(
				() => GameStateMachine.SwitchToUserAuthFinish(),
				errorMessage => OnAuthError(errorMessage, loadingOverlay));
		}

		private void OnAuthError(string errorMessage, Screen loadingOverlay)
		{
			ScreenService.Close(loadingOverlay);

			ScreenService
				.OpenInfoPopup()
				.SetTitle("Sign-in Error")
				.SetMessage(errorMessage)
				.SetCloseCallback(() => SignInButton.interactable = true);
		}
	}
}