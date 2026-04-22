using System;
using System.Collections;
using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public class AuthenticatesViaSteamByInternalBrowser : AuthenticatesViaSocialNetworkScenario
	{
		public override void SetUp()
		{
			base.SetUp();
			SetInAppBrowserActive(true);
		}

		public override void TearDown()
		{
			base.TearDown();
			CloseInAppBrowser();
		}

		public override IEnumerator Execute(Action onSuccess, Action<Error> onError)
		{
			AuthViaSocialNetwork(SocialProvider.Steam, onSuccess, onError);
			yield break;
		}
	}
}