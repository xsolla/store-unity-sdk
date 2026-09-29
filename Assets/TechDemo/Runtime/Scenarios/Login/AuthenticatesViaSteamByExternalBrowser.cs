using System;
using System.Collections;
using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public class AuthenticatesViaSteamByExternalBrowser : AuthenticatesViaSocialNetworkScenario
	{
		public override void SetUp()
		{
			base.SetUp();
			SetInAppBrowserActive(false);
		}

		public override IEnumerator Execute(Action onSuccess, Action<Error> onError)
		{
			AuthViaSocialNetwork(SocialProvider.Steam, onSuccess, onError);
			yield break;
		}
	}
}