namespace Xsolla.Demo
{
	public class OnboardingStateMachineFactory
	{
		public OnboardingStateMachine Create(DebugConfig debugConfig)
		{
			var stateFactory = new OnboardingStateFactory(debugConfig);
			return new OnboardingStateMachine(stateFactory);
		}
	}
}