namespace Xsolla.Demo
{
	public class OnboardingGameplayState : IOnboardingState
	{
		private static ScreenService ScreenService => ServiceLocator.Resolve<ScreenService>();

		public void OnEnter()
		{
			ScreenService.CloseAll();
			ScreenService.OpenOnboardingGameplay();
		}
	}
}