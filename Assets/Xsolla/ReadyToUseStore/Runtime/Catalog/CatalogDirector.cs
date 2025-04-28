using UnityEngine;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.ReadyToUseStore
{
	internal class CatalogDirector : MonoBehaviour
	{
		[SerializeField] private Transform WidgetsContainer;
		[SerializeField] private Transform CurtainContainer;

		private Config Config;
		private IPrefabsProvider PrefabsProvider;
		private GameObject Curtain;

		private void Start()
		{
			ClearWidgets();
			GetCatalog();
		}

		public void Construct(Config config, IPrefabsProvider prefabsProvider)
		{
			Config = config;
			PrefabsProvider = prefabsProvider;
		}

		private void GetCatalog()
		{
			CreateCurtain();

			XsollaCatalog.GetItems(
				OnGetCatalogSuccess,
				OnCatalogError,
				Config?.Locale);
		}

		private void DrawStore(StoreItem[] items)
		{
			ClearWidgets();

			var widgetPrefab = PrefabsProvider.GetCatalogItemWidgetPrefab();
			foreach (var item in items)
			{
				Instantiate(widgetPrefab, WidgetsContainer, false)
					.GetComponent<ICatalogItemWidget>()
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
				// Destroy(Curtain);
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
			DeleteCurtain();
			DrawStore(items.items);
		}

		private void OnCatalogError(Error error)
		{
			XsollaReadyToUseStore.RiseGetCatalogError(error);
		}
	}
}