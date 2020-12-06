using System;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;
using Xsolla.Login;

namespace Xsolla.Demo
{
	public class MainMenuNicknameChecker : MonoBehaviour
	{
		private static bool _isFirstLaunch = true;

		public static void ResetFlag()
		{
			_isFirstLaunch = true;
		}

		private void Start()
		{
			if (_isFirstLaunch)
			{
				_isFirstLaunch = false;
				CheckNicknamePresence();
			}
		}

		private void CheckNicknamePresence()
		{
			var token = DemoController.Instance.LoginDemo.Token;
			DemoController.Instance.LoginDemo.GetUserInfo(token, info =>
			{
				if (string.IsNullOrEmpty(info.nickname))
				{
					var isUserEmailRegistration = !string.IsNullOrEmpty(info.email) && !string.IsNullOrEmpty(info.username);

					if (isUserEmailRegistration)
						SetNickname(info.username);
					else if (XsollaSettings.RequestNicknameOnAuth)
						RequestNickname();
				}
			});
		}

		private void RequestNickname()
		{
			PopupFactory.Instance.CreateNickname().SetCallback(SetNickname).SetCancelCallback(() => { });
		}

		private void SetNickname(string newNickname)
		{
			var isNicknameUpdateInProgress = true;
			ShowWaiting(() => isNicknameUpdateInProgress);

			var token = DemoController.Instance.LoginDemo.Token;
			var updatePack = new UserInfoUpdate() { nickname = newNickname };

			DemoController.Instance.LoginDemo.UpdateUserInfo(token, updatePack,
				onSuccess: _ => isNicknameUpdateInProgress = false,
				onError: error =>
				{
					Debug.LogError("Could not update user info");
					isNicknameUpdateInProgress = false;
					StoreDemoPopup.ShowError(error);
				});
		}

		private void ShowWaiting(Func<bool> waitWhile)
		{
			PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => !waitWhile.Invoke());
		}
	}
}
