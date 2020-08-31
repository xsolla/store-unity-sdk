using System;
using System.Linq;
using Xsolla.Core;

public class LauncherArguments : MonoSingleton<LauncherArguments>
{
    const string LAUNCHER_TOKEN = "xsolla-login-jwt";

    private bool invalidated = false;

    public string GetToken()
	{
		if(invalidated) {
            return "";
		}
		var args = Environment.GetCommandLineArgs().ToList();
        var str = args.First(a => a.Contains(LAUNCHER_TOKEN));
        return !string.IsNullOrEmpty(str) ? str.Split('=')[1] : string.Empty;
	}

	public void InvalidateTokenArguments()
	{
        invalidated = true;
    }
}
