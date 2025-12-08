#if DEBUG_XSOLLA_DEMO
using System;

namespace Xsolla.Demo
{
	public class FakeAuthService : IAuthService
	{
		private readonly IAuthService InnerService;
		private readonly string FakeUsername;
		private readonly string FakeEmail;
		private readonly string FakePassword;

		public FakeAuthService(IAuthService innerService, string fakeUsername, string fakeEmail, string fakePassword)
		{
			InnerService = innerService;
			FakeUsername = fakeUsername;
			FakePassword = fakePassword;
			FakeEmail = fakeEmail;
		}

		public void ClearSavedData()
		{
			InnerService.ClearSavedData();
		}

		public void AuthBuySavedData(Action onSuccess, Action onError)
		{
			InnerService.AuthBuySavedData(onSuccess, onError);
		}

		public void SignIn(string username, string password, Action onSuccess, Action<string> onError)
		{
			InnerService.SignIn(FakeUsername, FakePassword, onSuccess, onError);
		}

		public void Register(string username, string email, string password, Action onSuccess, Action<string> onError)
		{
			InnerService.Register(FakeUsername, FakeEmail, FakePassword, onSuccess, onError);
		}

		public void AuthViaSocialNetwork(string socialNetwork, Action onSuccess, Action<string> onError, Action onCancel)
		{
			InnerService.SignIn(FakeUsername, FakePassword, onSuccess, onError);
		}
	}
}
#endif