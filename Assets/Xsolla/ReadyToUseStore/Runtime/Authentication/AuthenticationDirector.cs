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

		public AuthenticationDirector(ReadyToUseStoreConfig config)
		{
			IsCheckEventSystemExists = config != null
				? config.IsCheckEventSystemExists
				: true;

			Authenticators = new Queue<IAuthenticator>();
			Authenticators.Enqueue(new ByTokenDataAuthenticator(config?.TokenData));
			Authenticators.Enqueue(new BySavedDataAuthenticator());
			Authenticators.Enqueue(new ByWidgetAuthenticator(config?.Locale));
		}

		public void StartAuthentication(Action onSuccess, Action<Error> onError, Action onCancel)
		{
			if (IsCheckEventSystemExists)
				CheckEventSystemExists();

			ExecuteNextAuthenticator(onSuccess, onError, onCancel);
		}

		private static void CheckEventSystemExists()
		{
			var eventSystem = Object.FindAnyObjectByType<EventSystem>();
			if (eventSystem)
				return;

			XDebug.Log("EventSystem not found. Creating new one.");

			var gameObject = new GameObject("EventSystem");
			Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<EventSystem>();
			
#if ENABLE_INPUT_SYSTEM && XSOLLA_UNITY_INPUT_PACKAGE_EXISTS
			gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
			gameObject.AddComponent<StandaloneInputModule>();
#endif
		}

		private void ExecuteNextAuthenticator(Action onSuccess, Action<Error> onError, Action onCancel)
		{
			if (Authenticators.Count == 0)
			{
				var error = new Error(ErrorType.Undefined, "All authenticators failed");
				OnAuthError(error, onError);
				return;
			}

			var authenticator = Authenticators.Dequeue();
			authenticator.Execute(
				() => OnAuthSuccess(onSuccess),
				error => OnAuthError(error, onError),
				() => OnAuthCancel(onCancel),
				() => ExecuteNextAuthenticator(onSuccess, onError, onCancel));
		}

		private static void OnAuthSuccess(Action callback)
		{
			XsollaReadyToUseStore.RiseAuthSuccess();
			callback?.Invoke();
		}

		private static void OnAuthError(Error error, Action<Error> callback)
		{
			XsollaReadyToUseStore.RiseAuthError(error);
			callback?.Invoke(error);
		}

		private static void OnAuthCancel(Action callback)
		{
			XsollaReadyToUseStore.RiseAuthCancel();
			callback?.Invoke();
		}
	}
}