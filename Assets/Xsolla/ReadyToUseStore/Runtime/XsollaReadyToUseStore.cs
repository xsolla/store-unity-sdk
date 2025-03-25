using System;
using UnityEngine;
using Xsolla.Core;
using Object = UnityEngine.Object;

namespace Xsolla.ReadyToUseStore
{
	public static class XsollaReadyToUseStore
	{
		private static ReadyToUseStoreDirector ReadyToUseStoreDirector { get; set; }

		public static event Action OnAuthSuccess;
		public static event Action<Error> OnAuthFailed;
		public static event Action OnAuthCancelled;

		public static void Initialize()
		{
			var config = new ReadyToUseStoreConfig();
			Initialize(config);
		}

		public static void Destroy()
		{
			if (!ReadyToUseStoreDirector)
				return;

			ReadyToUseStoreDirector.gameObject.SetActive(false);
			Object.Destroy(ReadyToUseStoreDirector.gameObject);
		}

		internal static void RiseOnAuthSuccess()
			=> OnAuthSuccess?.Invoke();

		internal static void RiseOnAuthError(Error error)
			=> OnAuthFailed?.Invoke(error);

		internal static void RiseOnAuthCancelled()
			=> OnAuthCancelled?.Invoke();

		private static void Initialize(ReadyToUseStoreConfig config)
		{
			if (config == null)
				throw new ArgumentNullException(nameof(config));

			if (ReadyToUseStoreDirector && ReadyToUseStoreDirector.isActiveAndEnabled)
				return;

			var prefab = Resources.Load<ReadyToUseStoreDirector>(config.PrefabPath);
			ReadyToUseStoreDirector = Object.Instantiate(prefab);
			Object.DontDestroyOnLoad(ReadyToUseStoreDirector);

			ReadyToUseStoreDirector.Initialize(config);
		}
	}
}