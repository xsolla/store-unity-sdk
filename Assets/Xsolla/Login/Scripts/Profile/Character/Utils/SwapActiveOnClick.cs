using UnityEngine;

namespace Xsolla.Demo
{
	public class SwapActiveOnClick : MonoBehaviour
	{
		[SerializeField] private SimpleButton Button;
		[SerializeField] private GameObject ActiveObject;
		[SerializeField] private GameObject InactiveObject;

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
