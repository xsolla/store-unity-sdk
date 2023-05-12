using System;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class SteamAuth : LoginAuthorization
	{
		public override void TryAuth(object[] args, Action onSuccess, Action<Error> onError)
		{
#if !(UNITY_EDITOR || UNITY_STANDALONE)
			onError?.Invoke(null);
#else
			if (!DemoSettings.UseSteamAuth)
			{
				onError?.Invoke(null);
				return;
			}

			var appId = DemoSettings.SteamAppId;
			if (!int.TryParse(appId, out _))
			{
				onError?.Invoke(new Error(errorMessage: "Steam auth failed. Can't parse SteamAppId"));
				return;
			}

			var sessionTicket = SteamUtils.GetSteamSessionTicket();
			if (string.IsNullOrEmpty(sessionTicket))
			{
				onError?.Invoke(new Error(errorMessage: "Steam auth failed. Can't get session ticket"));
				return;
			}

			XsollaAuth.SilentAuth(
				"steam",
				appId,
				sessionTicket,
				onSuccess,
				onError);
#endif
		}
	}
}