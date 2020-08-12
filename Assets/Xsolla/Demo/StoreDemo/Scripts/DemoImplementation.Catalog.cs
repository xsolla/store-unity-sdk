using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Store;

public partial class DemoImplementation : MonoBehaviour, IDemoImplementation
{
	private readonly Dictionary<string, List<string>> _itemsGroups = new Dictionary<string, List<string>>();
	private List<StoreItem> _itemsCache;
	private DateTime _cacheTime = DateTime.Now;
	private bool _inProgress;

	private void RequestStoreItems(Action<List<StoreItem>> onSuccess, Action<Error> onError = null)
	{
		if (_itemsCache == null || (DateTime.Now - _cacheTime).TotalSeconds > 10)
		{
			if (!_inProgress)
			{
				_inProgress = true;
				XsollaStore.Instance.GetCatalog(XsollaSettings.StoreProjectId, items =>
				{
					_cacheTime = DateTime.Now;
					_itemsCache = items.items.ToList();
					onSuccess?.Invoke(_itemsCache);
					_inProgress = false;
				}, WrapErrorCallback(onError));	
			}
			else
				StartCoroutine(WaitItemsCoroutine(onSuccess));
		}
		else
			onSuccess?.Invoke(_itemsCache);
	}

	private IEnumerator WaitItemsCoroutine(Action<List<StoreItem>> onSuccess)
	{
		yield return new WaitWhile(() => _inProgress);
		onSuccess?.Invoke(_itemsCache);
	}
	
	public void GetVirtualCurrencies(Action<List<VirtualCurrencyModel>> onSuccess, Action<Error> onError = null)
	{
		XsollaStore.Instance.GetVirtualCurrencyList(XsollaSettings.StoreProjectId, items =>
		{
			var currencies = items.items.ToList();
			if (currencies.Any())
			{
				var result = currencies.Select(c =>
				{
					var model = new VirtualCurrencyModel();
					FillItemModel(model, c);
					return model;
				}).ToList();
				onSuccess?.Invoke(result);
			}
			else onSuccess?.Invoke(new List<VirtualCurrencyModel>());
		}, WrapErrorCallback(onError));
	}

	public void GetCatalogVirtualItems(Action<List<CatalogVirtualItemModel>> onSuccess, Action<Error> onError = null)
	{
		RequestStoreItems(items =>
		{
			var virtualItems = new List<CatalogVirtualItemModel>();
			items.Where(i => !i.IsSubscription()).ToList().ForEach(i =>
			{
				virtualItems.Add(new CatalogVirtualItemModel
				{
					IsConsumable = i.IsConsumable()
				});
				FillCatalogItem(virtualItems.Last(), i);
				AddItemGroups(i);
			});
			onSuccess?.Invoke(virtualItems);
		}, WrapErrorCallback(onError));
	}
	
	public void GetCatalogVirtualCurrencyPackages(Action<List<CatalogVirtualCurrencyModel>> onSuccess, Action<Error> onError = null)
	{
		XsollaStore.Instance.GetVirtualCurrencyPackagesList(XsollaSettings.StoreProjectId, packages =>
		{
			var currencies = new List<CatalogVirtualCurrencyModel>();
			packages.items.ForEach(p =>
			{
				currencies.Add(new CatalogVirtualCurrencyModel
				{
					IsConsumable = p.IsConsumable(),
					Amount = (uint)p.content.First().quantity,
					CurrencySku = p.content.First().sku
				});
				FillCatalogItem(currencies.Last(), p);
				AddItemGroups(p);
			});
			onSuccess?.Invoke(currencies);
		}, WrapErrorCallback(onError));
	}
	
	public void GetCatalogSubscriptions(Action<List<CatalogSubscriptionItemModel>> onSuccess, Action<Error> onError = null)
	{
		RequestStoreItems(items =>
		{
			var subscriptionItems = new List<CatalogSubscriptionItemModel>();
			items.Where(i => i.IsSubscription()).ToList().ForEach(i =>
			{
				subscriptionItems.Add(new CatalogSubscriptionItemModel
				{
					IsConsumable = false,
					ExpirationPeriod = i.inventory_options.expiration_period.ToTimeSpan(),
					ExpirationPeriodText = i.inventory_options.expiration_period.ToString()
				});
				FillCatalogItem(subscriptionItems.Last(), i);
				AddItemGroups(i);
			});
			onSuccess?.Invoke(subscriptionItems);
		}, WrapErrorCallback(onError));
	}

	private void AddItemGroups(StoreItem item)
	{
		var groups = item.groups.Select(g => g.name).ToList();
		if(!_itemsGroups.ContainsKey(item.sku))
			_itemsGroups.Add(item.sku, new List<string>());
		else
			groups = groups.Except(_itemsGroups[item.sku]).ToList();
		if(groups.Any())
			_itemsGroups[item.sku].AddRange(groups);
	}

	private static void FillItemModel(ItemModel model, StoreItem item)
	{
		model.Sku = item.sku;
		model.Name = item.name;
		model.Description = item.description;
		model.ImageUrl = item.image_url;
	}
	
	private static void FillCatalogItem(CatalogItemModel model, StoreItem item)
	{
		FillItemModel(model, item);
		model.RealPrice = GetRealPrice(item);
		model.VirtualPrice = GetVirtualPrice(item);
	}

	private static KeyValuePair<string, float>? GetRealPrice(StoreItem item)
	{
		if (item.price == null) return null;
		return new KeyValuePair<string, float>(item.price.currency, item.price.GetAmount());
	}
	
	private static KeyValuePair<string, uint>? GetVirtualPrice(StoreItem item)
	{
		var virtualPrices = item.virtual_prices.ToList();
		if (!virtualPrices.Any()) return null;
		var virtualPrice = virtualPrices.Count(v => v.is_default) == 0
			? virtualPrices.First()
			: virtualPrices.First(v => v.is_default); 
		return  new KeyValuePair<string, uint>(virtualPrice.sku, virtualPrice.GetAmount());
	}
	
	public List<string> GetCatalogGroupsByItem(CatalogItemModel item)
	{
		return _itemsGroups.ContainsKey(item.Sku) ? _itemsGroups[item.Sku] : new List<string>();
	}
}
