using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.ReadyToUseStore
{
	internal class StoreDirector : MonoBehaviour
	{
		private readonly List<GameObject> SpawnedObjects = new();

		private Config Config;
		private IPrefabsProvider PrefabsProvider;

		private void Start()
		{
			StartAuthentication();
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

		public void Initialize(Config config)
		{
			Config = config;
		}

		private void StartAuthentication()
		{
			var director = new AuthenticationDirector(Config);
			director.StartAuthentication(
				OpenStore,
				null,
				null);
		}

		private void OpenStore()
		{
			var prefab = PrefabsProvider.GetCatalogDirectorPrefab();
			var director = Instantiate(prefab).GetComponent<CatalogDirector>();

			if (Config != null && Config.Parent)
				director.transform.SetParent(Config.Parent, Config.IsWorldSpace);

			if (Config != null && Config.IsWorldSpace)
				DontDestroyOnLoad(director.gameObject);

			SpawnedObjects.Add(director.gameObject);
			director.Construct(Config);
		}
	}
}