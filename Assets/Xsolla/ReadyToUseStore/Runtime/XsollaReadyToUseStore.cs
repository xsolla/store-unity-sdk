using System;
using UnityEngine;
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
		public static event Action OnAuthCancel;

		public static event Action OnGetCatalogSuccess;
		public static event Action<Error> OnGetCatalogError;

		public static event Action<OrderStatus> OnPurchaseSuccess;
		public static event Action<Error> OnPurchaseError;

		public static void OpenStore(ReadyToUseStoreConfig config = null, IPrefabsProvider prefabsProvider = null, IStoreListener storeListener = null)
		{
			config ??= new ReadyToUseStoreConfig();
			prefabsProvider ??= new DefaultPrefabsProvider();
			StoreListener = storeListener;

			if (StoreDirector && StoreDirector.isActiveAndEnabled)
				return;

			StoreDirector = Object.FindAnyObjectByType<StoreDirector>(FindObjectsInactive.Include);
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
			if (!StoreDirector)
				return;

			StoreDirector.gameObject.SetActive(false);
			Object.Destroy(StoreDirector.gameObject);
			StoreDirector = null;
		}

		public static void WarmupCatalogImages()
		{
			WarmupHelper.WarmupCatalogImages();
		}

		internal static void RiseAuthSuccess()
		{
			LogMessage("Auth success");
			StoreListener?.OnAuthSuccess();
			OnAuthSuccess?.Invoke();
		}

		internal static void RiseAuthError(Error error)
		{
			LogError($"Auth error: {error}");
			StoreListener?.OnAuthError(error);
			OnAuthError?.Invoke(error);
		}

		internal static void RiseAuthCancel()
		{
			LogMessage("Auth cancel");
			StoreListener?.OnAuthCancel();
			OnAuthCancel?.Invoke();
		}

		internal static void RiseGetCatalogSuccess()
		{
			LogMessage("Get catalog success");
			StoreListener?.OnGetCatalogSuccess();
			OnGetCatalogSuccess?.Invoke();
		}

		internal static void RiseGetCatalogError(Error error)
		{
			LogError($"Get catalog error: {error}");
			StoreListener?.OnGetCatalogError(error);
			OnGetCatalogError?.Invoke(error);
		}

		internal static void RisePurchaseSuccess(OrderStatus orderStatus)
		{
			LogMessage($"Purchase success: {orderStatus}");
			StoreListener?.OnPurchaseSuccess(orderStatus);
			OnPurchaseSuccess?.Invoke(orderStatus);
		}

		internal static void RisePurchaseError(Error error)
		{
			LogError($"Purchase error: {error}");
			StoreListener?.OnPurchaseError(error);
			OnPurchaseError?.Invoke(error);
		}

		private static void LogMessage(string message)
		{
			XDebug.Log("[ReadyToUseStore] " + message);
		}

		private static void LogError(string message)
		{
			XDebug.LogError("[ReadyToUseStore] " + message);
		}
	}
}