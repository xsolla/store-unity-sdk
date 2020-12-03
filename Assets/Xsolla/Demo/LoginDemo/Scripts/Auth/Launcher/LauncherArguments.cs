using System;
using System.Linq;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class LauncherArguments : MonoSingleton<LauncherArguments>
	{
		const string LAUNCHER_TOKEN = "xsolla-login-jwt";

		private bool invalidated = false;

		public string GetToken()
		{
			if(invalidated || Environment.GetCommandLineArgs().Count(a => a.Contains(LAUNCHER_TOKEN)) == 0) {
				return string.Empty;
			}
			var str = Environment.GetCommandLineArgs().First(a => a.Contains(LAUNCHER_TOKEN));
			return !string.IsNullOrEmpty(str) ? str.Split('=')[1] : string.Empty;
		}

		public void InvalidateTokenArguments()
		{
			invalidated = true;
		}
	}
}
