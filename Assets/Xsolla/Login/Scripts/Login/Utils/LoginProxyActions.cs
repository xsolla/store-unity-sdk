using System;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public static class LoginProxyActions
	{
		public static Action<LoginPageEnterController, object> RunDemoUserAuthDelegate
		{
			get
			{
				return RunDemoUserAuthMethod;
			}
		}
		public static Action<LoginPageEnterController, object> RunSocialAuthDelegate
		{
			get
			{
				return RunSocialAuthMethod;
			}
		}
		public static Action<LoginPageEnterController, object> RunSteamAuthDelegate
		{
			get
			{
				return RunSteamAuth;
			}
		}

		private static void RunDemoUserAuthMethod(LoginPageEnterController loginController, object arg)
		{
			loginController.RunBasicAuth(username: "xsolla", password: "xsolla", rememberMe: true);
		}

		private static void RunSocialAuthMethod(LoginPageEnterController loginController, object arg)
		{
			SocialProvider provider = (SocialProvider)arg;
			loginController.RunSocialAuth(provider);
		}

		private static void RunSteamAuth(LoginPageEnterController loginController, object arg)
		{
			loginController.RunManualSteamAuth();
		}
	}
}
