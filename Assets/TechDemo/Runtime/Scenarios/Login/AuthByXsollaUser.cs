using System;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public class AuthByXsollaUser : Scenario
	{
		public override void Execute(Action onSuccess, Action<Error> onError)
		{
			ClearAuthData();

			var username = "xsolla";
			var password = "xsolla";
			XsollaAuth.SignIn(username, password, onSuccess, onError);
		}
	}
}