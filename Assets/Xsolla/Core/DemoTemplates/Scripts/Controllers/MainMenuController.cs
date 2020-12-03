using UnityEngine;

public class MainMenuController : BaseMenuController
{
	[SerializeField] private SimpleButton storeButton;
	[SerializeField] private SimpleButton inventoryButton;
	[SerializeField] private SimpleButton documentationButton;
	[SerializeField] private SimpleButton publisherAccountButton;
	[SerializeField] private SimpleButton profileButton;
	[SerializeField] private SimpleButton characterButton;
	[SerializeField] private SimpleButton friendsButton;
	[SerializeField] private SimpleButton feedbackButton;
	[SerializeField] private SimpleButton logoutButton;
	
	private void Start()
	{
		AttachButtonCallback(storeButton, 
			() => SetMenuState(MenuState.Store, () => UserCatalog.Instance.IsUpdated && UserInventory.Instance.IsUpdated));
		AttachButtonCallback(inventoryButton, 
			() => SetMenuState(MenuState.Inventory, () => UserInventory.Instance.IsUpdated));
		AttachButtonCallback(profileButton,
			() => SetMenuState(MenuState.Profile, () => UserCatalog.Instance.IsUpdated));
		AttachButtonCallback(characterButton,
			() => SetMenuState(MenuState.Character, () => UserCatalog.Instance.IsUpdated));
		AttachButtonCallback(friendsButton, 
			() => SetMenuState(MenuState.Friends, () => UserFriends.Instance.IsUpdated));
		AttachButtonCallback(logoutButton, 
			() => SetMenuState(MenuState.Authorization));
		
		AttachUrlToButton(documentationButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.DocumentationUrl));
		AttachUrlToButton(feedbackButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.FeedbackUrl));
		AttachUrlToButton(publisherAccountButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.PublisherUrl));
	}
}
