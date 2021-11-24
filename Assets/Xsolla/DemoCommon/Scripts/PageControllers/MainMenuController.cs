using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;

namespace Xsolla.Demo
{
	public class MainMenuController : BaseMenuController
	{
		[SerializeField] private SimpleButton storeButton;
		[SerializeField] private SimpleButton battlepassButton;
		[SerializeField] private SimpleButton inventoryButton;
		[SerializeField] private SimpleButton webStoreButton;
		[SerializeField] private AccountLinkingManagerManual accountLinkingManager;
		[SerializeField] private SimpleButton profileButton;
		[SerializeField] private SimpleButton characterButton;
		[SerializeField] private SimpleButton friendsButton;
		[SerializeField] private SimpleButton publisherAccountButton;
		[SerializeField] private SimpleButton documentationButton;
		[SerializeField] private SimpleButton feedbackButton;
		[SerializeField] private SimpleButton logoutButton;
		[SerializeField] private SimpleButton tutorialButton;

		private void Start()
		{
			if (DemoMarker.IsStoreDemo)
				InitStoreDemo();
			else if (DemoMarker.IsInventoryDemo)
				InitInventoryDemo();
			else/*if (DemoMarker.IsLoginDemo)*/
				InitLoginDemo();
		}

		private void InitLoginDemo()
		{
			InitLoginButtons();
			InitCommonButtons();
		}

		private void InitStoreDemo()
		{
			AttachButtonCallback(storeButton,
				() => SetMenuState(MenuState.Store, () => UserCatalog.Instance.IsUpdated && UserInventory.Instance.IsUpdated));

			if (!DemoController.Instance.IsAccessTokenAuth)
			{
				AttachUrlToButton(webStoreButton, DemoController.Instance.GetWebStoreUrl());
				AttachButtonCallback(battlepassButton,
					() => SetMenuState(MenuState.Battlepass, () => UserCatalog.Instance.IsUpdated));
			}

			AttachButtonCallback(inventoryButton, () =>
			{
				if (UserInventory.Instance.IsUpdated)
				{
					UserInventory.Instance.Refresh(() => SetMenuState(MenuState.Inventory), StoreDemoPopup.ShowError);
					PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => UserInventory.Instance.IsUpdated);
				}
				else
					SetMenuState(MenuState.Inventory, () => UserInventory.Instance.IsUpdated);
			});

			InitLoginButtons();
			InitCommonButtons();
		}

		private void InitInventoryDemo()
		{
			AttachButtonCallback(inventoryButton, () =>
			{
				if (UserInventory.Instance.IsUpdated)
				{
					UserInventory.Instance.Refresh(() => SetMenuState(MenuState.Inventory), StoreDemoPopup.ShowError);
					PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => UserInventory.Instance.IsUpdated);
				}
				else
					SetMenuState(MenuState.Inventory, () => UserInventory.Instance.IsUpdated);
			});

			if (!DemoController.Instance.IsAccessTokenAuth)
			{
				AttachUrlToButton(webStoreButton, DemoController.Instance.GetWebStoreUrl());
				accountLinkingManager.Init();

				AttachButtonCallback(tutorialButton, () =>
				{
					if (DemoController.Instance.IsTutorialAvailable)
						DemoController.Instance.ShowTutorial(false);
				});
			}
			
			InitCommonButtons();
		}

		private void InitLoginButtons()
		{
			if (DemoController.Instance.IsAccessTokenAuth)
				return;

			AttachButtonCallback(profileButton,
				() => SetMenuState(MenuState.Profile, () => UserCatalog.Instance.IsUpdated));
			AttachButtonCallback(characterButton,
				() => SetMenuState(MenuState.Character, () => UserCatalog.Instance.IsUpdated));

			AttachButtonCallback(friendsButton, HandleFriendsButton);
		}

		private void HandleFriendsButton()
		{
			UserFriends.Instance.UpdateFriends(
			onSuccess: () =>
			{
				SetMenuState(MenuState.Friends, () => UserFriends.Instance.IsUpdated);
			},
			onError: error =>
			{
				StoreDemoPopup.ShowError(error);
				DemoController.Instance.SetState(MenuState.Authorization);
			});
		}

		private void InitCommonButtons()
		{
			AttachUrlToButton(publisherAccountButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.PublisherUrl));
			AttachUrlToButton(documentationButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.DocumentationUrl));
			AttachUrlToButton(feedbackButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.FeedbackUrl));

			AttachButtonCallback(logoutButton,
				() => SetMenuState(MenuState.Authorization, () => !WebRequestHelper.Instance.IsBusy()));
		}
	}
}
