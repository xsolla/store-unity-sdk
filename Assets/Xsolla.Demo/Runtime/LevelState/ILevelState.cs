namespace Xsolla.Demo
{
	public interface ILevelState
	{
		void OnEnter();
	}

	public interface ILevelStateExit
	{
		void OnExit();
	}
}