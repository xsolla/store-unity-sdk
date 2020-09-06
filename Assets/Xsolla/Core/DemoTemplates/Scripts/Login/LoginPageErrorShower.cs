using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

public class LoginPageErrorShower : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] Text ErrorText;
#pragma warning restore 0649

	public void ShowError(string errorMessage)
	{
		ErrorText.text = errorMessage;
	}

	public void ShowError(Error error)
	{
		if (error == null)
			ShowError("Unknown error");
		else if (!string.IsNullOrEmpty(error.errorMessage))
			ShowError(error.errorMessage);
		else
			ShowError(error.ErrorType.ToString());
	}
}
