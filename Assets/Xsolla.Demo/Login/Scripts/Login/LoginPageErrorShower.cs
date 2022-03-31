using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class LoginPageErrorShower : MonoBehaviour
	{
		[SerializeField] Text ErrorText = default;
		[SerializeField] private SimpleTextButton ResendEmailButton = default;

		private void Awake()
		{
			if (ResendEmailButton)
			{
				ResendEmailButton.onClick += () => SdkAuthLogic.Instance.ResendConfirmationLink(LoginPageEnterController.LastUsername);
				ResendEmailButton.gameObject.SetActive(false);
			}
		}

		private void DisableCommonButtons()
		{
			var buttonsProvider = GetComponent<LoginPageCommonButtonsProvider>();
			if (buttonsProvider != null)
			{
				if (buttonsProvider.DemoUserButton != null)
					buttonsProvider.DemoUserButton.gameObject.SetActive(false);
				if (buttonsProvider.LogInButton != null)
					buttonsProvider.LogInButton.gameObject.SetActive(false);
			}
		}

		public void ShowError(string errorMessage)
		{
			ErrorText.text = errorMessage;
		}

		public void ShowError(Error error)
		{
			if (ResendEmailButton)
				ResendEmailButton.gameObject.SetActive(error != null && error.ErrorType == ErrorType.UserIsNotActivated);

			if (error == null)
				ShowError("Unknown error");
			else if (!string.IsNullOrEmpty(error.errorMessage))
				ShowError(error.errorMessage);
			else
				ShowError(error.ErrorType.ToString());
		}
	}
}
