using UnityEngine;
using UnityEngine.UI;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Samples.Authorization
{
	public class ResetPasswordPage : MonoBehaviour
	{
		// Declaration of variables for UI elements
		public InputField UsernameInput;
		public Button ResetPasswordButton;

		private void Start()
		{
			// Handling the button click
			ResetPasswordButton.onClick.AddListener(() =>
			{
				// Get the username from the input field
				var username = UsernameInput.text;

				// Call the password reset method
				// Pass username and callback functions for success and error cases
				XsollaAuth.ResetPassword(username, OnSuccess, OnError);
			});
		}

		private void OnSuccess()
		{
			Debug.Log("Password reset successful");
			// Some actions
		}

		private void OnError(Error error)
		{
			Debug.LogError($"Password reset failed. Error: {error.errorMessage}");
			// Some actions
		}
	}
}