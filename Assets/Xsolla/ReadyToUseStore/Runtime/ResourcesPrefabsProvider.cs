using System;
using UnityEngine;

namespace Xsolla.ReadyToUseStore
{
	public class ResourcesPrefabsProvider : IPrefabsProvider
	{
		public ResourcesPrefabsProvider()
		{
			StoreDirectorPath = "Xsolla/ReadyToUseStore/ReadyToUseStoreDirector";
		}

		public string StoreDirectorPath { get; set; }

		public string AuthenticationDirectorPath { get; set; }

		public string CatalogDirectorPath { get; set; }

		public string CatalogItemWidgetPath { get; set; }

		public GameObject GetStoreDirectorPrefab()
		{
			return Resources.Load<GameObject>(StoreDirectorPath);
		}

		public GameObject GetAuthenticationDirectorPrefab()
		{
			throw new NotImplementedException();
		}

		public GameObject GetCatalogDirectorPrefab()
		{
			throw new NotImplementedException();
		}

		public GameObject GetCatalogItemWidgetPrefab()
		{
			throw new NotImplementedException();
		}
	}
}