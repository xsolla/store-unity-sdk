namespace Xsolla.Demo
{
	public class GameStateFactory
	{
		public IGameState CreateGameAwake()
			=> new GameAwakeState();

		public IGameState CreateUserAuthStart()
			=> new UserAuthStartState();

		public IGameState CreateUserAuthFinish()
			=> new UserAuthFinishState();

		public IGameState CreateUserOnboarding()
			=> new UserOnboardingState();

		public IGameState CreateGamePlay()
			=> new GamePlayState();

		public IGameState CreateGameFinish()
			=> new GameFinishState();

		public IGameState CreateGameOver()
			=> new GameOverState();
	}
}