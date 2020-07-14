using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xsolla.Core;

public class UserInventory : MonoSingleton<UserInventory>
{
	public event Action<List<InventoryItemModel>> UpdateItemsEvent;
	public event Action<List<VirtualCurrencyBalanceModel>> UpdateVirtualCurrencyBalanceEvent;

	public List<InventoryItemModel> Items { get; private set; }
	public List<VirtualCurrencyBalanceModel> Balance { get; private set; }

	private IDemoImplementation _demoImplementation;

	public void Init(IDemoImplementation demoImplementation)
	{
		_demoImplementation = demoImplementation;
		Items = new List<InventoryItemModel>();
		Balance = new List<VirtualCurrencyBalanceModel>();
	}

	public void Refresh([CanBeNull] Action<Error> onError = null)
	{
		if (_demoImplementation == null)
		{
			Balance = new List<VirtualCurrencyBalanceModel>();
			UpdateVirtualCurrencyBalanceEvent?.Invoke(Balance);

			Items = new List<InventoryItemModel>();
			UpdateItemsEvent?.Invoke(Items);

			return;
		}

		_demoImplementation.GetVirtualCurrencyBalance(balance =>
		{
			Balance = balance;
			UpdateVirtualCurrencyBalanceEvent?.Invoke(Balance);
		}, onError);

		_demoImplementation.GetInventoryItems(items =>
		{
			Items = items;
			UpdateItemsEvent?.Invoke(Items);
		}, onError);
	}
}