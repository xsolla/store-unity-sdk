using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class OnboardingGameplayScreen : Screen
	{
		[field: SerializeField] private Button PlayButton { get; set; }

		private InputService InputService => ServiceLocator.Resolve<InputService>();
		private GameStateMachine GameStateMachine => ServiceLocator.Resolve<GameStateMachine>();

		private void Start()
		{
			PlayButton.onClick.AddListener(ToggleGamePlay);
		}

		private void Update()
		{
			if (InputService.IsSkipOnboardingRequested())
				ToggleGamePlay();
		}

		private void ToggleGamePlay()
			=> GameStateMachine.SwitchToGamePlay();
	}
}