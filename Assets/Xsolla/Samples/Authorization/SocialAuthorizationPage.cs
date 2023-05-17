using UnityEngine;
using UnityEngine.UI;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Samples.Authorization
{
	public class SocialAuthorizationPage : MonoBehaviour
	{
		// Declaration of variables for SocialProvider and signIn button
		public SocialProvider SocialProvider = SocialProvider.Facebook;
		public Button SignInButton;

		private void Start()
		{
			// Handling the button click
			SignInButton.onClick.AddListener(() =>
			{
				// Call the social authorization method
				// Pass the social network provider and callback functions for success and error cases
				XsollaAuth.AuthViaSocialNetwork(SocialProvider, OnSuccess, OnError, OnCancel);
			});
		}

		private void OnSuccess()
		{
			Debug.Log("Social authorization successful");
			// Some actions
		}

		private void OnError(Error error)
		{
			Debug.LogError($"Social authorization failed. Error: {error.errorMessage}");
			// Some actions
		}

		private void OnCancel()
		{
			Debug.Log("Social authorization cancelled by user.");
			// Some actions
		}
	}
}