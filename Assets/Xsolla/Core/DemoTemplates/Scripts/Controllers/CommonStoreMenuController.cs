using UnityEngine;

public class CommonStoreMenuController : BaseMenuController
{
	[SerializeField] private SimpleButton publisherAccountButton;
	[SerializeField] private SimpleButton documentationButton;
	[SerializeField] private SimpleButton testCardsButton;
	[SerializeField] private SimpleButton backButton;
	[SerializeField] private SimpleButton buyCurrencyButton;

	private void Start()
	{
		AttachUrlToButton(publisherAccountButton, DemoController.Instance.publisherUrl);
		AttachUrlToButton(documentationButton, DemoController.Instance.documentationUrl);
		AttachUrlToButton(testCardsButton, DemoController.Instance.testCardsUrl);
		AttachButtonCallback(backButton, () => DemoController.Instance.SetPreviousState());
		AttachButtonCallback(buyCurrencyButton, 
			() => SetMenuState(MenuState.BuyCurrency, () => UserCatalog.Instance.IsUpdated));
	}
}
