using System;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public abstract class Scenario
	{
		public abstract void Execute(Action onSuccess, Action<Error> onError);

		protected void ClearAuthData()
		{
			XsollaToken.DeleteSavedInstance();
		}

		protected void AuthAsDemoUser(Action onSuccess, Action<Error> onError)
		{
			var username = "xsolla";
			var password = "xsolla";
			XsollaAuth.SignIn(username, password, onSuccess, onError);
		}
	}
}