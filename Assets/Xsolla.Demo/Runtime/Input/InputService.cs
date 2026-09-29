using UnityEngine;

namespace Xsolla.Demo
{
	public class InputService
	{
		public bool IsSkipOnboardingRequested()
		{
			return Input.GetKeyDown(KeyCode.Space);
		}
	}
}