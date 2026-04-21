using System.Collections;

namespace Xsolla.Demo
{
	public class GameAwakeState : IGameState, IGameStateEnterAsync
	{
		private ICatalogService CatalogService => ServiceLocator.Resolve<ICatalogService>();
		private ScreenService ScreenService => ServiceLocator.Resolve<ScreenService>();
		private GameStateMachine GameStateMachine => ServiceLocator.Resolve<GameStateMachine>();
		private CameraService CameraService => ServiceLocator.Resolve<CameraService>();
		private TimeService TimeService => ServiceLocator.Resolve<TimeService>();

		public IEnumerator OnEnter()
		{
			ScreenService.CloseAll();
			ScreenService.OpenSplashScreen();

			TimeService.Unfreeze();
			CameraService.ToggleCamera(PawnMode.Onboarding);

			yield return CatalogService.FetchCatalogCoroutine();
			GameStateMachine.SwitchToUserAuthStart();
		}
	}
}