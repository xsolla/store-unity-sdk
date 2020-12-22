using System;
using System.Linq;
using Newtonsoft.Json.Utilities;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class LauncherArguments : MonoSingleton<LauncherArguments>
	{
		const string LAUNCHER_TOKEN = "xsolla-login-token";

		private bool invalidated = false;

		public string GetToken()
		{
			var commandLineArgs = Environment.GetCommandLineArgs();

			if (invalidated || commandLineArgs.Count(a => a.Contains(LAUNCHER_TOKEN)) == 0)
			{
				return string.Empty;
			}

			var tokenParamValueIndex = commandLineArgs.IndexOf(a => a.Contains(LAUNCHER_TOKEN)) + 1;

			if (tokenParamValueIndex < commandLineArgs.Length)
			{
				return commandLineArgs[tokenParamValueIndex];
			}

			return string.Empty;
		}

		public void InvalidateTokenArguments()
		{
			invalidated = true;
		}
	}
}
