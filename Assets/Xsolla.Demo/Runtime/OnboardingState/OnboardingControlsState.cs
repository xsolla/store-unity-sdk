namespace Xsolla.Demo
{
	public class OnboardingControlsState : IOnboardingState
	{
		private static ScreenService ScreenService => ServiceLocator.Resolve<ScreenService>();

		public void OnEnter()
		{
			ScreenService.CloseAll();
			ScreenService.OpenOnboardingControls();
		}
	}
}