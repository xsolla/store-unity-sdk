using UnityEngine;

public class SwapActiveOnClick : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] private SimpleButton Button;
	[SerializeField] private GameObject ActiveObject;
	[SerializeField] private GameObject InactiveObject;
#pragma warning restore 0649

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
