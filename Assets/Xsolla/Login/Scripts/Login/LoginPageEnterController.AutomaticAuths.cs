using System;
using Xsolla.Core;
using Xsolla.Core.Popup;

namespace Xsolla.Demo
{
	public partial class LoginPageEnterController : LoginPageController
	{
		private static bool _isFirstLaunch = true;
		private int _autoAuthState = 0;

		private void Start()
		{
			if (_isFirstLaunch)
			{
				_isFirstLaunch = false;
				IsAuthInProgress = true;
				PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => IsAuthInProgress == false);
				RunAutomaticAuths();
			}
			else
			{
				DemoController.Instance.LoginDemo.DeleteToken(Constants.LAST_SUCCESS_AUTH_TOKEN);
			}
		}

		private void RunAutomaticAuths()
		{
			Action<Error> onFailedAutomaticAuth = error =>
			{
				if (error != null)
				{
					ProcessError(error);
				}
				else
				{
					_autoAuthState++;
					RunAutomaticAuths();
				}
			};

			Action<string> onSuccessfulAutomaticAuth = token =>
				DemoController.Instance.LoginDemo.ValidateToken(token, onSuccess: validToken => CompleteSuccessfulAuth(validToken), onError: _ => onFailedAutomaticAuth.Invoke(null));

			switch (_autoAuthState)
			{
				case 0:
					TryAuthBy<SavedTokenAuth>(args: null, onSuccess: onSuccessfulAutomaticAuth, onFailed: onFailedAutomaticAuth);
					break;
				case 1:
					TryAuthBy<LauncherAuth>(args: null, onSuccess: onSuccessfulAutomaticAuth, onFailed: onFailedAutomaticAuth);
					break;
				case 2:
					TryAuthBy<ConsolePlatformAuth>(args: null, onSuccess: onSuccessfulAutomaticAuth, onFailed: onFailedAutomaticAuth);
					break;
				//case 3:
				//	TryAuthBy<SteamAuth>(args: null, onSuccess: onSuccessfulAutomaticAuth, onFailed: onFailedAutomaticAuth);
				//	break;
				default:
					IsAuthInProgress = false;
					break;
			}
		}
	}
}
