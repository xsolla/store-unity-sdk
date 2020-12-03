using System;
using System.Collections;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;

public class UserProfilePasswordReset : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] SimpleButton PasswordEditButton;
#pragma warning restore 0649

	private const string MESSAGE_TEMPLATE = "Change your password using the instructions we sent to <color=#F8115D>{email@domain.com}</color>.";

	private void Start()
	{
		PasswordEditButton.onClick = () => StartCoroutine(ResetPassword());
	}

	private IEnumerator ResetPassword()
	{
		bool? isInfoObtained = null;

		string email = null;

		var token = DemoController.Instance.GetImplementation().Token;
		DemoController.Instance.GetImplementation().GetUserInfo(token,
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

		DemoController.Instance.GetImplementation().ResetPassword(email, onSuccessfulPasswordChange, onFailedPasswordChange);
	}
}
