using System;
using Xsolla.Core;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Login;

public class LoginPageChangePasswordController : LoginPageController
{
#pragma warning disable 0649
	[SerializeField] InputField EmailInputField;
	[SerializeField] SimpleButton ChangePasswordButton;
#pragma warning restore 0649

	private bool IsPasswordChangeInProgress
	{
		get { return base.IsInProgress; }
		set
		{
			if (value == true)
			{
				if (base.OnStarted != null)
					base.OnStarted.Invoke();
				Debug.Log("LoginPageChangePasswordController: Password reset started");
			}
			else
				Debug.Log("LoginPageChangePasswordController: Password reset ended");

			base.IsInProgress = value;
		}
	}

	private void Awake()
	{
		if (ChangePasswordButton != null)
			ChangePasswordButton.onClick += PrepareAndRunPasswordChange;
	}

	private void PrepareAndRunPasswordChange()
	{
		RunPasswordChange(EmailInputField.text);
	}

	public void RunPasswordChange(string email)
	{
		if (IsPasswordChangeInProgress)
			return;

		IsPasswordChangeInProgress = true;

		Action onSuccessfulPasswordChange = () =>
		{
			Debug.Log("LoginPageChangePasswordController: Password change success");
			if (base.OnSuccess != null)
				base.OnSuccess.Invoke();
		};

		Action<Error> onFailedPasswordChange = error =>
		{
			Debug.LogError(string.Format("LoginPageChangePasswordController: Password change error: {0}", error.ToString()));
			if (base.OnError != null)
				base.OnError.Invoke(error);
		};

		DemoController.Instance.GetImplementation().ResetPassword(email, onSuccessfulPasswordChange, onFailedPasswordChange);
		IsPasswordChangeInProgress = false;
	}
}
