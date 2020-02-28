using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Login;
using Xsolla.Store;


public class StoreController : MonoBehaviour
{
	[SerializeField]
	GameObject popupPrefab;
	
	GroupsController _groupsController;
	ItemsController _itemsController;
	ExtraController _extraController;
	ItemsTabControl _itemsTabControl;

	MessagePopup _popup;

	[HideInInspector]
	public InventoryItems inventory;
	
	[HideInInspector]
	public List<UserAttribute> attributes;
	public CartModel CartModel { get; private set; }

	public static Dictionary<string, Sprite> ItemIcons;
	private ImageLoader imageLoader;

	const string DefaultStoreToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJleHAiOjE5NjIyMzQwNDgsImlzcyI6Imh0dHBzOi8vbG9naW4ueHNvbGxhLmNvbSIsImlhdCI6MTU2MjE0NzY0OCwidXNlcm5hbWUiOiJ4c29sbGEiLCJ4c29sbGFfbG9naW5fYWNjZXNzX2tleSI6IjA2SWF2ZHpDeEVHbm5aMTlpLUc5TmMxVWFfTWFZOXhTR3ZEVEY4OFE3RnMiLCJzdWIiOiJkMzQyZGFkMi05ZDU5LTExZTktYTM4NC00MjAxMGFhODAwM2YiLCJlbWFpbCI6InN1cHBvcnRAeHNvbGxhLmNvbSIsInR5cGUiOiJ4c29sbGFfbG9naW4iLCJ4c29sbGFfbG9naW5fcHJvamVjdF9pZCI6ImU2ZGZhYWM2LTc4YTgtMTFlOS05MjQ0LTQyMDEwYWE4MDAwNCIsInB1Ymxpc2hlcl9pZCI6MTU5MjR9.GCrW42OguZbLZTaoixCZgAeNLGH2xCeJHxl8u8Xn2aI";

	public Cart Cart { get; private set; }

	private bool isInventoryLoaded;
	private bool isCatalogLoaded;

	private void Awake()
	{
		imageLoader = gameObject.AddComponent<ImageLoader>();
		
		attributes = new List<UserAttribute>();
	}

	void Start()
	{
		_groupsController = FindObjectOfType<GroupsController>();
		_itemsController = FindObjectOfType<ItemsController>();
		_extraController = FindObjectOfType<ExtraController>();
		_itemsTabControl = FindObjectOfType<ItemsTabControl>();
		
		CartModel = new CartModel();
		
		ItemIcons = new Dictionary<string, Sprite>();

		if (FindObjectOfType<XsollaLogin>() != null)
		{
			print("Store demo starts. Use token obtained from Login: " + XsollaLogin.Instance.Token);
			XsollaStore.Instance.Token = XsollaLogin.Instance.Token;
		}
		else
		{
			print("Store demo starts. Use default hardcoded token: " + DefaultStoreToken);
			XsollaStore.Instance.Token = DefaultStoreToken;
		}

		isCatalogLoaded = false;
		isInventoryLoaded = false;

		XsollaStore.Instance.CreateNewCart(XsollaSettings.StoreProjectId, newCart => { Cart = newCart; }, ShowError);

		XsollaStore.Instance.GetListOfItems(XsollaSettings.StoreProjectId, InitStoreUi, ShowError);

		RefreshInventory(() => isInventoryLoaded = true);

		StartCoroutine(LockPurchasedNonConsumableItemsCoroutine());
	}

	public void GetImageAsync(string url, Action<string, Sprite> callback)
	{
		imageLoader.GetImageAsync(url, callback);
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
			_extraController.RefreshAttributesPanel();
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
			_extraController.Init();

			_groupsController.SelectDefault();
			XsollaStore.Instance.GetVirtualCurrencyPackagesList(XsollaSettings.StoreProjectId, _itemsController.AddVirtualCurrencyPackage, ShowError);
			XsollaStore.Instance.GetVirtualCurrencyList(
				XsollaSettings.StoreProjectId,
				currencies =>
				{
					_itemsTabControl.VirtualCurrencyBalance.SetCurrencies(currencies);
					RefreshVirtualCurrencyBalance();
				} , ShowError);

			RefreshAttributes();
			isCatalogLoaded = true;
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

	MessagePopup PreparePopUp()
	{
		if (_popup == null)
			return Instantiate(popupPrefab, gameObject.transform).GetComponent<MessagePopup>();
		print("Popup is already shown");
		return null;
	}

	public void ShowSuccess() => PreparePopUp()?.ShowSuccess(() => { _popup = null; });

	public void ShowError(Xsolla.Core.Error error)
	{
		print(error);
		PreparePopUp()?.ShowError(error, () => { _popup = null; });
	}

	public void ShowConfirm(Action confirmCase, Action cancelCase, string message = "")
	{
		Text text = PreparePopUp()?.ShowConfirm(
			() => { confirmCase?.Invoke(); _popup = null; },
			() => { cancelCase?.Invoke(); _popup = null; }).confirmText;
		if (!String.IsNullOrEmpty(message)) {
			text.text = message;
		}
	}
}