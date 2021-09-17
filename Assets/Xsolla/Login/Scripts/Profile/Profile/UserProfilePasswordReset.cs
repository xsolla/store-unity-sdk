using System;
using System.Collections;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;

namespace Xsolla.Demo
{
	public class UserProfilePasswordReset : MonoBehaviour
	{
		[SerializeField] SimpleButton PasswordEditButton = default;

		private const string MESSAGE_TEMPLATE = "Change your password using the instructions we sent to <color=#F8115D>{email@domain.com}</color>.";

		private void Start()
		{
			PasswordEditButton.onClick = () => StartCoroutine(ResetPassword());
		}

		private IEnumerator ResetPassword()
		{
			bool? isInfoObtained = null;

			string email = null;

			var token = Token.Instance;
			SdkLoginLogic.Instance.GetUserInfo(token,
				onSuccess: info =>
				{
					email = info.email;
					isInfoObtained = true;
				},
				onError: error =>
				{
					StoreDemoPopup.ShowError(error);
					isInfoObtained = false;
				});

			yield return new WaitWhile(() => isInfoObtained == null);

			if (isInfoObtained == false)
				yield break;

			Action onSuccessfulPasswordChange = () =>
			{
				Debug.Log("Password reset request success");
				var message = MESSAGE_TEMPLATE.Replace("{email@domain.com}", email);
				PopupFactory.Instance.CreateSuccessPasswordReset().SetMessage(message);
			};

			Action<Error> onFailedPasswordChange = error =>
			{
				StoreDemoPopup.ShowError(error);
			};

			SdkLoginLogic.Instance.ResetPassword(email, onSuccessfulPasswordChange, onFailedPasswordChange);
		}
	}
}
