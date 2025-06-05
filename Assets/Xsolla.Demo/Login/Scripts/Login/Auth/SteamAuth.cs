using System;
using System.Collections;
using UnityEngine;
using Xsolla.Auth;
using Xsolla.Core;
#if XSOLLA_STEAMWORKS_PACKAGE_EXISTS
using Steamworks;
using SteamUtils = Xsolla.Core.SteamUtils;
#endif

namespace Xsolla.Demo
{
	public class SteamAuth : LoginAuthorization
	{
		public override void TryAuth(object[] args, Action onSuccess, Action<Error> onError)
		{
#if !(UNITY_EDITOR || UNITY_STANDALONE) || !XSOLLA_STEAMWORKS_PACKAGE_EXISTS
			onError?.Invoke(null);
#else
			StartCoroutine(ProcessSteamAuth(onSuccess, onError));
#endif
		}

#if XSOLLA_STEAMWORKS_PACKAGE_EXISTS && (UNITY_EDITOR || UNITY_STANDALONE)
		private IEnumerator ProcessSteamAuth(Action onSuccess, Action<Error> onError)
		{
			SteamAPI.Init();

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