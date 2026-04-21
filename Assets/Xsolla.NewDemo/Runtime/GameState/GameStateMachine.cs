namespace Xsolla.Demo
{
	public class GameStateMachine
	{
		private readonly GameStateFactory StateFactory;

		public GameStateMachine(GameStateFactory stateFactory)
		{
			StateFactory = stateFactory;
		}

		private IGameState CurrentState { get; set; }

		public void SwitchToGameAwake()
			=> ToggleState(StateFactory.CreateGameAwake());

		public void SwitchToUserAuthStart()
			=> ToggleState(StateFactory.CreateUserAuthStart());

		public void SwitchToUserAuthFinish()
			=> ToggleState(StateFactory.CreateUserAuthFinish());

		public void SwitchToUserOnboarding()
			=> ToggleState(StateFactory.CreateUserOnboarding());

		public void SwitchToGamePlay()
			=> ToggleState(StateFactory.CreateGamePlay());

		public void SwitchToGameFinish()
			=> ToggleState(StateFactory.CreateGameFinish());

		public void SwitchToGameOver()
			=> ToggleState(StateFactory.CreateGameOver());

		private void ToggleState(IGameState state)
		{
			CurrentState = state;

			if (state is IGameStateEnter enterState)
				enterState.OnEnter();

			if (state is IGameStateEnterAsync enterStateAsync)
				CoroutineExecutor.Run(enterStateAsync.OnEnter());
		}
	}
}