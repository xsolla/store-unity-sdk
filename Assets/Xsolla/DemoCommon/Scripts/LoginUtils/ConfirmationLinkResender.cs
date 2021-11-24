using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Demo;
using Debug = UnityEngine.Debug;

public class ConfirmationLinkResender : MonoBehaviour
{
	[SerializeField] SimpleButton resendButton;
	[SerializeField] Text emailText;

	void Awake()
	{
		switch (DemoController.Instance.GetState())
		{
			case MenuState.RegistrationSuccess:
				resendButton.onClick += ResendRegistrationConfirmationEmail;
				ReplaceEmailText(LoginPageCreateController.LastEmail);
				break;
			case MenuState.ChangePasswordSuccess:
				resendButton.onClick += ResendPasswordResetEmail;
				ReplaceEmailText(LoginPageChangePasswordController.LastEmail);
				break;
			default:
				Debug.LogWarning("Unexpected state");
				break;
		}
	}

	public void ResendRegistrationConfirmationEmail()
	{
		SdkLoginLogic.Instance.ResendConfirmationLink(LoginPageCreateController.LastEmail);
	}

	public void ResendPasswordResetEmail()
	{
		SdkLoginLogic.Instance.ResetPassword(LoginPageChangePasswordController.LastEmail);
	}

	private void ReplaceEmailText(string email)
	{
		if (!string.IsNullOrEmpty(email))
		{
			var currentMessage = emailText.text;
			var modifiedMessage = currentMessage.Replace(Constants.EMAIL_TEXT_TEMPLATE, email);
			emailText.text = modifiedMessage;
		}
	}
}
