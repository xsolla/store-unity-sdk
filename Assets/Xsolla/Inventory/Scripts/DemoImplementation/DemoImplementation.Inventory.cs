using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;
using Xsolla.Store;

namespace Xsolla.Demo
{
	public partial class DemoImplementation : MonoBehaviour, IInventoryDemoImplementation
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
						RemainingUses = (uint?)i.quantity,
						Attributes = ItemAttributesConverter.ConvertAttributes(i.attributes)
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

		public void ConsumeInventoryItem(InventoryItemModel item, int? count, Action<InventoryItemModel> onSuccess, Action<InventoryItemModel> onFailed = null, bool isConfirmationRequired = true)
		{
			var countToShow = count != null ? (uint)count : 1;
			var onConfirmation = new Action ( () =>
			{
				var isFinished = false;
				PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => isFinished);
				SendConsumeItemRequest(item, count,
					onSuccess: () => { isFinished = true; onSuccess?.Invoke(item); },
					onError: WrapErrorCallback(_ => { isFinished = true; onFailed?.Invoke(item); }));
			});

			if (isConfirmationRequired)
				StoreDemoPopup.ShowConsumeConfirmation(item.Name, countToShow, onConfirmation, () => onFailed?.Invoke(item));
			else
				onConfirmation?.Invoke();
		}

		private void SendConsumeItemRequest(InventoryItemModel item, int? count, Action onSuccess, Action<Error> onError)
		{
			XsollaStore.Instance.ConsumeInventoryItem(XsollaSettings.StoreProjectId, new ConsumeItem
			{
				sku = item.Sku,
				instance_id = item.InstanceId,
				quantity = count
			}, onSuccess, onError);
		}

		public void RedeemCouponCode(string couponCode, Action<List<CouponRedeemedItemModel>> onSuccess, Action<Error> onError)
		{
			var isFinished = false;
			PopupFactory.Instance.CreateWaiting()
				.SetCloseCondition(() => isFinished);

			SendRedeemCouponCodeRequest(couponCode, (redeemedItems) =>
			{
				isFinished = true;
				onSuccess?.Invoke(redeemedItems);
			}, WrapRedeemCouponErrorCallback(error =>
			{
				isFinished = true;
				onError?.Invoke(error);
			}));
		}
	
		private void SendRedeemCouponCodeRequest(string couponCode, Action<List<CouponRedeemedItemModel>> onSuccess, Action<Error> onError)
		{
			XsollaStore.Instance.RedeemCouponCode(XsollaSettings.StoreProjectId, new CouponCode {coupon_code = couponCode}, redeemedItems =>
			{
				var redeemedItemModels = redeemedItems.items.Select(
					i => new CouponRedeemedItemModel 
					{
						Sku = i.sku,
						Description = i.description,
						Name = i.name,
						ImageUrl = i.image_url,
						Quantity = i.quantity,
					}).ToList();
				onSuccess?.Invoke(redeemedItemModels);
			}, onError);
		}
	
		private Action<Error> WrapRedeemCouponErrorCallback(Action<Error> onError)
		{
			return error =>
			{
				if (error.ErrorType != ErrorType.InvalidCoupon)
				{
					StoreDemoPopup.ShowError(error);
				}
				onError?.Invoke(error);
			};
		}
	}
}
