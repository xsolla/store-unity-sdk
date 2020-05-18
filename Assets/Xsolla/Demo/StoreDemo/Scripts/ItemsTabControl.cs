using System;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Store;

public partial class ItemsTabControl : MonoBehaviour
{
	private const string STORE_BUTTON_TEXT = "Store";
	private const string CART_BUTTON_TEXT = "Cart";

	[SerializeField]
	MenuButton storeButton;
	[SerializeField]
	MenuButton inventoryButton;
	[SerializeField]
	MenuButton subscriptionsButton;
	[SerializeField]
	MenuButton attributesButton;
	[SerializeField]
	VirtualCurrencyContainer virtualCurrencyBalance;

	private ItemsController _itemsController;
	private GroupsController _groupsController;

	private void Awake()
	{
		UserCatalog.Instance.UpdateVirtualCurrenciesEvent += virtualCurrencyBalance.SetCurrencies;
		UserInventory.Instance.UpdateVirtualCurrencyBalanceEvent += virtualCurrencyBalance.SetCurrenciesBalance;
	}

	public void Init()
	{
		_groupsController = FindObjectOfType<GroupsController>();
		_itemsController = FindObjectOfType<ItemsController>();

		storeButton.gameObject.SetActive(true);
		inventoryButton.gameObject.SetActive(true);
		subscriptionsButton.gameObject.SetActive(true);
		attributesButton.gameObject.SetActive(true);

		storeButton.Select(false);

		attributesButton.onClick = _ => InternalActivateAttributesTab();
		storeButton.onClick = _ => InternalActivateStoreTab();
		inventoryButton.onClick = _ => InternalActivateInventoryTab();
		subscriptionsButton.onClick = _ => InternalActivateSubscriptionsTab();

		InitHotKeys();
	}

	private void InternalActivateStoreTab()
	{
		attributesButton.Deselect();
		inventoryButton.Deselect();
		subscriptionsButton.Deselect();

		var selectedGroup = _groupsController.GetSelectedGroup();
		if (selectedGroup != null)
		{
			_itemsController.ActivateContainer(selectedGroup.Id);
		}
	}

	private void InternalActivateInventoryTab()
	{
		attributesButton.Deselect();
		storeButton.Deselect();
		subscriptionsButton.Deselect();
		_itemsController.ActivateContainer(Constants.InventoryContainerName);
	}

	private void InternalActivateSubscriptionsTab()
	{
		storeButton.Deselect();
		inventoryButton.Deselect();
		attributesButton.Deselect();
		_itemsController.ActivateContainer(Constants.SubscriptionsContainerName);
	}

	private void InternalActivateAttributesTab()
	{
		storeButton.Deselect();
		inventoryButton.Deselect();
		subscriptionsButton.Deselect();
		_itemsController.ActivateContainer(Constants.AttributesContainerName);
	}

	public void ActivateStoreTab(string groupId)
	{
		storeButton.Text = (groupId == Constants.CartGroupName) ? CART_BUTTON_TEXT : STORE_BUTTON_TEXT;
		storeButton.Select(false);
		attributesButton.Deselect();
		inventoryButton.Deselect();
		subscriptionsButton.Deselect();
	}
}