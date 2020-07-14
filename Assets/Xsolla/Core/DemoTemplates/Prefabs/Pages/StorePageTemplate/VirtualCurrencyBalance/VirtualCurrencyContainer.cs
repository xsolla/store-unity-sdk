using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VirtualCurrencyContainer : MonoBehaviour
{
	public GameObject virtualCurrencyBalancePrefab;

	private Dictionary<string, VirtualCurrencyBalanceUI> _currencies =
		new Dictionary<string, VirtualCurrencyBalanceUI>();

	private void Awake()
	{
		if (virtualCurrencyBalancePrefab == null)
		{
			Debug.LogAssertion("VirtualCurrencyBalancePrefab is missing!");
			Destroy(gameObject);
		}
	}

	public void SetCurrencies(List<CatalogVirtualCurrencyModel> items)
	{
		_currencies.Values.ToList().ForEach(c =>
		{
			if (c.gameObject != null)
				Destroy(c.gameObject);
		});
		List<CatalogVirtualCurrencyModel> uniqueItems = new List<CatalogVirtualCurrencyModel>();
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
		if (_currencies.ContainsKey(item.CurrencySku)) return _currencies[item.CurrencySku];
		if (string.IsNullOrEmpty(item.ImageUrl)) return null;
		GameObject currencyBalance = Instantiate(virtualCurrencyBalancePrefab, transform);
		VirtualCurrencyBalanceUI balanceUi = currencyBalance.GetComponent<VirtualCurrencyBalanceUI>();
		balanceUi.Initialize(item);
		_currencies.Add(item.CurrencySku, balanceUi);
		return balanceUi;
	}

	public void SetCurrenciesBalance(List<VirtualCurrencyBalanceModel> balance)
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