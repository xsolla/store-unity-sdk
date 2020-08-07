using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;
using Xsolla.Store;

public partial class DemoImplementation : MonoBehaviour, IDemoImplementation
{
	public void GetInventoryItems(Action<List<InventoryItemModel>> onSuccess, Action<Error> onError = null)
	{
		XsollaStore.Instance.GetInventoryItems(XsollaSettings.StoreProjectId, items =>
		{
			var inventoryItems = items.items.Where(i => !i.IsVirtualCurrency() && !i.IsSubscription()).Select(
				i => new InventoryItemModel 
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

	public void GetUserSubscriptions(Action<List<UserSubscriptionModel>> onSuccess, Action<Error> onError = null)
	{
		XsollaStore.Instance.GetSubscriptions(XsollaSettings.StoreProjectId, items =>
		{
			var subscriptionItems = items.items.Select(i => new UserSubscriptionModel
			{
				Sku = i.sku,
				Description = i.description,
				Name = i.name,
				ImageUrl = i.image_url,
				IsConsumable = false,
				Status = GetSubscriptionStatus(i.status),
				Expired = i.expired_at.HasValue ? UnixTimeToDateTime(i.expired_at.Value) : (DateTime?) null
			}).ToList();
			onSuccess?.Invoke(subscriptionItems);
		}, WrapErrorCallback(onError));
	}

	private static UserSubscriptionModel.SubscriptionStatusType GetSubscriptionStatus(string status)
	{
		if(string.IsNullOrEmpty(status)) return UserSubscriptionModel.SubscriptionStatusType.None;
		switch (status)
		{
			case "active": return UserSubscriptionModel.SubscriptionStatusType.Active;
			case "expired": return UserSubscriptionModel.SubscriptionStatusType.Expired;
			default: return UserSubscriptionModel.SubscriptionStatusType.None;
		}
	}
	
	private DateTime UnixTimeToDateTime(long unixTime)
	{
		DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		dtDateTime = dtDateTime.AddSeconds(unixTime).ToLocalTime();
		return dtDateTime;
	}

	public void ConsumeInventoryItem(InventoryItemModel item, uint count, Action<InventoryItemModel> onSuccess, Action<InventoryItemModel> onFailed = null)
	{
		StoreDemoPopup.ShowConsumeConfirmation(item.Name, count, () =>
		{
			StartCoroutine(ConsumeCoroutine(item, count, onSuccess, onFailed));
		}, () => onFailed?.Invoke(item));
	}

	IEnumerator ConsumeCoroutine(InventoryItemModel item, uint count, Action<InventoryItemModel> onSuccess, Action<InventoryItemModel> onFailed = null)
	{
		while (count-- > 0)
		{
			var busy = true;
			SendConsumeOneItemRequest(item, () => busy = false, () => WrapErrorCallback(_ => onFailed?.Invoke(item)));
			yield return new WaitWhile(() => busy);
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
