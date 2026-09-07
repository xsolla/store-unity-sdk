namespace Xsolla.Demo
{
	public class GameStateMachineFactory
	{
		public GameStateMachine Create()
		{
			var stateFactory = new GameStateFactory();
			return new GameStateMachine(stateFactory);
		}
	}
}