using UnityEngine;
using UnityEngine.UI;

public class LoginCreateEmailReplacer : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] Text EmailText;
#pragma warning restore 0649

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
