using UnityEngine;

namespace Xsolla.Demo
{
	public class OnEnableButtonDisabler : MonoBehaviour
    {
		[SerializeField] private SimpleTextButtonDisableable Button = default;

		private void OnEnable()
		{
			Button.Disable();
		}
	}
}
