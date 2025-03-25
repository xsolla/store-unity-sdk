using UnityEngine;

namespace Xsolla.ReadyToUseStore
{
	internal class ReadyToUseStoreDirector : MonoBehaviour
	{
		[SerializeField] private StoreHandler StoreHandlerPrefab;
		private ReadyToUseStoreConfig Config;

		private void Start()
		{
			AuthenticateUser();
		}

		public void Initialize(ReadyToUseStoreConfig config)
		{
			Config = config;
		}

		private void AuthenticateUser()
		{
			new StoreAuthenticator().Execute(
				Config,
				OpenStore,
				null,
				null);
		}

		private void OpenStore()
		{
			var handler = Instantiate(StoreHandlerPrefab);

			if (Config.Parent)
				handler.transform.SetParent(Config.Parent, Config.IsWorldSpace);

			if (Config.IsWorldSpace)
				DontDestroyOnLoad(handler.gameObject);

			handler.Initialize(Config);
		}
	}
}