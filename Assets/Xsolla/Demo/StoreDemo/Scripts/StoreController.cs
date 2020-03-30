using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;
using Xsolla.Login;
using Xsolla.Store;

public class StoreController : MonoBehaviour
{	
	GroupsController _groupsController;
	ItemsController _itemsController;
	IExtraPanelController _extraController;
	ItemsTabControl _itemsTabControl;

	[HideInInspector]
	public InventoryItems inventory;
	
	public CartModel CartModel { get; private set; }

	public static Dictionary<string, Sprite> ItemIcons;

	const string DefaultStoreToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJleHAiOjE5NjIyMzQwNDgsImlzcyI6Imh0dHBzOi8vbG9naW4ueHNvbGxhLmNvbSIsImlhdCI6MTU2MjE0NzY0OCwidXNlcm5hbWUiOiJ4c29sbGEiLCJ4c29sbGFfbG9naW5fYWNjZXNzX2tleSI6IjA2SWF2ZHpDeEVHbm5aMTlpLUc5TmMxVWFfTWFZOXhTR3ZEVEY4OFE3RnMiLCJzdWIiOiJkMzQyZGFkMi05ZDU5LTExZTktYTM4NC00MjAxMGFhODAwM2YiLCJlbWFpbCI6InN1cHBvcnRAeHNvbGxhLmNvbSIsInR5cGUiOiJ4c29sbGFfbG9naW4iLCJ4c29sbGFfbG9naW5fcHJvamVjdF9pZCI6ImU2ZGZhYWM2LTc4YTgtMTFlOS05MjQ0LTQyMDEwYWE4MDAwNCIsInB1Ymxpc2hlcl9pZCI6MTU5MjR9.GCrW42OguZbLZTaoixCZgAeNLGH2xCeJHxl8u8Xn2aI";

	public Cart Cart { get; private set; }

	private bool isInventoryLoaded;
	private bool isCatalogLoaded;

	[HideInInspector]
	public List<UserAttribute> attributes;
	private void Awake()
	{		
		attributes = new List<UserAttribute>();
	}

	void Start()
	{
		_groupsController = FindObjectOfType<GroupsController>();
		_itemsController = FindObjectOfType<ItemsController>();
		_extraController = FindObjectOfType<ExtraController>();
		_extraController.LinkingAccountComplete += _extraController_LinkingAccountComplete;
		_itemsTabControl = FindObjectOfType<ItemsTabControl>();

		CartModel = new CartModel();
		
		ItemIcons = new Dictionary<string, Sprite>();

		CheckAuth();

		isCatalogLoaded = false;
		isInventoryLoaded = false;

		XsollaStore.Instance.CreateNewCart(XsollaSettings.StoreProjectId, newCart => { Cart = newCart; }, ShowError);

		XsollaStore.Instance.GetListOfItems(XsollaSettings.StoreProjectId, InitStoreUi, ShowError);

		RefreshInventory(() => isInventoryLoaded = true);

		StartCoroutine(LockPurchasedNonConsumableItemsCoroutine());
	}

	private void _extraController_LinkingAccountComplete()
	{
		RefreshInventory();
		RefreshVirtualCurrencyBalance();
	}

	private void CheckAuth()
	{
		if (XsollaLogin.IsExist && !XsollaLogin.Instance.Token.IsNullOrEmpty()) {
			print("Store demo starts. Use token obtained from Login: " + XsollaLogin.Instance.Token);
			XsollaStore.Instance.Token = XsollaLogin.Instance.Token;
		} else {
			XsollaStore.Instance.Token = LauncherArguments.Instance.GetToken();
			if (XsollaStore.Instance.Token.IsNullOrEmpty()) {
				XsollaStore.Instance.Token = DefaultStoreToken;
				print("Store demo starts. Use default hardcoded token: " + XsollaStore.Instance.Token);
			} else {
				print("Store demo starts. Use token obtained from Launcher: " + XsollaStore.Instance.Token);
			}
		}
	}

	public void RefreshInventory(Action refreshCallback = null)
	{
		XsollaStore.Instance.GetInventoryItems(
			XsollaSettings.StoreProjectId,
			(items) => { SetInventoryItems(items); refreshCallback?.Invoke(); },
			ShowError);
	}

	IEnumerator LockPurchasedNonConsumableItemsCoroutine()
	{
		yield return new WaitUntil(() => isInventoryLoaded && isCatalogLoaded);
		LockPurchasedNonConsumableItems();
	}

	void LockPurchasedNonConsumableItems()
	{
		List<ItemUI> catalogItems = new List<ItemUI>();
		List<ItemContainer> itemContainers = _itemsController.GetCatalogContainers();
		foreach (ItemContainer itemContainer in itemContainers) {
			catalogItems.AddRange(itemContainer.Items.Where(i => !i.IsConsumable()));
		}
		List<InventoryItem> inventoryItems = inventory.items.ToList();
		catalogItems = catalogItems.Where(i => inventoryItems.Count((item) => item.sku == i.GetSku()) > 0).ToList();
		catalogItems.ForEach(i => i.Lock());
	}

