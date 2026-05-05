using System;
using System.Collections;
using Xsolla.Auth;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public class CompletesItemPurchaseByExternalBrowserLive : Scenario
	{
		private const string ItemSku = "gloves_1";
		private const string ProjectId = "185446";

		private Action OnSuccessCallback { get; set; }
		private Action<Error> OnErrorCallback { get; set; }
		private bool IsCompleted { get; set; }

		public override void SetUp()
		{
			base.SetUp();
			SetSandboxMode(false);
			SetInAppBrowserActive(false);
			SetProjectId(ProjectId);
		}

		public override IEnumerator Execute(Action onSuccess, Action<Error> onError)
		{
			OnSuccessCallback = onSuccess;
			OnErrorCallback = onError;
			IsCompleted = false;

			XsollaAuth.AuthWithXsollaWidget(
				() => {
					XsollaCatalog.Purchase(
						ItemSku,
						x => { TryCompleteSuccess(); },
						error => { TryCompleteError(error); });
				},
				error => TryCompleteError(error),
				() => TryCompleteError(new Error(errorMessage: "Canceled by user")));

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