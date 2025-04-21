using UnityEngine;

namespace Xsolla.ReadyToUseStore
{
	public interface IPrefabsProvider
	{
		GameObject GetStoreDirectorPrefab();

		GameObject GetAuthenticationDirectorPrefab();

		GameObject GetCatalogDirectorPrefab();

		GameObject GetCatalogItemWidgetPrefab();
	}
}