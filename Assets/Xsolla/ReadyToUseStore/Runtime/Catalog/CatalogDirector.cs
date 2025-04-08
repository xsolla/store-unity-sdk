using UnityEngine;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.ReadyToUseStore
{
	internal class CatalogDirector : MonoBehaviour
	{
		[SerializeField] private Transform WidgetsContainer;
		[SerializeField] private Transform CurtainContainer;
		[SerializeField] private GameObject CurtainPrefab;
		[SerializeField] private Transform StoreItemWidgetPrefab;

		private Config Config;
		private GameObject Curtain;

		private void Start()
		{
			ClearContainer();
			FetchCatalog();
		}

		public void Construct(Config config)
		{
			Config = config;
		}

		private void FetchCatalog()
		{
			CreateCurtain();

			XsollaCatalog.GetItems(
				response => {
					DeleteCurtain();
					DrawStore(response.items);
				},
				OnCatalogError,
				Config?.Locale);
		}

		private void DrawStore(StoreItem[] items)
		{
			ClearContainer();

			foreach (var item in items)
			{
				Instantiate(StoreItemWidgetPrefab, WidgetsContainer, false)
					.GetComponent<ICatalogItemWidget>()
					.Construct(item);
			}
		}

		private void CreateCurtain()
		{
			if (Curtain)
				return;

			if (!CurtainPrefab)
				return;

			Curtain = Instantiate(CurtainPrefab, WidgetsContainer);
			Curtain.transform.SetParent(CurtainContainer.transform, false);
			Curtain.transform.SetAsFirstSibling();
		}

		private void DeleteCurtain()
		{
			if (Curtain)
				Destroy(Curtain);
		}

		private void ClearContainer()
		{
			foreach (Transform obj in WidgetsContainer)
			{
				if (obj)
					Destroy(obj.gameObject);
			}
		}

		private void OnCatalogError(Error error)
		{
			XDebug.Log("Catalog error: " + error);
		}
	}
}