#if DEBUG_XSOLLA_DEMO
namespace Xsolla.Demo
{
	public class FakeOnboardingDocumentationState : IOnboardingState
	{
		private static OnboardingStateMachine OnboardingStateMachine => ServiceLocator.Resolve<OnboardingStateMachine>();

		public void OnEnter()
		{
			OnboardingStateMachine.SwitchToGameplayState();
		}
	}
}
#endif