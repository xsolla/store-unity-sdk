using System;
using System.Collections;
using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public class AuthenticatesWithXsollaWidgetByExternalBrowser : AuthenticatesViaSocialNetworkScenario
	{
		public override void SetUp()
		{
			base.SetUp();
			XsollaSettings.InAppBrowserEnabled = false;
		}

		public override IEnumerator Execute(Action onSuccess, Action<Error> onError)
		{
			AuthWithXsollaWidget(onSuccess, onError);
			yield break;
		}
	}
}