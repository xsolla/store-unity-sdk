using UnityEngine;

namespace Xsolla.Demo
{
	public class SwapActiveOnClick : MonoBehaviour
	{
		[SerializeField] private SimpleButton Button = default;
		[SerializeField] private GameObject ActiveObject = default;
		[SerializeField] private GameObject InactiveObject = default;

		private void Awake()
		{
			Button.onClick += SwapActiveState;
		}

		private void SwapActiveState()
		{
			ActiveObject.SetActive(false);
			InactiveObject.SetActive(true);
		}
	}
}
