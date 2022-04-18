using System;
using System.Linq;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public partial class LoginPageEnterController : LoginPageController
	{
		public static string LastUsername { get; set; }
		
		public bool IsAuthInProgress
		{
			get => base.IsInProgress;

			set
			{
				if (value)
					Debug.Log("LoginPageEnterController: Authentication process started");
				else
					Debug.Log("LoginPageEnterController: Authentication process ended");

				base.IsInProgress = value;
			}
		}

		private void TryAuthBy<T>(object[] args, Action<string> onSuccess = null, Action<Error> onFailed = null) where T : LoginAuthorization
		{
			T auth = base.gameObject.AddComponent<T>();
			Debug.Log($"Trying {auth.GetType().Name}");
			auth.OnSuccess = token => { Destroy(auth); onSuccess?.Invoke(token); };
			auth.OnError = error => { Destroy(auth); onFailed?.Invoke(error); };
			auth.TryAuth(args);
		}
	
		private void CompleteSuccessfulAuth(string encodedToken, bool isBasicAuth = false, bool isPaystation = false, bool isSaveToken = false)
		{
			if (!isBasicAuth)
			{
				encodedToken = encodedToken.Split('&').First();
				Token.Instance = Token.Create(encodedToken);
			}

			if(isSaveToken)
				Token.Save();

			Debug.Log($"Successful auth with token = {encodedToken}");
			MainMenuNicknameChecker.ResetFlag();
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

		public void RunLoginProxyAction(Action<LoginPageEnterController, object> action, object arg = null)
		{
			action.Invoke(this, arg);
		}
	}
}
