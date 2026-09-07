using UnityEngine;

namespace Xsolla.Demo
{
	public class DebugController : MonoBehaviour
	{
		private GameStateMachine GameStateMachine => ServiceLocator.Resolve<GameStateMachine>();

		private void Update()
		{
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				if (Input.GetKeyDown(KeyCode.F1))
				{
					PlayerPrefs.DeleteAll();
					GameStateMachine.SwitchToGameAwake();
				}
			}
		}
	}
}