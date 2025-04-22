using System;
using Xsolla.Core;
using Object = UnityEngine.Object;

namespace Xsolla.ReadyToUseStore
{
	public static class XsollaReadyToUseStore
	{
		private static StoreDirector StoreDirector { get; set; }
		private static IStoreListener StoreListener { get; set; }

		public static event Action OnAuthSuccess;
		public static event Action<Error> OnAuthError;
		public static event Action OnAuthCancelled;

		public static event Action OnGetCatalogSuccess;
		public static event Action<Error> OnGetCatalogError;

		public static event Action<OrderStatus> OnPurchaseSuccess;
		public static event Action<Error> OnPurchaseError;

		public static void OpenStore(Config config = null, IPrefabsProvider prefabsProvider = null, IStoreListener storeListener = null)
		{
			config ??= new Config();
			prefabsProvider ??= new PrefabsProvider();
			StoreListener = storeListener;

			if (StoreDirector && StoreDirector.isActiveAndEnabled)
				return;

			StoreDirector = Object.FindAnyObjectByType<StoreDirector>();
			if (!StoreDirector)
			{
				var prefab = prefabsProvider.GetStoreDirectorPrefab();
				StoreDirector = Object.Instantiate(prefab).GetComponent<StoreDirector>();
				Object.DontDestroyOnLoad(StoreDirector);
			}

			StoreDirector.Initialize(config, prefabsProvider);
		}

		public static void CloseStore()
		{
			if (StoreDirector)
			{
				StoreDirector.gameObject.SetActive(false);
				Object.Destroy(StoreDirector.gameObject);
				StoreDirector = null;
			}

			StoreListener = null;
		}

		internal static void RiseAuthSuccess()
		{
			XDebug.Log("Auth success");
			StoreListener?.OnAuthSuccess();
			OnAuthSuccess?.Invoke();
		}

		internal static void RiseAuthError(Error error)
		{
			XDebug.LogError($"Auth error: {error}");
			StoreListener?.OnAuthError(error);
			OnAuthError?.Invoke(error);
		}

		internal static void RiseAuthCancelled()
		{
			XDebug.Log("Auth cancelled");
			StoreListener?.OnAuthCancelled();
			OnAuthCancelled?.Invoke();
		}

		internal static void RiseGetCatalogSuccess()
		{
			XDebug.Log("Get catalog success");
			StoreListener?.OnGetCatalogSuccess();
			OnGetCatalogSuccess?.Invoke();
		}

		internal static void RiseGetCatalogError(Error error)
		{
			XDebug.LogError($"Get catalog error: {error}");
			StoreListener?.OnGetCatalogError(error);
			OnGetCatalogError?.Invoke(error);
		}

		internal static void RisePurchaseSuccess(OrderStatus orderStatus)
		{
			XDebug.Log($"Purchase success: {orderStatus}");
			StoreListener?.OnPurchaseSuccess(orderStatus);
			OnPurchaseSuccess?.Invoke(orderStatus);
		}

		internal static void RisePurchaseError(Error error)
		{
			XDebug.LogError($"Purchase error: {error}");
			StoreListener?.OnPurchaseError(error);
			OnPurchaseError?.Invoke(error);
		}
	}
}