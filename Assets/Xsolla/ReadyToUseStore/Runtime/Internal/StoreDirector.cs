using System.Collections.Generic;
using UnityEngine;
using Xsolla.Catalog;

namespace Xsolla.ReadyToUseStore
{
	internal class StoreDirector : MonoBehaviour
	{
		private readonly List<GameObject> SpawnedObjects = new();

		private Config Config;
		private IPrefabsProvider PrefabsProvider;

		private void Start()
		{
			PrefabsProvider ??= new PrefabsProvider();
			StartAuthentication();
			WarmupCatalogImages();
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

		public void Initialize(Config config, IPrefabsProvider prefabsProvider)
		{
			Config = config;
			PrefabsProvider = prefabsProvider;
		}

		private void StartAuthentication()
		{
			var director = new AuthenticationDirector(Config);
			director.StartAuthentication(
				OpenStore,
				null,
				null);
		}

		private void WarmupCatalogImages()
		{
			XsollaCatalog.GetItems(
				items => {
					foreach (var item in items.items)
					{
						SpriteCache.Get(item.image_url, null);

						foreach (var price in item.virtual_prices)
						{
							SpriteCache.Get(price.image_url, null);
						}
					}
				},
				null,
				Config?.Locale);
		}

		private void OpenStore()
		{
			var prefab = PrefabsProvider.GetCatalogDirectorPrefab();
			var director = Instantiate(prefab).GetComponent<CatalogDirector>();

			if (Config != null && Config.CatalogParent)
				director.transform.SetParent(Config.CatalogParent, false);

			if (Config != null && Config.IsDontDestroyOnLoad)
				DontDestroyOnLoad(director.gameObject);

			SpawnedObjects.Add(director.gameObject);
			director.Construct(Config, PrefabsProvider);
		}
	}
}