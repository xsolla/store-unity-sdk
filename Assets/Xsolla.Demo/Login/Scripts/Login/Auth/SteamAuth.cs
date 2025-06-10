using System;
using System.Collections;
using UnityEngine;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class SteamAuth : LoginAuthorization
	{
		public override void TryAuth(object[] args, Action onSuccess, Action<Error> onError)
		{
#if UNITY_STANDALONE && XSOLLA_STEAMWORKS_PACKAGE_EXISTS
			StartCoroutine(ProcessSteamAuth(onSuccess, onError));
#else
			onError?.Invoke(null);
#endif
		}

#if XSOLLA_STEAMWORKS_PACKAGE_EXISTS && (UNITY_STANDALONE)
		private IEnumerator ProcessSteamAuth(Action onSuccess, Action<Error> onError)
		{
			Steamworks.SteamAPI.Init();

			if (!DemoSettings.UseSteamAuth)
			{
				onError?.Invoke(null);
				yield break;
			}

			var appId = DemoSettings.SteamAppId;
			if (!int.TryParse(appId, out _))
			{
				onError?.Invoke(new Error(errorMessage: "Steam auth failed. Can't parse SteamAppId"));
				yield break;
			}

			var sessionTicket = SteamUtils.GetSteamSessionTicket();
			if (string.IsNullOrEmpty(sessionTicket))
			{
				onError?.Invoke(new Error(errorMessage: "Steam auth failed. Can't get session ticket"));
				yield break;
			}

			// Delay is required for the Steam server to process the session ticket.
			yield return new WaitForSeconds(1.0f);

			XsollaAuth.SilentAuth(
				"steam",
				appId,
				sessionTicket,
				onSuccess,
				onError);
		}
#endif
	}
}