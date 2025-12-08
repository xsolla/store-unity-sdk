namespace Xsolla.Demo
{
	public class GameFinishState : IGameState, IGameStateEnter
	{
		private ScreenService ScreenService => ServiceLocator.Resolve<ScreenService>();
		private GuyService GuyService => ServiceLocator.Resolve<GuyService>();
		private FoxService FoxService => ServiceLocator.Resolve<FoxService>();
		private OwlService OwlService => ServiceLocator.Resolve<OwlService>();

		public void OnEnter()
		{
			ScreenService.CloseAll();
			ScreenService.OpenGameFinishScreen();

			GuyService.DeleteInstance();
			FoxService.DeleteInstance();
			OwlService.DeleteInstance();
		}
	}
}