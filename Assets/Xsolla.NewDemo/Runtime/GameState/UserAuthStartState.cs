namespace Xsolla.Demo
{
	public class UserAuthStartState : IGameState, IGameStateEnter
	{
		private ScreenService ScreenService => ServiceLocator.Resolve<ScreenService>();
		private IAuthService AuthService => ServiceLocator.Resolve<IAuthService>();
		private GameStateMachine GameStateMachine => ServiceLocator.Resolve<GameStateMachine>();

		public void OnEnter()
		{
			ScreenService.CloseAll();
			ScreenService.OpenSplashScreen();

			AuthService.AuthBuySavedData(
				() => GameStateMachine.SwitchToUserAuthFinish(),
				() => {
					ScreenService.CloseAll();
					ScreenService.OpenUserAuthScreen();
				});
		}
	}
}