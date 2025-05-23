using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.ReadyToUseStore
{
	internal class StoreDirector : MonoBehaviour
	{
		private readonly List<GameObject> SpawnedObjects = new();

		private ReadyToUseStoreConfig ReadyToUseStoreConfig;
		private IPrefabsProvider PrefabsProvider;

		private static StoreDirector Instance;

		private void Start()
		{
			if (Instance)
			{
				gameObject.SetActive(false);
				return;
			}

			Instance = this;

			PrefabsProvider ??= new DefaultPrefabsProvider();
			StartAuthentication();
			WarmupHelper.WarmupCatalogImages();
		}

		private void OnDestroy()
		{
			for (var i = 0; i < SpawnedObjects.Count; i++)
			{
				var obj = SpawnedObjects[i];
				if (obj)
				{
					obj.SetActive(false);
					Destroy(obj);
				}
			}

			SpawnedObjects.Clear();
		}

		public void Initialize(ReadyToUseStoreConfig config, IPrefabsProvider prefabsProvider)
		{
			ReadyToUseStoreConfig = config;
			PrefabsProvider = prefabsProvider;
		}

		private void StartAuthentication()
		{
			var director = new AuthenticationDirector(ReadyToUseStoreConfig);
			director.StartAuthentication(
				OpenStore,
				null,
				null);
		}

		private void OpenStore()
		{
			var prefab = PrefabsProvider.GetCatalogDirectorPrefab();
			var director = Instantiate(prefab).GetComponent<CatalogDirector>();

			if (ReadyToUseStoreConfig != null && ReadyToUseStoreConfig.CatalogParent)
				director.transform.SetParent(ReadyToUseStoreConfig.CatalogParent, false);

			if (ReadyToUseStoreConfig != null && ReadyToUseStoreConfig.IsDontDestroyOnLoad)
				DontDestroyOnLoad(director.gameObject);

			SpawnedObjects.Add(director.gameObject);
			director.Construct(ReadyToUseStoreConfig, PrefabsProvider);
		}
	}
}