using Xsolla.Core;

namespace Xsolla.Demo
{
	public class ConsolePlatformAuth : LoginAuthorization
	{
		public override void TryAuth(params object[] args)
		{
			if (DemoSettings.UseConsoleAuth)
			{
				Debug.Log("ConsolePlatformAuth.TryAuth: Console auth enabled, trying to get token");
				RequestToken();
			}
			else
			{
				Debug.Log("ConsolePlatformAuth.TryAuth: Console auth disabled");
				base.OnError?.Invoke(null);
			}
		}

		private void RequestToken()
		{
			SdkAuthLogic.Instance.SignInConsoleAccount(
				userId: DemoSettings.UsernameFromConsolePlatform,
				platform: DemoSettings.Platform.GetString(),
				SuccessHandler,
				FailHandler);
		}

		private void SuccessHandler(string token)
		{
			Debug.Log("ConsolePlatformAuth.SuccessHandler: Token loaded");
			base.OnSuccess?.Invoke(token);
		}

		private void FailHandler(Error error)
		{
			Debug.LogError($"Failed request token by console account with user = `{DemoSettings.UsernameFromConsolePlatform}` and platform = `{DemoSettings.Platform.GetString()}`. Error:{error.ToString()}");
			base.OnError?.Invoke(error);
		}
	}
}
