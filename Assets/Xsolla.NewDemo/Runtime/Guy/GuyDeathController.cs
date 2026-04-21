using UnityEngine;

namespace Xsolla.Demo
{
	public class GuyDeathController : MonoBehaviour, IPawnModeComponent
	{
		[SerializeField] private Transform Target;
		[SerializeField] private float MinPositionY = -1f;

		private GameStateMachine GameStateMachine => ServiceLocator.Resolve<GameStateMachine>();

		private void Update()
		{
			if (Target.position.y < MinPositionY)
				GameStateMachine.SwitchToGameOver();
		}

		public void ToggleMode(PawnMode pawnMode)
		{
			enabled = pawnMode == PawnMode.Gameplay;
		}
	}
}