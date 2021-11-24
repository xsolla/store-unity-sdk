using System;
using Xsolla.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class LoginPageChangePasswordController : LoginPageController
	{
		[SerializeField] InputField EmailInputField;
		[SerializeField] SimpleButton ChangePasswordButton;

		public static string LastEmail { get; private set; }

		private bool IsPasswordChangeInProgress
		{
			get
			{
				return base.IsInProgress;
			}
			set
			{
				if (value)
					Debug.Log("LoginPageChangePasswordController: Password reset started");
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

			LastEmail = email;

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

			SdkLoginLogic.Instance.ResetPassword(email, onSuccessfulPasswordChange, onFailedPasswordChange);
			IsPasswordChangeInProgress = false;
		}
	}
}
