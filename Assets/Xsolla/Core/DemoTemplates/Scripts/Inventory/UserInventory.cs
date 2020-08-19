using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Xsolla.Core;

public class UserInventory : MonoSingleton<UserInventory>
{
	public event Action RefreshEvent;
	public event Action<List<InventoryItemModel>> UpdateItemsEvent;
	public event Action<List<VirtualCurrencyBalanceModel>> UpdateVirtualCurrencyBalanceEvent;
	public event Action<List<UserSubscriptionModel>> UpdateSubscriptionsEvent;

	public List<ItemModel> AllItems { get; private set; }
	public List<InventoryItemModel> VirtualItems { get; private set; }
	public List<VirtualCurrencyBalanceModel> Balance { get; private set; }
	public List<UserSubscriptionModel> Subscriptions { get; private set; }

	public bool IsUpdated { get; private set; }

	private IDemoImplementation _demoImplementation;

	public void Init(IDemoImplementation demoImplementation)
	{
		IsUpdated = false;
		_demoImplementation = demoImplementation;
		AllItems = new List<ItemModel>();
		VirtualItems = new List<InventoryItemModel>();
		Balance = new List<VirtualCurrencyBalanceModel>();
		Subscriptions = new List<UserSubscriptionModel>();
	}

	public void Refresh(Action onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		if (_demoImplementation == null)
		{
			Balance = new List<VirtualCurrencyBalanceModel>();
			UpdateVirtualCurrencyBalanceEvent?.Invoke(Balance);
			VirtualItems = new List<InventoryItemModel>();
			UpdateItemsEvent?.Invoke(VirtualItems);
			Subscriptions = new List<UserSubscriptionModel>();
			UpdateSubscriptionsEvent?.Invoke(Subscriptions);
			return;
		}
		
		IsUpdated = false;
		StartCoroutine(WaitItemUpdatingCoroutine(onSuccess));
	}

	private IEnumerator WaitItemUpdatingCoroutine(Action onSuccess = null, Action<Error> onError = null)
	{
		var balanceUpdated = false;
		var itemsUpdated = false;
		var subscriptionsUpdated = false;

		_demoImplementation.GetVirtualCurrencyBalance(balance =>
		{
			Balance = balance;
			balanceUpdated = true;
		}, onError);

		_demoImplementation.GetInventoryItems(items =>
		{
			VirtualItems = items.Where(i => !i.IsVirtualCurrency() && !i.IsSubscription()).ToList();
			itemsUpdated = true;
		}, onError);
		
		_demoImplementation.GetUserSubscriptions(items =>
		{
			Subscriptions = items;
			subscriptionsUpdated = true;
		}, onError);
		yield return new WaitUntil(() => balanceUpdated && itemsUpdated && subscriptionsUpdated);
		
		IsUpdated = true;
		HandleInventoryUpdate(onSuccess);
	}

	private void HandleInventoryUpdate(Action callback)
	{
		AllItems.Clear();
		AllItems.AddRange(VirtualItems);
		AllItems.AddRange(Subscriptions);
		
		UpdateVirtualCurrencyBalanceEvent?.Invoke(Balance);
		UpdateItemsEvent?.Invoke(VirtualItems);
		UpdateSubscriptionsEvent?.Invoke(Subscriptions);
		RefreshEvent?.Invoke();
		callback?.Invoke();
	}
}