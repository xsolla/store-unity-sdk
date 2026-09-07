namespace Xsolla.Demo
{
	public class AuthServiceFactory
	{
		public IAuthService Create(DebugConfig debugConfig)
		{
			IAuthService authService = new XsollaAuthService();

#if DEBUG_XSOLLA_DEMO
			if (debugConfig.UseFakeUserCredentials)
			{
				authService = new FakeAuthService(
					authService,
					debugConfig.FakeUserName,
					debugConfig.FakeUserEmail,
					debugConfig.FakeUserPassword);
			}
#endif

			return authService;
		}
	}
}