namespace Xsolla.Demo
{
	public class OnboardingStateFactory
	{
		private readonly bool IsSkipOnboardingControls;
		private readonly bool IsSkipOnboardingDocumentation;
		private readonly bool IsSkipOnboardingGameplay;

		public OnboardingStateFactory(DebugConfig debugConfig)
		{
			IsSkipOnboardingControls = debugConfig.IsSkipOnboardingControlsState;
			IsSkipOnboardingDocumentation = debugConfig.IsSkipOnboardingDocumentationState;
			IsSkipOnboardingGameplay = debugConfig.IsSkipOnboardingGameplayState;
		}

		public IOnboardingState CreateControlsState()
		{
#if DEBUG_XSOLLA_DEMO
			if (IsSkipOnboardingControls)
				return new FakeOnboardingControlsState();
#endif

			return new OnboardingControlsState();
		}

		public IOnboardingState CreateDocumentationState()
		{
#if DEBUG_XSOLLA_DEMO
			if (IsSkipOnboardingDocumentation)
				return new FakeOnboardingDocumentationState();
#endif

			return new OnboardingDocumentationState();
		}

		public IOnboardingState CreateGameplayState()
		{
#if DEBUG_XSOLLA_DEMO
			if (IsSkipOnboardingGameplay)
				return new FakeOnboardingGameplayState();
#endif

			return new OnboardingGameplayState();
		}
	}
}