using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Store;

public partial class DemoImplementation : MonoSingleton<DemoImplementation>, IDemoImplementation
{
	public void GetInventoryItems(Action<List<InventoryItemModel>> onSuccess, Action<Error> onError = null)
	{
		XsollaStore.Instance.GetInventoryItems(XsollaSettings.StoreProjectId, items =>
		{
			var inventoryItems = items.items.Select(i => new InventoryItemModel
			{
				Sku = i.sku,
				Description = i.description,
				Name = i.name,
				ImageUrl = i.image_url,
				IsConsumable = i.IsConsumable(),
				InstanceId = i.instance_id,
				RemainingUses = (uint?)i.quantity
			}).ToList();
			onSuccess?.Invoke(inventoryItems);
		}, WrapErrorCallback(onError));
	}

	public void GetVirtualCurrencyBalance(Action<List<VirtualCurrencyBalanceModel>> onSuccess, Action<Error> onError = null)
	{
		XsollaStore.Instance.GetVirtualCurrencyBalance(XsollaSettings.StoreProjectId, balances =>
		{
			var result = balances.items.ToList().Select(b => new VirtualCurrencyBalanceModel
			{
				Sku = b.sku,
				Description = b.description,
				Name = b.name,
				ImageUrl = b.image_url,
				IsConsumable = false,
				Amount = b.amount
			}).ToList();
			onSuccess?.Invoke(result);
		}, WrapErrorCallback(onError));
	}

	public void ConsumeInventoryItem(InventoryItemModel item, uint count, Action<InventoryItemModel> onSuccess, Action<InventoryItemModel> onFailed = null)
	{
		StartCoroutine(ConsumeCoroutine(item, count, onSuccess, onFailed));
	}

	IEnumerator ConsumeCoroutine(InventoryItemModel item, uint count, Action<InventoryItemModel> onSuccess, Action<InventoryItemModel> onFailed = null)
	{
		while (count > 0)
		{
			var busy = true;
			SendConsumeOneItemRequest(item, () => busy = false, () => WrapErrorCallback(_ => onFailed?.Invoke(item)));
			yield return new WaitWhile(() => busy);
			count--;
		}
		onSuccess?.Invoke(item);
	}

	private void SendConsumeOneItemRequest(InventoryItemModel item, Action onSuccess, Action onError)
	{
		XsollaStore.Instance.ConsumeInventoryItem(XsollaSettings.StoreProjectId, new ConsumeItem
		{
			sku = item.Sku,
			instance_id = item.InstanceId,
			quantity = 1
		}, onSuccess, _ => onError?.Invoke());
	}
}
