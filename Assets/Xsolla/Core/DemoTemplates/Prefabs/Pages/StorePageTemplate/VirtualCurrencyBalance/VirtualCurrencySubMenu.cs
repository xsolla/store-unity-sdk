using UnityEngine;

public class VirtualCurrencySubMenu : MonoBehaviour
{
	[SerializeField] private SimpleButton buyCurrencyButton;
	[SerializeField] private GameObject virtualCurrencyBalance;

	private void Start()
	{
		if (!DemoController.Instance.IsStateAvailable(MenuState.BuyCurrency))
		{
			buyCurrencyButton.gameObject.SetActive(false);

			var balancePosition = virtualCurrencyBalance.GetComponent<RectTransform>().anchoredPosition;
			balancePosition.x = 0;
			virtualCurrencyBalance.GetComponent<RectTransform>().anchoredPosition = balancePosition;
		}
	}
}