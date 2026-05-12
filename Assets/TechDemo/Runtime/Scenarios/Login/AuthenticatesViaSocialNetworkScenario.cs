using System;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public abstract class AuthenticatesViaSocialNetworkScenario : Scenario
	{
		protected static void AuthViaSocialNetwork(SocialProvider provider, Action onSuccess, Action<Error> onError)
		{
			XsollaAuth.AuthViaSocialNetwork(
				provider,
				() => onSuccess?.Invoke(),
				error => onError?.Invoke(error),
				() => onError?.Invoke(new Error(errorMessage: "Canceled by user")));
		}

		protected static void AuthWithXsollaWidget(Action onSuccess, Action<Error> onError)
		{
			XsollaAuth.AuthWithXsollaWidget(
				() => onSuccess?.Invoke(),
				error => onError?.Invoke(error),
				() => onError?.Invoke(new Error(errorMessage: "Canceled by user")));
		}
	}
}