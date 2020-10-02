using System;
using System.Linq;
using UnityEngine;
using Xsolla.Core;

public partial class LoginPageEnterController : LoginPageController
{
	private bool IsAuthInProgress
	{
		get => base.IsInProgress;

		set
		{
			if (value == true)
			{
				base.OnStarted?.Invoke();
				Debug.Log("LoginPageEnterController: Authentication process started");
			}
			else
				Debug.Log("LoginPageEnterController: Authentication process ended");

			base.IsInProgress = value;
		}
	}

	private void TryAuthBy<T>(object[] args, Action<string> onSuccess = null, Action<Error> onFailed = null) where T : MonoBehaviour, ILoginAuthorization
	{
		T auth = base.gameObject.AddComponent<T>();
		Debug.Log($"Trying {auth.GetType().Name}");
		auth.OnSuccess = token => { Destroy(auth); onSuccess?.Invoke(token); };
		auth.OnError = error => { Destroy(auth); onFailed?.Invoke(error); };
		auth.TryAuth(args);
	}
	
	private void CompleteSuccessfulAuth(string token, bool isBasicAuth = false, bool isPaystation = false, bool isSteam = false, bool isSaveToken = false)
	{
		if(isSaveToken)
			DemoController.Instance.GetImplementation().SaveToken(Constants.LAST_SUCCESS_AUTH_TOKEN, token);

		if (!isBasicAuth)
		{
			token = token.Split('&').First();

			var jwtToken = new Token(token, isPaystation);
			jwtToken.FromSteam = isSteam;
			DemoController.Instance.GetImplementation().Token = jwtToken;
		}

		Debug.Log($"Successful auth with token = {token}");
		IsAuthInProgress = false;
		base.OnSuccess?.Invoke();
	}

	private void ProcessError(Error error)
	{
		IsAuthInProgress = false;

		if (error == null)
		{
			//Do nothing, it means that chosen auth was not completed, but ended without any errors (example: cancelled SocialAuth)
		}
		else
		{
			base.OnError?.Invoke(error);
		}
	}

	public void RunLoginAction(Action<LoginPageEnterController, object> action, object arg = null)
	{
		action.Invoke(this, arg);
	}
}