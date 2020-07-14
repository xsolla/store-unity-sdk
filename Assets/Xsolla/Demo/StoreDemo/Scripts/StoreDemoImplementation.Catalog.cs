using System;
using System.Collections.Generic;
using System.Linq;
using Xsolla.Core;
using Xsolla.Store;

public partial class DemoImplementation : MonoSingleton<DemoImplementation>, IDemoImplementation
{
	private readonly Dictionary<string, List<string>> _itemsGroups = new Dictionary<string, List<string>>();
	
	public void GetCatalogVirtualItems(Action<List<CatalogVirtualItemModel>> onSuccess, Action<Error> onError = null)
	{
		XsollaStore.Instance.GetCatalog(XsollaSettings.StoreProjectId, items =>
		{
			var virtualItems = new List<CatalogVirtualItemModel>();
			items.items.ToList().ForEach(i =>
			{
				virtualItems.Add(new CatalogVirtualItemModel
				{
					Sku = i.sku,
					Description = i.description,
					ImageUrl = i.image_url,
					IsConsumable = i.IsConsumable(),
					RealPrice = GetRealPrice(i),
					VirtualPrice = GetVirtualPrice(i)
				});
				_itemsGroups.Add(i.sku, i.groups.Select(g => g.name).ToList());
			});
			onSuccess?.Invoke(virtualItems);
		}, WrapErrorCallback(onError));
	}

	private KeyValuePair<string, float>? GetRealPrice(StoreItem item)
	{
		if (item.price == null) return null;
		return new KeyValuePair<string, float>(item.price.currency, item.price.GetAmount());
	}
	
	private KeyValuePair<string, uint>? GetVirtualPrice(StoreItem item)
	{
		var virtualPrices = item.virtual_prices.ToList();
		if (!virtualPrices.Any()) return null;
		var virtualPrice = virtualPrices.Count(v => v.is_default) == 0
			? virtualPrices.First()
			: virtualPrices.First(v => v.is_default); 
		return  new KeyValuePair<string, uint>(virtualPrice.name, virtualPrice.GetAmount());
	}

	public void GetCatalogVirtualCurrencies(Action<List<CatalogVirtualCurrencyModel>> onSuccess, Action<Error> onError = null)
	{
		XsollaStore.Instance.GetVirtualCurrencyPackagesList(XsollaSettings.StoreProjectId, packages =>
		{
			var currencies = new List<CatalogVirtualCurrencyModel>();
			packages.items.ForEach(p =>
			{
				currencies.Add(new CatalogVirtualCurrencyModel
				{
					Sku = p.sku,
					Description = p.description,
					ImageUrl = p.image_url,
					IsConsumable = p.IsConsumable(),
					RealPrice = GetRealPrice(p),
					VirtualPrice = GetVirtualPrice(p),
					Amount = (uint)p.content.First().quantity,
					CurrencySku = p.content.First().sku
				});
				_itemsGroups.Add(p.sku, p.groups.Select(g => g.name).ToList());
			});
			onSuccess?.Invoke(currencies);
		}, WrapErrorCallback(onError));
	}

	public List<string> GetCatalogGroupsByItem(CatalogItemModel item)
	{
		return _itemsGroups.ContainsKey(item.Sku) ? _itemsGroups[item.Sku] : new List<string>();
	}
}
