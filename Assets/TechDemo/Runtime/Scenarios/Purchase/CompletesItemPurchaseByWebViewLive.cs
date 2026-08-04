using System;
using System.Collections;
using Xsolla.Auth;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public class CompletesItemPurchaseByWebViewLive : Scenario
	{
		private const string ItemSku = "gloves_1";
		private const string ProjectId = "185446";

		public override void SetUp()
		{
			base.SetUp();
			SetSandboxMode(false);
			SetInAppBrowserActive(true);
			SetProjectId(ProjectId);
		}

		public override IEnumerator Execute(Action onSuccess, Action<Error> onError)
		{
			InitExecution(onSuccess, onError);

			XsollaAuth.AuthWithXsollaWidget(
				() => {
					var appearance = new PlatformSpecificAppearance {
						AndroidActivityType = AndroidActivityType.WebView
					};

					XsollaCatalog.Purchase(
						ItemSku,
						_ => TryCompleteSuccess(),
						error => TryCompleteError(error),
						platformSpecificAppearance: appearance);
				},
				error => TryCompleteError(error),
				() => TryCompleteError(new Error(errorMessage: "Canceled by user")));

			yield return WaitForCompletion();
		}
	}
}
