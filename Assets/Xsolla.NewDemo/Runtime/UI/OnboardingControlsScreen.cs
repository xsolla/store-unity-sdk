namespace Xsolla.Demo
{
	public class OnboardingControlsScreen : Screen
	{
		private InputService InputService => ServiceLocator.Resolve<InputService>();
		private OnboardingStateMachine OnboardingStateMachine => ServiceLocator.Resolve<OnboardingStateMachine>();

		private void Update()
		{
			if (InputService.IsSkipOnboardingRequested())
				OnboardingStateMachine.SwitchToDocumentationState();
		}
	}
}