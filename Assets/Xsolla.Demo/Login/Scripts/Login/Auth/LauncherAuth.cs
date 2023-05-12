using System;
using System.Linq;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class LauncherAuth : LoginAuthorization
	{
		private const string LAUNCHER_TOKEN_KEY = "xsolla-login-token";

		public override void TryAuth(object[] _, Action onSuccess, Action<Error> onError)
		{
			var launcherToken = GetLauncherToken();
			if (string.IsNullOrEmpty(launcherToken))
			{
				onError?.Invoke(null);
				return;
			}

			XsollaToken.Create(launcherToken);
			onSuccess?.Invoke();
		}

		private static string GetLauncherToken()
		{
			var commandLineArgs = Environment.GetCommandLineArgs();
			if (!commandLineArgs.Any(x => x.Contains(LAUNCHER_TOKEN_KEY)))
				return null;

			var tokenParamValueIndex = default(int);
			for (var i = 0; i < commandLineArgs.Length; i++)
			{
				if (commandLineArgs[i].Contains(LAUNCHER_TOKEN_KEY))
				{
					tokenParamValueIndex = i + 1;
					break;
				}
			}

			if (tokenParamValueIndex < commandLineArgs.Length)
				return commandLineArgs[tokenParamValueIndex];

			return null;
		}
	}
}