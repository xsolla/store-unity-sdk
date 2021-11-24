using System;
using System.Linq;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public partial class LoginPageEnterController : LoginPageController
	{
		public bool IsAuthInProgress
		{
			get
			{
				return base.IsInProgress;
			}
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
			Debug.Log(string.Format("Trying {0}", auth.GetType().Name));
			auth.OnSuccess = token =>
			{
				Destroy(auth);
				if (onSuccess != null)
					onSuccess.Invoke(token);
			};
			auth.OnError = error =>
			{
				Destroy(auth);
				if (onFailed != null)
					onFailed.Invoke(error);
			};
			auth.TryAuth(args);
		}
	
		private void CompleteSuccessfulAuth(string encodedToken, bool isBasicAuth = false, bool isPaystation = false, bool isSaveToken = false)
		{
			if(isSaveToken)
				Token.Save();

			if (!isBasicAuth)
			{
				encodedToken = encodedToken.Split('&').First();
				Token.Instance = Token.Create(encodedToken);
			}

			Debug.Log(string.Format("Successful auth with token = {0}", encodedToken));
			MainMenuNicknameChecker.ResetFlag();
			IsAuthInProgress = false;
			if (base.OnSuccess != null)
				base.OnSuccess.Invoke();
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
				if (base.OnError != null)
					base.OnError.Invoke(error);
			}
		}

		public void RunLoginProxyAction(Action<LoginPageEnterController, object> action, object arg = null)
		{
			action.Invoke(this, arg);
		}
	}
}
