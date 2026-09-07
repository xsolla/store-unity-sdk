namespace Xsolla.Demo
{
	public class UserOnboardingState : IGameState, IGameStateEnter
	{
		private OnboardingStateMachine OnboardingStateMachine => ServiceLocator.Resolve<OnboardingStateMachine>();
		private GuyService GuyService => ServiceLocator.Resolve<GuyService>();
		private FoxService FoxService => ServiceLocator.Resolve<FoxService>();
		private CameraService CameraService => ServiceLocator.Resolve<CameraService>();

		public void OnEnter()
		{
			var pawnMode = PawnMode.Onboarding;
			GuyService.SpawnGuy(pawnMode);
			FoxService.SpawnFox(pawnMode);
			CameraService.ToggleCamera(pawnMode);

			OnboardingStateMachine.SwitchToControlsState();
		}
	}
}