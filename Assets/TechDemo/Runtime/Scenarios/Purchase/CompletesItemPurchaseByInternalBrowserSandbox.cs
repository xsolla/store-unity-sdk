using System;
using System.Collections;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public class CompletesItemPurchaseByInternalBrowserSandbox : Scenario
	{
		private const string ItemSku = "booster_max_1";

		private Action OnSuccessCallback { get; set; }
		private Action<Error> OnErrorCallback { get; set; }
		private bool IsCompleted { get; set; }

		public override void SetUp()
		{
			base.SetUp();
			SetSandboxMode(true);
			SetInAppBrowserActive(true);
		}

		public override IEnumerator Execute(Action onSuccess, Action<Error> onError)
		{
			OnSuccessCallback = onSuccess;
			OnErrorCallback = onError;
			IsCompleted = false;

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

			while (!IsCompleted)
				yield return null;
		}

		private void TryCompleteSuccess()
		{
			if (IsCompleted)
				return;

			IsCompleted = true;
			OnSuccessCallback?.Invoke();
		}

		private void TryCompleteError(Error error)
		{
			if (IsCompleted)
				return;

			IsCompleted = true;
			OnErrorCallback?.Invoke(error);
		}
	}
}