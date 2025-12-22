namespace Xsolla.Demo
{
	public class AwakeLevelState : ILevelState
	{
		private static ScreenService ScreenService => ServiceLocator.Resolve<ScreenService>();
		private static GuyService GuyService => ServiceLocator.Resolve<GuyService>();
		private static FoxService FoxService => ServiceLocator.Resolve<FoxService>();
		private static CameraService CameraService => ServiceLocator.Resolve<CameraService>();
		private static OwlService OwlService => ServiceLocator.Resolve<OwlService>();

		public void OnEnter()
		{
			ScreenService.CloseAll();
			ScreenService.OpenLevelPlayScreen();

			var pawnMode = PawnMode.Gameplay;
			GuyService.SpawnGuy(pawnMode);
			OwlService.SpawnOwl(pawnMode);
			FoxService.SpawnFox(pawnMode);
			CameraService.ToggleCamera(pawnMode);
		}

		public void OnExit()
		{
		}
	}
}