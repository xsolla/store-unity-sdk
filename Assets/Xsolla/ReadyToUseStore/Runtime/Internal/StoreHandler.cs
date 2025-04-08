using System;
using UnityEngine;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.ReadyToUseStore
{
	internal class StoreHandler : MonoBehaviour
	{
		[SerializeField] private Transform WidgetsContainer;
		[SerializeField] private Transform StoreItemWidgetPrefab;

		private ReadyToUseStoreConfig Config;

		private void Start()
		{
			ClearContainer();
			DrawMain();
		}

		public void Initialize(ReadyToUseStoreConfig config)
		{
			Config = config;
		}

		private void DrawMain()
		{
			XsollaCatalog.GetItems(
				response => DrawStore(response.items),
				error => Debug.LogError("Error: " + error),
				Config?.Locale);
		}

		private void DrawStore(StoreItem[] items)
		{
			foreach (var item in items)
			{
				Instantiate(StoreItemWidgetPrefab, WidgetsContainer, false)
					.GetComponent<StoreItemWidget>().Initialize(item);
			}
		}

		private void ClearContainer()
		{
			foreach (Transform obj in WidgetsContainer)
				Destroy(obj.gameObject);
		}
	}
}