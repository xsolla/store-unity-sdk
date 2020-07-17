using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VirtualCurrencyContainer : MonoBehaviour
{
	public GameObject virtualCurrencyBalancePrefab;

	private readonly Dictionary<string, VirtualCurrencyBalanceUI> _currencies =
		new Dictionary<string, VirtualCurrencyBalanceUI>();

	private void Awake()
	{
		if (virtualCurrencyBalancePrefab != null) return;
		Debug.LogAssertion("VirtualCurrencyBalancePrefab is missing!");
		Destroy(gameObject);
	}

	private void Start()
	{
		if (UserCatalog.Instance.IsUpdated && UserInventory.Instance.IsUpdated)
		{
			SetCurrencies(UserCatalog.Instance.Currencies);
			SetCurrenciesBalance(UserInventory.Instance.Balance);
		}
		UserCatalog.Instance.UpdateVirtualCurrenciesEvent += SetCurrencies;
		UserInventory.Instance.UpdateVirtualCurrencyBalanceEvent += SetCurrenciesBalance;
	}

	private void OnDestroy()
	{
		if(UserCatalog.IsExist)
			UserCatalog.Instance.UpdateVirtualCurrenciesEvent -= SetCurrencies;
		if(UserInventory.IsExist)
			UserInventory.Instance.UpdateVirtualCurrencyBalanceEvent -= SetCurrenciesBalance;
	}

	private void SetCurrencies(List<CatalogVirtualCurrencyModel> items)
	{
		_currencies.Values.ToList().ForEach(c =>
		{
			if (c.gameObject != null)
				Destroy(c.gameObject);
		});
		var uniqueItems = new List<CatalogVirtualCurrencyModel>();
		items.ForEach(i =>
		{
			if (uniqueItems.Count(u => u.CurrencySku.Equals(i.CurrencySku)) == 0)
				uniqueItems.Add(i);
		});
		_currencies.Clear();
		uniqueItems.ForEach(i => AddCurrency(i));
	}

	private VirtualCurrencyBalanceUI AddCurrency(CatalogVirtualCurrencyModel item)
	{
		if (item == null) return null;
		if (_currencies.ContainsKey(item.CurrencySku)) return _currencies[item.CurrencySku];
		if (string.IsNullOrEmpty(item.ImageUrl)) return null;
		var currencyBalance = Instantiate(virtualCurrencyBalancePrefab, transform);
		var balanceUi = currencyBalance.GetComponent<VirtualCurrencyBalanceUI>();
		balanceUi.Initialize(item);
		_currencies.Add(item.CurrencySku, balanceUi);
		return balanceUi;
	}

	private void SetCurrenciesBalance(List<VirtualCurrencyBalanceModel> balance)
	{
		balance.ForEach(SetCurrencyBalance);
	}

	private void SetCurrencyBalance(VirtualCurrencyBalanceModel balance)
	{
		AddCurrency(new CatalogVirtualCurrencyModel
		{
			Sku = balance.Sku,
			CurrencySku = balance.Sku,
			ImageUrl = balance.ImageUrl,
			Amount = balance.Amount
		})?.SetBalance(balance.Amount);
	}
}