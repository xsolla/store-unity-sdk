using System;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.ReadyToUseStore
{
	internal class StoreAuthenticator
	{
		public void Execute(ReadyToUseStoreConfig config, Action onSuccess, Action<Error> onError, Action onCancel)
		{
			Debug.Log("Authenticating...");
			var authenticators = new Queue<IAuthenticator>();
			authenticators.Enqueue(new BySavedDataAuthenticator());
			authenticators.Enqueue(new ByTokenDataAuthenticator(config?.TokenData));
			authenticators.Enqueue(new ByWidgetAuthenticator());
			Execute(authenticators, onSuccess, onError, onCancel);
		}

		private void Execute(Queue<IAuthenticator> authenticators, Action onSuccess, Action<Error> onError, Action onCancel)
		{
			if (authenticators.Count == 0)
			{
				onCancel?.Invoke();
				return;
			}

			var authenticator = authenticators.Dequeue();
			Debug.Log($"Count: {authenticators.Count + 1}. Type: " + authenticator.GetType().Name);
			authenticator.Execute(
				() => OnAuthSuccess(onSuccess),
				error => OnAuthError(error, onError),
				() => OnAuthCancel(onCancel),
				() => Execute(authenticators, onSuccess, onError, onCancel));
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