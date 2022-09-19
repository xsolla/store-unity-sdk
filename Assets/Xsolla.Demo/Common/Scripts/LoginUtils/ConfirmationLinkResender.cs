using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class ConfirmationLinkResender : MonoBehaviour
	{
		[SerializeField] SimpleButton resendButton = default;
		[SerializeField] Text emailText = default;

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
					Debug.LogWarning($"Unexpected state");
					break;
			}
		}

		public void ResendRegistrationConfirmationEmail()
		{
			SdkAuthLogic.Instance.ResendConfirmationLink(LoginPageCreateController.LastEmail);
		}

		public void ResendPasswordResetEmail()
		{
			SdkAuthLogic.Instance.ResetPassword(LoginPageChangePasswordController.LastEmail);
		}

		private void ReplaceEmailText(string email)
		{
			if (!string.IsNullOrEmpty(email))
			{
				var currentMessage = emailText.text;
				var modifiedMessage = currentMessage.Replace("{email@domen.com}", email);
				emailText.text = modifiedMessage;
			}
		}
	}
}