namespace Xsolla.Demo
{
	public class GamePlayState : IGameState, IGameStateEnter
	{
		private ScreenService ScreenService => ServiceLocator.Resolve<ScreenService>();
		private LevelStateMachine LevelStateMachine => ServiceLocator.Resolve<LevelStateMachine>();

		public void OnEnter()
		{
			ScreenService.CloseAll();
			ScreenService.OpenLevelPlayScreen();

			LevelStateMachine.SetAwakeLevel();
		}
	}
}