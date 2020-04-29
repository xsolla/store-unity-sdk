using NUnit.Framework;
using Xsolla.Core;
using Xsolla.Login;
using Xsolla.Store;

namespace Tests
{
    public class Test_2_Get_Login_JWT : BaseTestApiScript
    {
	    private const string USERNAME = "test123";
	    private const string PASSWORD = "121212";

		protected override void Request()
		{
			XsollaLogin.Instance.SignIn(USERNAME, PASSWORD, false, SuccessCase, FailedRequest);
		}

		private void SuccessCase()
		{
			XsollaStore.Instance.Token = XsollaLogin.Instance.Token;
			Complete();
		}
	}
}
