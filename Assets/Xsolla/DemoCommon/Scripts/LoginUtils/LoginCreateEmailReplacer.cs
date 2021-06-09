using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class LoginCreateEmailReplacer : MonoBehaviour
	{
		[SerializeField] Text EmailText = default;

		// Start is called before the first frame update
		void Start()
		{
			var createAccountEmail = LoginPageCreateController.LastEmail;

			if (!string.IsNullOrEmpty(createAccountEmail))
			{
				var currentMessage = EmailText.text;
				var modifiedMessage = currentMessage.Replace(Constants.EMAIL_TEXT_TEMPLATE, createAccountEmail);
				EmailText.text = modifiedMessage;
			}

			LoginPageCreateController.DropLastCredentials();
		}
	}
}
