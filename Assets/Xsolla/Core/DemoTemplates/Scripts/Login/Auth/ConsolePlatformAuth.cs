using UnityEngine;
using Xsolla.Core;
using Xsolla.Store;

namespace Xsolla.Demo
{
	public class ConsolePlatformAuth : StoreStringActionResult, ILoginAuthorization
	{
		public void TryAuth(params object[] args)
		{
			if (XsollaSettings.UseConsoleAuth)
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
			DemoController.Instance.GetImplementation().SignInConsoleAccount(
				userId: XsollaSettings.UsernameFromConsolePlatform,
				platform: XsollaSettings.Platform.GetString(),
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
			Debug.LogError($"Failed request token by console account with user = `{XsollaSettings.UsernameFromConsolePlatform}` and platform = `{XsollaSettings.Platform.GetString()}`. Error:{error.ToString()}");
			base.OnError?.Invoke(error);
		}
	}
}