	public void RefreshAttributes(Action refreshCallback = null)
	{
		XsollaLogin.Instance.GetUserAttributes(XsollaStore.Instance.Token, XsollaSettings.StoreProjectId, null, null, list =>
		{
			attributes = list;
			_extraController.SetAttributes(list);
			refreshCallback?.Invoke();
		}, ShowError);
	}

	void SetInventoryItems(InventoryItems items)
	{
		items.items = FilterVirtualCurrency(items.items);
		inventory = items;
		_itemsController.RefreshActiveContainer();
	}

	public void RefreshVirtualCurrencyBalance(Action refreshCallback = null)
	{
		XsollaStore.Instance.GetVirtualCurrencyBalance(
			XsollaSettings.StoreProjectId,
			(balance) => {
				_itemsTabControl.VirtualCurrencyBalance.SetCurrenciesBalance(balance);
				refreshCallback?.Invoke();},
			ShowError);
	}

	InventoryItem[] FilterVirtualCurrency(InventoryItem[] items)
	{
		return items.ToList().Where(i => i.type != "virtual_currency").ToArray();
	}

	void InitStoreUi(StoreItems items)
	{
		XsollaStore.Instance.GetListOfItemGroups(XsollaSettings.StoreProjectId, groups =>
		{
			_groupsController.CreateGroups(items, groups);
			_itemsController.CreateItems(items);

			_itemsTabControl.Init();
			_extraController.Initialize();

			_groupsController.SelectDefault();
			XsollaStore.Instance.GetVirtualCurrencyPackagesList(XsollaSettings.StoreProjectId,
				packages => {
					_itemsController.AddVirtualCurrencyPackage(packages);
					packages.items.ForEach(p => ImageLoader.Instance.GetImageAsync(p.image_url, null));
				}, ShowError);
			XsollaStore.Instance.GetVirtualCurrencyList(
				XsollaSettings.StoreProjectId,
				currencies =>
				{
					_itemsTabControl.VirtualCurrencyBalance.SetCurrencies(currencies);
					RefreshVirtualCurrencyBalance();
					currencies.items.ToList().ForEach(c => ImageLoader.Instance.GetImageAsync(c.image_url, null));
				} , ShowError);

			RefreshAttributes();
			isCatalogLoaded = true;

			items.items.ToList().ForEach(i => ImageLoader.Instance.GetImageAsync(i.image_url, null));
		}, ShowError);
	}

	public void ProcessOrder(int orderId, Action onOrderPaid = null)
	{
		StartCoroutine(CheckOrderStatus(orderId, onOrderPaid));
	}

	IEnumerator CheckOrderStatus(int orderId, Action onOrderPaid = null)
	{
		yield return new WaitForSeconds(3.0f);
		
		XsollaStore.Instance.CheckOrderStatus(XsollaSettings.StoreProjectId, orderId,status =>
		{
			if ((status.Status != OrderStatusType.Paid) && (status.Status != OrderStatusType.Done))
			{
				print(string.Format("Waiting for order {0} to be processed...", orderId));
				StartCoroutine(CheckOrderStatus(orderId, onOrderPaid));
			}
			else
			{
				print(string.Format("Order {0} was successfully processed!", orderId));

				ShowSuccess();
				RefreshInventory(() => {
					LockPurchasedNonConsumableItems();
					onOrderPaid();
				});
			}
		}, ShowError);
	}
	
	void OnDestroy()
	{
		StopAllCoroutines();
	}

	public void ResetCart()
	{
		XsollaStore.Instance.CreateNewCart(XsollaSettings.StoreProjectId, newCart =>
		{
			Cart = newCart; 
			CartModel.Clear();

			var cartGroup = FindObjectOfType<CartGroupUI>();
			cartGroup.ResetCounter();

			_itemsController.RefreshActiveContainer();
		}, ShowError);
	}

	public void ShowSuccess(string message = "") => PopupFactory.Instance.CreateSuccess().SetMessage(message);

	public void ShowError(Error error)
	{
		print(error);
		PopupFactory.Instance.CreateError().SetMessage(error.ToString());
	}

	public void ShowConfirm(Action confirmCase, Action cancelCase = null, string message = "") => PopupFactory.Instance.
		CreateConfirmation().
		SetMessage(message).
		SetConfirmCallback(confirmCase).
		SetCancelCallback(cancelCase);

	public void ShowConfirmCode(Action<string> confirmCase, Action cancelCase = null) => PopupFactory.Instance.
		CreateCodeConfirmation().
		SetConfirmCallback(confirmCase).
		SetCancelCallback(cancelCase);
}