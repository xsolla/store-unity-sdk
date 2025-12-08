using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class UserAuthScreen : Screen
	{
		[field: SerializeField] private Button SignInTabButton { get; set; }
		[field: SerializeField] private Button SignUpTapButton { get; set; }
		[field: SerializeField] private TabButton SignInTabButtonWidget { get; set; }
		[field: SerializeField] private TabButton SignUpTabButtonWidget { get; set; }

		[field: Space]
		[field: SerializeField] private SignInWidget SignInWidget { get; set; }
		[field: SerializeField] private SignUpWidget SignUpWidget { get; set; }
		[field: SerializeField] private Button GamerSupportButton { get; set; }
		[field: Space]
		[field: SerializeField] private GameObject SocialSignInContainer { get; set; }
		[field: SerializeField] private Button GoogleSignInButton { get; set; }
		[field: SerializeField] private Button FacebookSignInButton { get; set; }

		private UrlService UrlService => ServiceLocator.Resolve<UrlService>();
		private ScreenService ScreenService => ServiceLocator.Resolve<ScreenService>();
		private IAuthService AuthService => ServiceLocator.Resolve<IAuthService>();
		private GameStateMachine GameStateMachine => ServiceLocator.Resolve<GameStateMachine>();

		private void Start()
		{
			ToggleMode(EMode.SignIn);

			SignInTabButton.onClick.AddListener(() => ToggleMode(EMode.SignIn));
			SignUpTapButton.onClick.AddListener(() => ToggleMode(EMode.SignUp));
			GamerSupportButton.onClick.AddListener(() => UrlService.OpenGamerSupport());

			GoogleSignInButton.onClick.AddListener(() => AuthViaSocialNetwork("google"));
			FacebookSignInButton.onClick.AddListener(() => AuthViaSocialNetwork("facebook"));
		}

		public void ToggleMode(EMode mode)
		{
			SignInTabButtonWidget.SwitchState(mode == EMode.SignIn);
			SignUpTabButtonWidget.SwitchState(mode == EMode.SignUp);

			SignInWidget.gameObject.SetActive(mode == EMode.SignIn);
			SignUpWidget.gameObject.SetActive(mode == EMode.SignUp);

			SocialSignInContainer.SetActive(mode == EMode.SignIn);
		}

		private void AuthViaSocialNetwork(string socialNetwork)
		{
			AuthService.AuthViaSocialNetwork(
				socialNetwork,
				GameStateMachine.SwitchToUserAuthFinish,
				error => {
					ScreenService
						.OpenInfoPopup()
						.SetTitle("Social Sign-In Error")
						.SetMessage(error);
				},
				null);
		}

		public enum EMode
		{
			SignIn,
			SignUp
		}
	}
}