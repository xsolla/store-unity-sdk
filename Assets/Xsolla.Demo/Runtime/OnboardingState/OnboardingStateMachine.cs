namespace Xsolla.Demo
{
	public class OnboardingStateMachine
	{
		private readonly OnboardingStateFactory StateFactory;

		public OnboardingStateMachine(OnboardingStateFactory stateFactory)
		{
			StateFactory = stateFactory;
		}

		private IOnboardingState CurrentState { get; set; }

		public void SwitchToControlsState()
			=> ToggleState(StateFactory.CreateControlsState());

		public void SwitchToDocumentationState()
			=> ToggleState(StateFactory.CreateDocumentationState());

		public void SwitchToGameplayState()
			=> ToggleState(StateFactory.CreateGameplayState());

		private void ToggleState(IOnboardingState state)
		{
			CurrentState = state;
			CurrentState?.OnEnter();
		}
	}
}