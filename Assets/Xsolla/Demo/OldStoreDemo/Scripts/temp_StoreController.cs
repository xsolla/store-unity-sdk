// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using Xsolla.Core;
// using Xsolla.Core.Popup;
// using Xsolla.Login;
// using Xsolla.Store;
//
// public class StoreController : MonoBehaviour
// {	
// 	GroupsController _groupsController;
// 	ItemsController _itemsController;
// 	IExtraPanelController _extraController;
// 	ItemsTabControl _itemsTabControl;
//
// 	const string DefaultStoreToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJleHAiOjE5NjIyMzQwNDgsImlzcyI6Imh0dHBzOi8vbG9naW4ueHNvbGxhLmNvbSIsImlhdCI6MTU2MjE0NzY0OCwidXNlcm5hbWUiOiJ4c29sbGEiLCJ4c29sbGFfbG9naW5fYWNjZXNzX2tleSI6IjA2SWF2ZHpDeEVHbm5aMTlpLUc5TmMxVWFfTWFZOXhTR3ZEVEY4OFE3RnMiLCJzdWIiOiJkMzQyZGFkMi05ZDU5LTExZTktYTM4NC00MjAxMGFhODAwM2YiLCJlbWFpbCI6InN1cHBvcnRAeHNvbGxhLmNvbSIsInR5cGUiOiJ4c29sbGFfbG9naW4iLCJ4c29sbGFfbG9naW5fcHJvamVjdF9pZCI6ImU2ZGZhYWM2LTc4YTgtMTFlOS05MjQ0LTQyMDEwYWE4MDAwNCIsInB1Ymxpc2hlcl9pZCI6MTU5MjR9.GCrW42OguZbLZTaoixCZgAeNLGH2xCeJHxl8u8Xn2aI";
// 	
// 	[HideInInspector]
// 	public List<UserAttribute> attributes = new List<UserAttribute>();
//
// 	private void OnDestroy()
// 	{
// 		StopAllCoroutines();
// 		
// 		if(UserCart.IsExist)
// 			Destroy(UserCart.Instance.gameObject);
// 		if(UserCatalog.IsExist)
// 			Destroy(UserCatalog.Instance.gameObject);
// 		if(UserInventory.IsExist)
// 			Destroy(UserInventory.Instance.gameObject);
// 	}
//
// 	private void Awake()
// 	{
// 		CheckAuth();
// 	}
//
// 	private void Start()
// 	{
// 		_groupsController = FindObjectOfType<GroupsController>();
// 		_itemsController = FindObjectOfType<ItemsController>();
// 		_itemsTabControl = FindObjectOfType<ItemsTabControl>();
// 		_extraController = FindObjectOfType<ExtraController>();
// 		_extraController.LinkingAccountComplete += RefreshUserState;
// 		
// 		CatalogInit();
// 		InventoryInit();
// 		CartInit();
// 	}
// 	
// 	private void CheckAuth()
// 	{
// 		if (XsollaLogin.IsExist && !XsollaLogin.Instance.Token.IsNullOrEmpty()) {
// 			print("Store demo starts. Use token obtained from Login: " + XsollaLogin.Instance.Token);
// 			XsollaStore.Instance.Token = XsollaLogin.Instance.Token;
// 		} else {
// 			XsollaStore.Instance.Token = LauncherArguments.Instance.GetToken();
// 			if (XsollaStore.Instance.Token.IsNullOrEmpty()) {
// 				XsollaStore.Instance.Token = DefaultStoreToken;
// 				print("Store demo starts. Use default hardcoded token: " + XsollaStore.Instance.Token);
// 			} else {
// 				print("Store demo starts. Use token obtained from Launcher: " + XsollaStore.Instance.Token);
// 			}
// 		}
// 	}
//
// 	private void CatalogInit()
// 	{
// 		UserCatalog.Instance.UpdateItemsEvent += _ => { LockPurchasedNonConsumableItems(); };
// 		UserCatalog.Instance.UpdateItems(InitStoreUi, StoreDemoPopup.ShowError);
// 	}
//
// 	private void CartInit()
// 	{
// 		UserCart.Instance.PurchaseCartEvent += () =>
// 		{
// 			RefreshUserState();
// 			StoreDemoPopup.ShowSuccess();
// 		};
// 	}
//
// 	private void InventoryInit()
// 	{
// 		UserInventory.Instance.UpdateItemsEvent += _ => LockPurchasedNonConsumableItems();
// 	}
// 	
// 	private void RefreshInventory(Action refreshCallback = null)
// 	{
// 		UserInventory.Instance.UpdateVirtualItems(_ => refreshCallback?.Invoke(), StoreDemoPopup.ShowError);
// 	}
//
// 	private void RefreshVirtualCurrencyBalance()
// 	{
// 		UserInventory.Instance.UpdateVirtualCurrencyBalance(null, StoreDemoPopup.ShowError);
// 	}
//
// 	private void RefreshSubscriptions(Action refreshCallback = null)
// 	{
// 		UserSubscriptions.Instance.UpdateSupscriptions(_ => refreshCallback?.Invoke(), StoreDemoPopup.ShowError);
// 	}
//
// 	private void RefreshUserState()
// 	{
// 		RefreshInventory();
// 		RefreshVirtualCurrencyBalance();
// 		RefreshSubscriptions();
// 	}
// 	
// 	void LockPurchasedNonConsumableItems()
// 	{
// 		if (UserInventory.Instance.IsEmpty()) return;
// 		if (UserCatalog.Instance.IsEmpty()) return;
// 		StartCoroutine(LockCatalogItemsCoroutine());
// 	}
//
// 	IEnumerator LockCatalogItemsCoroutine()
// 	{
// 		yield return new WaitWhile(() =>
// 		{
// 			List<ItemUI> itemsInContainer = new List<ItemUI>();
// 			_itemsController.GetCatalogContainers().ForEach(c => itemsInContainer.AddRange(c.Items));
// 			return !itemsInContainer.Any();
// 		});
// 		List<InventoryItem> inventoryItems = UserInventory.Instance.GetItems();
// 		List<StoreItem> catalogItems = UserCatalog.Instance.GetItems().
// 			Where(i => !i.IsConsumable() && !i.IsSubscription()).
// 			Where(i => inventoryItems.Exists(inv => inv.sku.Equals(i.sku))).ToList();
// 		List<ItemUI> containersItems = new List<ItemUI>();
// 		_itemsController.GetCatalogContainers().ForEach(c => containersItems.AddRange(c.Items));
// 		containersItems = containersItems.Where(i => catalogItems.Exists(cat => cat.sku.Equals(i.GetSku()))).ToList();
// 		containersItems.ForEach(item => item.Lock());
// 	}
//
// 	private void InitStoreUi(List<StoreItem> items)
// 	{	// This line for fastest image loading
// 		items.ForEach(i => ImageLoader.Instance.GetImageAsync(i.image_url, null));
// 		UserCatalog.Instance.UpdateGroups(groups =>
// 		{
// 			_groupsController.CreateGroupsBy(items);
// 			_itemsController.CreateItems(items);
//
// 			_itemsTabControl.Init();
// 			_extraController.Initialize();
//
// 			_groupsController.SelectDefault();
//
// 			RefreshSubscriptions();
// 			RefreshInventory();
//
// 			UserCatalog.Instance.UpdateVirtualCurrencyPackages(
// 				_itemsController.AddVirtualCurrencyPackages, StoreDemoPopup.ShowError);
// 			UserCatalog.Instance.UpdateVirtualCurrencies(_ =>
// 			{
// 				RefreshVirtualCurrencyBalance();
// 				RefreshAttributes();
// 			}, StoreDemoPopup.ShowError);
// 		}, StoreDemoPopup.ShowError);
// 	}
// 	
// 	public void RefreshAttributes(Action refreshCallback = null)
// 	{
// 		XsollaLogin.Instance.GetUserAttributes(XsollaStore.Instance.Token, XsollaSettings.StoreProjectId, null, null, list =>
// 		{
// 			attributes = list;
// 			_extraController.SetAttributes(list);
// 			refreshCallback?.Invoke();
// 		}, StoreDemoPopup.ShowError);
// 	}
// }