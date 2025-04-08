using System;
using Xsolla.Core;
using Object = UnityEngine.Object;

namespace Xsolla.ReadyToUseStore
{
	public static class XsollaReadyToUseStore
	{
		private static StoreDirector StoreDirector { get; set; }

		public static event Action OnAuthSuccess;
		public static event Action<Error> OnAuthFailed;
		public static event Action OnAuthCancelled;

		public static void OpenStore(Config config = null, IPrefabsProvider prefabsProvider = null)
		{
			if (config == null)
				config = new Config();

			if (prefabsProvider == null)
				prefabsProvider = new ResourcesPrefabsProvider();

			if (config == null)
				throw new ArgumentNullException(nameof(config));

			if (StoreDirector && StoreDirector.isActiveAndEnabled)
				return;

			StoreDirector = Object.FindAnyObjectByType<StoreDirector>();

			if (!StoreDirector)
			{
				var prefab = prefabsProvider.GetStoreDirectorPrefab();
				StoreDirector = Object.Instantiate(prefab).GetComponent<StoreDirector>();
				Object.DontDestroyOnLoad(StoreDirector);
			}

			StoreDirector.Initialize(config);
		}

		public static void CloseStore()
		{
			if (!StoreDirector)
				return;

			StoreDirector.gameObject.SetActive(false);
			Object.Destroy(StoreDirector.gameObject);
		}

		internal static void RiseOnAuthSuccess()
			=> OnAuthSuccess?.Invoke();

		internal static void RiseOnAuthError(Error error)
			=> OnAuthFailed?.Invoke(error);

		internal static void RiseOnAuthCancelled()
			=> OnAuthCancelled?.Invoke();
	}
}