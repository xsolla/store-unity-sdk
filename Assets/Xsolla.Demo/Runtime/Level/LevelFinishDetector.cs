using UnityEngine;

namespace Xsolla.Demo
{
	public class LevelFinishDetector : MonoBehaviour
	{
		private GameStateMachine GameStateMachine => ServiceLocator.Resolve<GameStateMachine>();

		private void OnTriggerEnter(Collider other)
		{
			var zone = other.GetComponent<LevelFinishZone>();
			if (zone)
				GameStateMachine.SwitchToGameFinish();
		}
	}
}