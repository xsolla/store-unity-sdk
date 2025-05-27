using UnityEngine;

namespace Xsolla.ReadyToUseStore
{
	public interface IPrefabsProvider
	{
		GameObject GetStoreDirectorPrefab();

		GameObject GetCatalogDirectorPrefab();

		GameObject GetCatalogCurtainPrefab();

		GameObject GetStoreItemWidgetPrefab();
	}
}