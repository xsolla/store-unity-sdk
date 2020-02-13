using System;
using UnityEngine;
using Xsolla.Core;

public class ItemsTabControl : MonoBehaviour
{
	const string StoreButtonText = "Store";
	const string CartButtonText = "Cart";

	[SerializeField]
	MenuButton storeButton;
	[SerializeField]
	MenuButton inventoryButton;
	[SerializeField]
	VirtualCurrencyContainer virtualCurrencyBalance;
	public VirtualCurrencyContainer VirtualCurrencyBalance { get => virtualCurrencyBalance; set => virtualCurrencyBalance = value; }

	private ItemsController itemsController;
	private GroupsController groupsController;

	public void Init()
	{
		groupsController = FindObjectOfType<GroupsController>();
		itemsController = FindObjectOfType<ItemsController>();
		
		storeButton.gameObject.SetActive(true);
		inventoryButton.gameObject.SetActive(true);
		
		storeButton.Select(false);

		storeButton.onClick = DeactivateInventoryTab;
		inventoryButton.onClick = ActivateInventoryTab;

		StoreTabsHotkey hotkey = gameObject.GetComponent<StoreTabsHotkey>();
		hotkey.TabKeyPressedEvent += TabKeyPressed;
		hotkey.LeftArrowKeyPressedEvent += LeftArrowKeyPressed;
		hotkey.RightArrowKeyPressedEvent += RightArrowKeyPressed;
	}

	private void TabKeyPressed()
	{
		if (inventoryButton.IsSelected) {
			storeButton.Select();
			DeactivateInventoryTab();
		} else {
			inventoryButton.Select();
			ActivateInventoryTab();
		}
	}

	private void LeftArrowKeyPressed()
	{
		if (inventoryButton.IsSelected) {
			storeButton.Select();
			DeactivateInventoryTab();
		}
	}

	private void RightArrowKeyPressed()
	{
		if (!inventoryButton.IsSelected) {
			inventoryButton.Select();
			ActivateInventoryTab();
		}
	}

	private void DeactivateInventoryTab(string s = "")
	{
		inventoryButton.Deselect();

		var selectedGroup = groupsController.GetSelectedGroup();
		if (selectedGroup != null) {
			itemsController.ActivateContainer(selectedGroup.Id);
		}
	}

	private void ActivateInventoryTab(string s = "")
	{
		storeButton.Deselect();
		itemsController.ActivateContainer(Constants.InventoryContainerName);
	}

	public void ActivateStoreTab(string groupID)
	{
		storeButton.Text = (groupID == Constants.CartGroupName) ? CartButtonText : StoreButtonText;
		storeButton.Select(false);
		inventoryButton.Deselect();
	}
}