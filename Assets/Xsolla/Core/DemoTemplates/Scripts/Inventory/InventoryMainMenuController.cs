using UnityEngine;

public class InventoryMainMenuController : BaseMenuController
{
	[SerializeField] private SimpleButton inventoryButton;
	[SerializeField] private SimpleButton webStoreButton;
	[SerializeField] private SimpleButton documentationButton;
	[SerializeField] private SimpleButton publisherAccountButton;
	[SerializeField] private SimpleButton logoutButton;
	
	private void Start()
	{
		AttachButtonCallback(inventoryButton, 
			() => SetMenuState(MenuState.Inventory, () => UserInventory.Instance.IsUpdated));
		AttachButtonCallback(logoutButton, 
			() => SetMenuState(MenuState.Authorization));

		AttachUrlToButton(documentationButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.DocumentationUrl));
		AttachUrlToButton(publisherAccountButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.PublisherUrl));

		AttachUrlToButton(webStoreButton, DemoController.Instance.GetWebStoreUrl());
	}
}