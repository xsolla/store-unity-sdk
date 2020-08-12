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
	private bool _balanceUpdated;
	private bool _itemsUpdated;
	private bool _subscriptionsUpdated;

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
		_balanceUpdated = false;
		_itemsUpdated = false;
		_subscriptionsUpdated = false;

		StartCoroutine(WaitItemUpdatingCoroutine(onSuccess));
		
		_demoImplementation.GetVirtualCurrencyBalance(balance =>
		{
			Balance = balance;
			_balanceUpdated = true;
		}, onError);

		_demoImplementation.GetInventoryItems(items =>
		{
			VirtualItems = items.Where(i => !i.IsVirtualCurrency() && !i.IsSubscription()).ToList();
			_itemsUpdated = true;
		}, onError);
		
		_demoImplementation.GetUserSubscriptions(items =>
		{
			Subscriptions = items;
			_subscriptionsUpdated = true;
		}, onError);
	}

	private IEnumerator WaitItemUpdatingCoroutine(Action onSuccess = null)
	{
		yield return new WaitUntil(() => _balanceUpdated && _itemsUpdated && _subscriptionsUpdated);
		AllItems.Clear();
		AllItems.AddRange(VirtualItems);
		AllItems.AddRange(Subscriptions);
		IsUpdated = true;
		UpdateVirtualCurrencyBalanceEvent?.Invoke(Balance);
		UpdateItemsEvent?.Invoke(VirtualItems);
		UpdateSubscriptionsEvent?.Invoke(Subscriptions);
		RefreshEvent?.Invoke();
		onSuccess?.Invoke();
	}
}