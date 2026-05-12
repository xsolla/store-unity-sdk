using System;
using System.Collections;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public class CompletesItemPurchaseByInternalBrowserSandbox : Scenario
	{
		private const string ItemSku = "booster_max_1";

		public override void SetUp()
		{
			base.SetUp();
			SetSandboxMode(true);
			SetInAppBrowserActive(true);
		}

		public override IEnumerator Execute(Action onSuccess, Action<Error> onError)
		{
			InitExecution(onSuccess, onError);

			var authFailed = false;
			yield return AuthAsDefaultUser(error => {
				authFailed = true;
				TryCompleteError(error);
			});

			if (authFailed || IsCompleted)
				yield break;

			XsollaCatalog.Purchase(
				ItemSku,
				_ => {
					CloseInAppBrowser();
					TryCompleteSuccess();
				},
				error => {
					CloseInAppBrowser();
					TryCompleteError(error);
				});

			yield return WaitForCompletion();
		}
	}
}