using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Store;

public class VirtualCurrencyContainer : MonoBehaviour
{
	public GameObject VirtualCurrencyBalancePrefab;
	private Dictionary<string, VirtualCurrencyBalanceUI> currencies;

	private void Awake()
	{
		currencies = new Dictionary<string, VirtualCurrencyBalanceUI>();
	}

	public void AddCurrency(StoreItem item)
	{
		if(VirtualCurrencyBalancePrefab == null) {
			Debug.LogError("VirtualCurrencyBalancePrefab is missing!");
			return;
		}
		if(!currencies.ContainsKey(item.sku) && item.image_url != null) {
			
			GameObject currencyBalance = Instantiate(VirtualCurrencyBalancePrefab, transform);
			VirtualCurrencyBalanceUI balanceUI = currencyBalance?.GetComponent<VirtualCurrencyBalanceUI>();
			currencies.Add(item.sku, balanceUI);
			balanceUI.Initialize(item);
		}
	}

	public void SetCurrencyBalance(VirtualCurrencyBalance balance)
	{
		if (currencies.ContainsKey(balance.sku)) {
			currencies[balance.sku].SetBalance(balance.amount);
		}
	}

	public void SetCurrenciesBalance(VirtualCurrenciesBalance balance)
	{
		balance.items.ToList().ForEach(SetCurrencyBalance);
	}

	public void SetCurrencies(VirtualCurrencyItems items)
	{
		currencies.Values.ToList().ForEach(c => Destroy(c.gameObject));
		currencies.Clear();
		items.items.ToList().ForEach(AddCurrency);
	}
}
