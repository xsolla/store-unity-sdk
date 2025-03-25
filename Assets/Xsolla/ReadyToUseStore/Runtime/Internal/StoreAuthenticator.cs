using System;
using UnityEngine;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.ReadyToUseStore
{
	internal class StoreAuthenticator
	{
		public void Execute(ReadyToUseStoreConfig config, Action onSuccess, Action<Error> onError, Action onCancel)
		{
			if (config?.TokenData != null)
			{
				XsollaToken.Create(config.TokenData.accessToken, config.TokenData.refreshToken, config.TokenData.expirationTime);
				OnAuthSuccess(onSuccess);
				return;
			}

			if (XsollaToken.TryLoadInstance())
			{
				OnAuthSuccess(onSuccess);
				return;
			}

			XsollaAuth.AuthWithXsollaWidget(
				() => OnAuthSuccess(onSuccess),
				error => OnAuthError(error, onError),
				() => OnAuthCancel(onCancel),
				config?.Locale);
		}

		private void OnAuthSuccess(Action callback)
		{
			Debug.Log("Auth success");
			XsollaReadyToUseStore.RiseOnAuthSuccess();
			callback?.Invoke();
		}

		private void OnAuthError(Error error, Action<Error> callback)
		{
			Debug.Log("Auth error: " + error);
			XsollaReadyToUseStore.RiseOnAuthError(error);
			callback?.Invoke(error);
		}

		private void OnAuthCancel(Action callback)
		{
			Debug.Log("Auth cancel");
			XsollaReadyToUseStore.RiseOnAuthCancelled();
			callback?.Invoke();
		}
	}
}