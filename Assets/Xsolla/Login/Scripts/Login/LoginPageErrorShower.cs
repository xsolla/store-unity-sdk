using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class LoginPageErrorShower : MonoBehaviour
	{
		[SerializeField] Text ErrorText = default;

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
}
