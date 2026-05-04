using System;
using System.Collections;
using UnityEngine;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.TechDemo.Scenarios
{
	public class AppliesConfiguredPayStationThemeAndRedirectPolicyDuringCheckout : Scenario
	{
		private const string TestThemeId = "63295a9a2e47fab76f7708e1";
		private const string ItemSku = "booster_min_1";

		private string PreviousThemeId { get; set; }
		private Action OnSuccessCallback { get; set; }
		private Action<Error> OnErrorCallback { get; set; }
		private bool IsCompleted { get; set; }

		public override void SetUp()
		{
			base.SetUp();
			PreviousThemeId = GetPlatformSpecificSettings().paystationThemeId;
		}

		public override void TearDown()
		{
			base.TearDown();
			GetPlatformSpecificSettings().paystationThemeId = PreviousThemeId;
		}

		public override IEnumerator Execute(Action onSuccess, Action<Error> onError)
		{
			OnSuccessCallback = onSuccess;
			OnErrorCallback = onError;
			IsCompleted = false;

			GetPlatformSpecificSettings().paystationThemeId = TestThemeId;

			var authFailed = false;
			yield return AuthAsDefaultUser(error => {
				authFailed = true;
				TryCompleteError(error);
			});

			if (authFailed || IsCompleted)
				yield break;

			var purchaseCompleted = false;
			Error purchaseError = null;

			XsollaCatalog.Purchase(
				ItemSku,
				_ => purchaseCompleted = true,
				error => {
					purchaseError = error;
					purchaseCompleted = true;
				});

			yield return new WaitUntil(() => purchaseCompleted);

			if (IsCompleted)
				yield break;

			if (purchaseError != null)
			{
				TryCompleteError(purchaseError);
				yield break;
			}

			TryCompleteSuccess();
		}

		private static PayStationUISettings GetPlatformSpecificSettings()
		{
			return Application.platform switch {
				RuntimePlatform.Android      => XsollaSettings.AndroidPayStationUISettings,
				RuntimePlatform.IPhonePlayer => XsollaSettings.IosPayStationUISettings,
				RuntimePlatform.WebGLPlayer  => XsollaSettings.WebglPayStationUISettings,
				_                            => XsollaSettings.DesktopPayStationUISettings
			};
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