using Xsolla.Core;

namespace Xsolla.Demo
{
	public class ConsolePlatformAuth : LoginAuthorization
	{
		public override void TryAuth(params object[] args)
		{
			if (XsollaSettings.UseConsoleAuth)
			{
				Debug.Log("ConsolePlatformAuth.TryAuth: Console auth enabled, trying to get token");
				RequestToken();
			}
			else
			{
				Debug.Log("ConsolePlatformAuth.TryAuth: Console auth disabled");
				if (base.OnError != null)
					base.OnError.Invoke(null);
			}
		}

		private void RequestToken()
		{
			SdkLoginLogic.Instance.SignInConsoleAccount(
				XsollaSettings.UsernameFromConsolePlatform,
				XsollaSettings.Platform.GetString(),
				SuccessHandler,
				FailHandler);
		}

		private void SuccessHandler(string token)
		{
			Debug.Log("ConsolePlatformAuth.SuccessHandler: Token loaded");
			if (base.OnSuccess != null)
				base.OnSuccess.Invoke(token);
		}

		private void FailHandler(Error error)
		{
			Debug.LogError(string.Format("Failed request token by console account with user = `{0}` and platform = `{1}`. Error:{2}",
				XsollaSettings.UsernameFromConsolePlatform,
				XsollaSettings.Platform.GetString(),
				error.ToString()));
			if (base.OnError != null)
				base.OnError.Invoke(error);
		}
	}
}
