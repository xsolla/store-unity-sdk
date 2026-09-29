namespace Xsolla.Demo
{
	public class LevelStateMachine
	{
		private readonly LevelStateFactory StateFactory;

		public LevelStateMachine(LevelStateFactory stateFactory)
		{
			StateFactory = stateFactory;
		}

		private ILevelState CurrentState { get; set; }

		public void SetAwakeLevel()
			=> SetState(StateFactory.CreateAwakeLevelState());

		public void SetStoreLevel()
			=> SetState(StateFactory.CreateStoreLevelState());

		public void SetResumeLevel()
			=> SetState(StateFactory.CreateResumeLevelState());

		public void SetOwlPurchased()
			=> SetState(StateFactory.CreateOwlPurchasedLevelState());

		private void SetState(ILevelState state)
		{
			if (state is ILevelStateExit stateExit)
				stateExit.OnExit();

			CurrentState = state;
			CurrentState.OnEnter();
		}
	}
}