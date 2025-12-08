using UnityEngine;

namespace Xsolla.Demo
{
	public class GameBootstrapper : MonoBehaviour
	{
		[SerializeField] private DebugConfig DebugConfig;
		[SerializeField] private ScreenPrefabRegistry ScreenPrefabRegistry;
		[SerializeField] private PawnPrefabRegistry PawnPrefabRegistry;

		private void Start()
		{
			BindServices();

#if DEBUG_XSOLLA_DEMO
			SetupDebug();
#endif

			ServiceLocator.Resolve<GameStateMachine>().SwitchToGameAwake();
		}

		private void BindServices()
		{
			var serviceLocator = ServiceLocator.Instance;

			serviceLocator.Register(new UrlService());
			serviceLocator.Register(new InputService());

			serviceLocator.Register(new AuthServiceFactory().Create(DebugConfig));
			serviceLocator.Register(new InventoryServiceFactory().Create(DebugConfig));
			serviceLocator.Register(new GameStateMachineFactory().Create());
			serviceLocator.Register(new OnboardingStateMachineFactory().Create(DebugConfig));
			serviceLocator.Register(new LevelStateMachineFactory().Create());
			serviceLocator.Register(new CatalogServiceFactory().Create());
			serviceLocator.Register(new StoreServiceFactory().Create(DebugConfig));
			serviceLocator.Register(new ScreenServiceFactory().Create(ScreenPrefabRegistry));

			serviceLocator.Register(new PawnServiceFactory().Create(PawnPrefabRegistry));
			serviceLocator.Register(new TimeService());
			serviceLocator.Register(new CameraService());
			serviceLocator.Register(new GuyService());
			serviceLocator.Register(new FoxService());
			serviceLocator.Register(new OwlService());
		}

#if DEBUG_XSOLLA_DEMO
		private void SetupDebug()
		{
			if (DebugConfig.IsEraseSavedAuthData)
				ServiceLocator.Resolve<IAuthService>().ClearSavedData();

			gameObject.AddComponent<DebugController>();
		}
#endif
	}
}