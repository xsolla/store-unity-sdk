using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class LoginCreateEmailReplacer : MonoBehaviour
	{
		[SerializeField] Text EmailText = default;

		private string _emailTextTemplate = "{email@domen.com}";

		// Start is called before the first frame update
		void Start()
		{
			var createAccountEmail = LoginPageCreateController.LastEmail;

			if (!string.IsNullOrEmpty(createAccountEmail))
			{
				var currentMessage = EmailText.text;
				var modifiedMessage = currentMessage.Replace(_emailTextTemplate, createAccountEmail);
				EmailText.text = modifiedMessage;
			}

			LoginPageCreateController.DropLastCredentials();
		}
	}
}
