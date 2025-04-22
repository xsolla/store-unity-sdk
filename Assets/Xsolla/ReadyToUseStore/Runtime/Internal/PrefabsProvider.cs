using UnityEngine;

namespace Xsolla.ReadyToUseStore
{
	internal class PrefabsProvider : IPrefabsProvider
	{
		public GameObject GetStoreDirectorPrefab()
			=> LoadPrefab("Xsolla/ReadyToUseStore/StoreDirector");

		public GameObject GetCatalogDirectorPrefab()
			=> LoadPrefab("Xsolla/ReadyToUseStore/CatalogDirector");

		public GameObject GetCatalogCurtainPrefab()
			=> LoadPrefab("Xsolla/ReadyToUseStore/CatalogCurtain");

		public GameObject GetCatalogItemWidgetPrefab()
			=> LoadPrefab("Xsolla/ReadyToUseStore/CatalogItemWidget");

		private static GameObject LoadPrefab(string path)
			=> Resources.Load<GameObject>(path);
	}
}