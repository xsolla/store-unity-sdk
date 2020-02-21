using System;
using UnityEngine;
using Xsolla.Core;

public partial class ItemsTabControl : MonoBehaviour
{
	const string StoreButtonText = "Store";
	const string CartButtonText = "Cart";

	[SerializeField]
	MenuButton storeButton;
	[SerializeField]
	MenuButton inventoryButton;
	[SerializeField]
	MenuButton attributesButton;
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
		attributesButton.gameObject.SetActive(true);
		
		storeButton.Select(false);

		attributesButton.onClick = InternalActivateAttributesTab;
		storeButton.onClick = InternalActivateStoreTab;
		inventoryButton.onClick = InternalActivateInventoryTab;

		InitHotKeys();	
	}

	private void InternalActivateStoreTab(string s = "")
	{
		attributesButton.Deselect();
		inventoryButton.Deselect();

		var selectedGroup = groupsController.GetSelectedGroup();
		if (selectedGroup != null) {
			itemsController.ActivateContainer(selectedGroup.Id);
		}
	}

	private void InternalActivateInventoryTab(string s = "")
	{
		attributesButton.Deselect();
		storeButton.Deselect();
		itemsController.ActivateContainer(Constants.InventoryContainerName);
	}

	private void InternalActivateAttributesTab(string s = "")
	{
		storeButton.Deselect();
		inventoryButton.Deselect();
		itemsController.ActivateContainer(Constants.AttributesContainerName);
	}

	public void ActivateStoreTab(string groupID)
	{
		storeButton.Text = (groupID == Constants.CartGroupName) ? CartButtonText : StoreButtonText;
		storeButton.Select(false);
		attributesButton.Deselect();
		inventoryButton.Deselect();
	}
}