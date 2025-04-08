using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Xsolla.Core;
using Error = Xsolla.Core.Error;
using Object = UnityEngine.Object;

namespace Xsolla.ReadyToUseStore
{
	internal class AuthenticationDirector
	{
		private readonly Queue<IAuthenticator> Authenticators;
		private readonly bool IsCheckEventSystemExists;

		public AuthenticationDirector(Config config)
		{
			IsCheckEventSystemExists = config != null
				? config.IsCheckEventSystemExists
				: true;

			Authenticators = new Queue<IAuthenticator>();
			Authenticators.Enqueue(new BySavedDataAuthenticator());
			Authenticators.Enqueue(new ByTokenDataAuthenticator(config?.TokenData));
			Authenticators.Enqueue(new ByWidgetAuthenticator(config?.Locale));
		}

		public void StartAuthentication(Action onSuccess, Action<Error> onError, Action onCancel)
		{
			CheckEventSystemExists();
			ExecuteNextAuthenticator(onSuccess, onError, onCancel);
		}

		private void CheckEventSystemExists()
		{
			if (!IsCheckEventSystemExists)
				return;

			var eventSystem = Object.FindAnyObjectByType<EventSystem>();
			if (eventSystem)
				return;

			Debug.Log("EventSystem not found. Creating new one.");

			var gameObject = new GameObject("EventSystem");
			gameObject.AddComponent<EventSystem>();
			gameObject.AddComponent<StandaloneInputModule>();
		}

		private void ExecuteNextAuthenticator(Action onSuccess, Action<Error> onError, Action onCancel)
		{
			if (Authenticators.Count == 0)
			{
				onError?.Invoke(new Error(ErrorType.Undefined, "All authenticators failed"));
				return;
			}

			var authenticator = Authenticators.Dequeue();
			authenticator.Execute(
				() => OnAuthSuccess(onSuccess),
				error => OnAuthError(error, onError),
				() => OnAuthCancel(onCancel),
				() => ExecuteNextAuthenticator(onSuccess, onError, onCancel));
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