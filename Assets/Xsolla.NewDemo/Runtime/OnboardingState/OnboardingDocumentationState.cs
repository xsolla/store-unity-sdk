namespace Xsolla.Demo
{
	public class OnboardingDocumentationState : IOnboardingState
	{
		private static ScreenService ScreenService => ServiceLocator.Resolve<ScreenService>();

		public void OnEnter()
		{
			ScreenService.CloseAll();
			ScreenService.OpenOnboardDocumentation();
		}
	}
}