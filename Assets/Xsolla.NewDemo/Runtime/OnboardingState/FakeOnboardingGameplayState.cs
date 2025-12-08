#if DEBUG_XSOLLA_DEMO
namespace Xsolla.Demo
{
	public class FakeOnboardingGameplayState : IOnboardingState
	{
		private static GameStateMachine GameStateMachine => ServiceLocator.Resolve<GameStateMachine>();

		public void OnEnter()
		{
			GameStateMachine.SwitchToGamePlay();
		}
	}
}
#endif