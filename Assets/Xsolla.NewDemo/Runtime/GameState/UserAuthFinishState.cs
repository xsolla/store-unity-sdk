using System.Collections;

namespace Xsolla.Demo
{
	public class UserAuthFinishState : IGameState, IGameStateEnterAsync
	{
		private ScreenService ScreenService => ServiceLocator.Resolve<ScreenService>();
		private IInventoryService InventoryService => ServiceLocator.Resolve<IInventoryService>();
		private GameStateMachine GameStateMachine => ServiceLocator.Resolve<GameStateMachine>();

		public IEnumerator OnEnter()
		{
			ScreenService.CloseAll();
			ScreenService.OpenSplashScreen();

			yield return InventoryService.FetchInventoryCoroutine();
			GameStateMachine.SwitchToUserOnboarding();
		}
	}
}