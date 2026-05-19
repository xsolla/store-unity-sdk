using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class GameOverScreen : Screen
	{
		[field: SerializeField] private Button PlayAgainButton { get; set; }

		private GameStateMachine GameStateMachine => ServiceLocator.Resolve<GameStateMachine>();

		private void Start()
		{
			PlayAgainButton.onClick.AddListener(() => GameStateMachine.SwitchToGamePlay());
		}
	}
}