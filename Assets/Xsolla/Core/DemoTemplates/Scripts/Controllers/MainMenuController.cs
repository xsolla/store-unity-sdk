using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;

public class MainMenuController : BaseMenuController
{
	[SerializeField] private SimpleButton storeButton;
	[SerializeField] private SimpleButton inventoryButton;
	[SerializeField] private SimpleButton documentationButton;
	[SerializeField] private SimpleButton publisherAccountButton;
	[SerializeField] private SimpleButton profileButton;
	[SerializeField] private SimpleButton feedbackButton;
	[SerializeField] private SimpleButton logoutButton;
	
	private void Start()
	{
		AttachButtonCallback(storeButton, () => DemoController.Instance.SetState(MenuState.Store));
		AttachButtonCallback(inventoryButton, () => DemoController.Instance.SetState(MenuState.Inventory));
		AttachButtonCallback(profileButton, () => DemoController.Instance.SetState(MenuState.Profile));
		AttachButtonCallback(logoutButton, () => DemoController.Instance.SetState(MenuState.Authorization));
		
		AttachUrlToButton(documentationButton, DemoController.Instance.documentationUrl);
		AttachUrlToButton(feedbackButton, DemoController.Instance.feedbackUrl);
		AttachUrlToButton(publisherAccountButton, DemoController.Instance.publisherUrl);

		Initialization();
	}

	private static void Initialization()
	{
		var impl = DemoController.Instance.GetImplementation();
		if(!UserInventory.Instance.IsUpdated)
			UserInventory.Instance.Init(impl);
		if (UserCatalog.Instance.IsUpdated) return;
		UserCatalog.Instance.Init(impl);
		UserCatalog.Instance.UpdateItems(() =>
		{ 	// This method used for fastest async image loading
			StartLoadItemImages(UserCatalog.Instance.AllItems);
			UserInventory.Instance.Refresh();
		});
	}
	
	private static void StartLoadItemImages(List<CatalogItemModel> items)
	{
		items.ForEach(i => ImageLoader.Instance.GetImageAsync(i.ImageUrl, null));
	}
}
