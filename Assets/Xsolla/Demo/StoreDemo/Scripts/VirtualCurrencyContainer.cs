using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Store;

public class VirtualCurrencyContainer : MonoBehaviour
{
	public GameObject VirtualCurrencyBalancePrefab;
	private Dictionary<string, VirtualCurrencyBalanceUI> _currencies;

	private void Awake()
	{
		if(VirtualCurrencyBalancePrefab == null) {
			Debug.LogAssertion("VirtualCurrencyBalancePrefab is missing!");
			Destroy(gameObject);
			return;
		}
		_currencies = new Dictionary<string, VirtualCurrencyBalanceUI>();
	}

	private void AddCurrency(StoreItem item)
	{
		if (_currencies.ContainsKey(item.sku) || string.IsNullOrEmpty(item.image_url)) return;
		GameObject currencyBalance = Instantiate(VirtualCurrencyBalancePrefab, transform);
		VirtualCurrencyBalanceUI balanceUi = currencyBalance.GetComponent<VirtualCurrencyBalanceUI>();
		balanceUi.Initialize(item);
		_currencies.Add(item.sku, balanceUi);
	}
	
	public void SetCurrencies(List<VirtualCurrencyItem> items)
	{
		_currencies.Values.ToList().ForEach(c =>
		{
			if(c.gameObject != null)
				Destroy(c.gameObject);
		});
		_currencies.Clear();
		items.ForEach(AddCurrency);
	}
	
	public void SetCurrenciesBalance(List<VirtualCurrencyBalance> balance)
	{
		balance.ForEach(SetCurrencyBalance);
	}
	
	private void SetCurrencyBalance(VirtualCurrencyBalance balance)
	{
		if (_currencies.ContainsKey(balance.sku)) {
			_currencies[balance.sku].SetBalance(balance.amount);
		}
	}
}
