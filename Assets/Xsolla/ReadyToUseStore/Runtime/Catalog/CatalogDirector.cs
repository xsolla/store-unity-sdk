using System.Collections.Generic;
using UnityEngine;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.ReadyToUseStore
{
	internal class CatalogDirector : MonoBehaviour
	{
		[SerializeField] private Transform WidgetsContainer;
		[SerializeField] private Transform CurtainContainer;

		private ReadyToUseStoreConfig ReadyToUseStoreConfig;
		private IPrefabsProvider PrefabsProvider;
		private GameObject Curtain;

		private void Start()
		{
			ClearWidgets();
			GetCatalog();
		}

		public void Construct(ReadyToUseStoreConfig config, IPrefabsProvider prefabsProvider)
		{
			ReadyToUseStoreConfig = config;
			PrefabsProvider = prefabsProvider;
		}

		private void GetCatalog()
		{
			CreateCurtain();

			XsollaCatalog.GetItems(
				OnGetCatalogSuccess,
				OnCatalogError,
				ReadyToUseStoreConfig?.Locale);
		}

		private void DrawStore(StoreItem[] items)
		{
			ClearWidgets();

			var widgetPrefab = PrefabsProvider.GetStoreItemWidgetPrefab();
			foreach (var item in items)
			{
				Instantiate(widgetPrefab, WidgetsContainer, false)
					.GetComponent<IStoreItemWidget>()
					.SetItem(item);
			}
		}

		private void CreateCurtain()
		{
			if (Curtain)
				return;

			var curtainPrefab = PrefabsProvider.GetCatalogCurtainPrefab();
			if (!curtainPrefab)
				return;

			Curtain = Instantiate(curtainPrefab, WidgetsContainer);
			Curtain.transform.SetParent(CurtainContainer, false);
		}

		private void DeleteCurtain()
		{
			CurtainContainer.gameObject.SetActive(false);
		}

		private void ClearWidgets()
		{
			foreach (Transform obj in WidgetsContainer)
			{
				if (obj)
					Destroy(obj.gameObject);
			}
		}

		private void OnGetCatalogSuccess(StoreItems items)
		{
			XsollaReadyToUseStore.RiseGetCatalogSuccess();
			FetchAllImages(items);
		}

		private void FetchAllImages(StoreItems items)
		{
			var urls = WarmupHelper.FetchAllImageUrls(items);
			var counter = urls.Count;

			foreach (var url in urls)
			{
				SpriteCache.Get(
					url,
					_ => {
						counter--;
						if (counter <= 0)
						{
							DeleteCurtain();
							DrawStore(items.items);
						}
					});
			}
		}

		private void OnCatalogError(Error error)
		{
			XsollaReadyToUseStore.RiseGetCatalogError(error);
		}
	}
}