using System;
using System.Collections;
using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public class AuthenticatesViaGoogleByInternalBrowser : AuthenticatesViaSocialNetworkScenario
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
			AuthViaSocialNetwork(SocialProvider.Google, onSuccess, onError);
			yield break;
		}
	}
}