using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;
using Xsolla.Store;

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
		AttachButtonCallback(storeButton, () => SetMenuState(MenuState.Store));
		AttachButtonCallback(inventoryButton, () => SetMenuState(MenuState.Inventory));
		AttachButtonCallback(profileButton, () => SetMenuState(MenuState.Profile));
		AttachButtonCallback(logoutButton, () => SetMenuState(MenuState.Authorization));
		
		AttachUrlToButton(documentationButton, DemoController.Instance.documentationUrl);
		AttachUrlToButton(feedbackButton, DemoController.Instance.feedbackUrl);
		AttachUrlToButton(publisherAccountButton, DemoController.Instance.publisherUrl);

		Initialization();
	}

	private static void SetMenuState(MenuState state)
	{
		if (UserInventory.Instance.IsUpdated && UserCatalog.Instance.IsUpdated)
			DemoController.Instance.SetState(state);
		else
		{
			PopupFactory.Instance.CreateWaiting()
				.SetCloseCondition(() => UserInventory.Instance.IsUpdated && UserCatalog.Instance.IsUpdated)
				.SetCloseHandler(() => DemoController.Instance.SetState(state));
		}
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
