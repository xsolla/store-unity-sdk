using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class MainMenuController : BaseMenuController
	{
		[SerializeField] private SimpleButton storeButton = default;
		[SerializeField] private SimpleButton inventoryButton = default;
		[SerializeField] private SimpleButton documentationButton = default;
		[SerializeField] private SimpleButton publisherAccountButton = default;
		[SerializeField] private SimpleButton profileButton = default;
		[SerializeField] private SimpleButton characterButton = default;
		[SerializeField] private SimpleButton friendsButton = default;
		[SerializeField] private SimpleButton feedbackButton = default;
		[SerializeField] private SimpleButton logoutButton = default;

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
				() => SetMenuState(MenuState.Authorization, () => !WebRequestHelper.Instance.IsBusy()));
		
			AttachUrlToButton(documentationButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.DocumentationUrl));
			AttachUrlToButton(feedbackButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.FeedbackUrl));
			AttachUrlToButton(publisherAccountButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.PublisherUrl));
		}
	}
}
