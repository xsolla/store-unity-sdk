using UnityEngine;

namespace Xsolla.ReadyToUseStore
{
	internal class DefaultPrefabsProvider : IPrefabsProvider
	{
		public GameObject GetStoreDirectorPrefab()
			=> LoadPrefab("Xsolla/ReadyToUseStore/StoreDirector");

		public GameObject GetCatalogDirectorPrefab()
			=> LoadPrefab("Xsolla/ReadyToUseStore/CatalogDirector");

		public GameObject GetCatalogCurtainPrefab()
			=> LoadPrefab("Xsolla/ReadyToUseStore/CatalogCurtain");

		public GameObject GetStoreItemWidgetPrefab()
			=> LoadPrefab("Xsolla/ReadyToUseStore/StoreItemWidget");

		private static GameObject LoadPrefab(string path)
			=> Resources.Load<GameObject>(path);
	}
}