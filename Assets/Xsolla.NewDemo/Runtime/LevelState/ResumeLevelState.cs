namespace Xsolla.Demo
{
	public class ResumeLevelState : ILevelState
	{
		private ScreenService ScreenService => ServiceLocator.Resolve<ScreenService>();
		private TimeService TimeService => ServiceLocator.Resolve<TimeService>();

		public void OnEnter()
		{
			var storeScreen = ScreenService.OpenStoreScreen();
			ScreenService.Close(storeScreen);

			ScreenService.OpenLevelPlayScreen();
			TimeService.Unfreeze();
		}
	}
}