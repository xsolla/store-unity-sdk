using NUnit.Framework;
using Xsolla.Core;
using Xsolla.Login;
using Xsolla.Store;

namespace Tests
{
    public class Test_2_Get_Login_JWT : BaseTestApiScript
    {
		const string USERNAME = "test123";
		const string PASSWORD = "121212";

		protected override void Request()
		{
			XsollaLogin.Instance.SignIn(USERNAME, PASSWORD, false, SuccessCase, FailedRequest);
		}

		void SuccessCase(User user)
		{
			XsollaStore.Instance.Token = XsollaLogin.Instance.Token;
			Complete();
		}
	}
}
