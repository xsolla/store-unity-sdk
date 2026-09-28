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
			InitExecution(onSuccess, onError);

			GetPlatformSpecificSettings().paystationThemeId = TestThemeId;

			yield return AuthAsDefaultUser();
			if (IsCompleted)
				yield break;

			yield return RunAsync<OrderStatus>((success, failure) => XsollaCatalog.Purchase(ItemSku, success, failure));
			if (IsCompleted)
				yield break;

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
	}
}