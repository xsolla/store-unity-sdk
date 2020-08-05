using System;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;

/// <summary>
/// Class that try load saved token, which is saved after completion
/// last login.
/// </summary>
public class OldSavedTokenAuth : MonoBehaviour, OldILoginAuthorization
{
	public Action<string> OnSuccess { get; set; }
	public Action OnFailed { get; set; }

	private void Start()
	{
		if (XsollaLogin.Instance.LoadToken(Constants.LAST_SUCCESS_AUTH_TOKEN, out var token))
			ValidateSavedToken(token);
		else
			OnFailed?.Invoke();
		Destroy(this, 0.1F);
	}

	private void ValidateSavedToken(string token)
	{
		Action<UserInfo> onValidationSuccess = _ =>
		{
			Debug.Log("Saved token validation success");
			OnSuccess?.Invoke(token);
		};

		Action<Error> onValidationError = _ =>
		{
			Debug.Log("Saved token validation error (probably expired)");
			OnFailed?.Invoke();
		};

		XsollaLogin.Instance.GetUserInfo(token, onValidationSuccess, onValidationError);
	}
}
