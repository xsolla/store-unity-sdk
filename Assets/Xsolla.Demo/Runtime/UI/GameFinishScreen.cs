using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class GameFinishScreen : Screen
	{
		[field: SerializeField] private Button PlayAgainButton { get; set; }
		[field: SerializeField] private Button IntegrateXsollaButton { get; set; }
		[field: SerializeField] private Button HowItWorksButton { get; set; }

		private UrlService UrlService => ServiceLocator.Resolve<UrlService>();
		private GameStateMachine GameStateMachine => ServiceLocator.Resolve<GameStateMachine>();

		private void Start()
		{
			PlayAgainButton.onClick.AddListener(() => GameStateMachine.SwitchToGamePlay());
			IntegrateXsollaButton.onClick.AddListener(() => UrlService.OpenIntegrationGuide());
			HowItWorksButton.onClick.AddListener(() => UrlService.OpenHowItWorks());
		}
	}
}