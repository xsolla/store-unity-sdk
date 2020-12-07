using UnityEngine;

namespace Xsolla.Demo
{
	public class InventoryMainMenuController : BaseMenuController
	{
		[SerializeField] private SimpleButton inventoryButton = default;
		[SerializeField] private SimpleButton webStoreButton = default;
		[SerializeField] private SimpleButton documentationButton = default;
		[SerializeField] private SimpleButton publisherAccountButton = default;
		[SerializeField] private SimpleButton logoutButton = default;
		[SerializeField] private SimpleButton tutorialButton = default;

		private void Start()
		{
			AttachButtonCallback(inventoryButton,
				() => SetMenuState(MenuState.Inventory, () => UserInventory.Instance.IsUpdated));
			AttachButtonCallback(logoutButton,
				() => SetMenuState(MenuState.Authorization));

			AttachUrlToButton(documentationButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.DocumentationUrl));
			AttachUrlToButton(publisherAccountButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.PublisherUrl));

			AttachUrlToButton(webStoreButton, DemoController.Instance.GetWebStoreUrl());

			AttachButtonCallback(tutorialButton,
				() =>
				{
					if (DemoController.Instance.IsTutorialAvailable)
						DemoController.Instance.TutorialManager.ShowTutorial(false);
				});
		}
	}
}
