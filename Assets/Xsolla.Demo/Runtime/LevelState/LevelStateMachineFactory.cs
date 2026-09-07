namespace Xsolla.Demo
{
	public class LevelStateMachineFactory
	{
		public LevelStateMachine Create()
		{
			var stateFactory = new LevelStateFactory();
			return new LevelStateMachine(stateFactory);
		}
	}
}