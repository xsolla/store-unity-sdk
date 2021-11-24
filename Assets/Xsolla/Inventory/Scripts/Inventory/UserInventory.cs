using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public partial class UserInventory : MonoSingleton<UserInventory>
	{
		public event Action RefreshEvent;
		public event Action<List<InventoryItemModel>> UpdateItemsEvent;
		public event Action<List<VirtualCurrencyBalanceModel>> UpdateVirtualCurrencyBalanceEvent;
		public event Action<List<UserSubscriptionModel>> UpdateSubscriptionsEvent;

		public List<ItemModel> AllItems { get; private set; }
		public List<InventoryItemModel> VirtualItems { get; private set; }
		public List<VirtualCurrencyBalanceModel> Balance { get; private set; }
		public List<UserSubscriptionModel> Subscriptions { get; private set; }

		public bool HasVirtualItems
		{
			get
			{
				return VirtualItems.Any();
			}
		}
		public bool HasPurchasedSubscriptions
		{
			get
			{
				return Subscriptions.FindAll(s => s.Status != UserSubscriptionModel.SubscriptionStatusType.None).Any();
			}
		}

		public override void Init()
		{
			base.Init();
			IsUpdated = false;
			AllItems = new List<ItemModel>();
			VirtualItems = new List<InventoryItemModel>();
			Balance = new List<VirtualCurrencyBalanceModel>();
			Subscriptions = new List<UserSubscriptionModel>();
		}

		partial void RefreshInventory(Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			IsUpdated = false;
			StartCoroutine(WaitItemUpdatingCoroutine(onSuccess, onError));
		}

		private IEnumerator WaitItemUpdatingCoroutine(Action onSuccess = null, Action<Error> onError = null)
		{
			var balanceUpdated = false;
			var itemsUpdated = false;
			var subscriptionsUpdated = false;

			SdkInventoryLogic.Instance.GetVirtualCurrencyBalance(balance =>
			{
				Balance = balance;
				balanceUpdated = true;
			}, onError);

			SdkInventoryLogic.Instance.GetInventoryItems(items =>
			{
				VirtualItems = items.Where(i => !i.IsVirtualCurrency() && !i.IsSubscription()).ToList();
				itemsUpdated = true;
			}, onError);

			SdkInventoryLogic.Instance.GetUserSubscriptions(items =>
			{
				Subscriptions = items;
				subscriptionsUpdated = true;
			}, onError);
			yield return new WaitUntil(() => balanceUpdated && itemsUpdated && subscriptionsUpdated);

			IsUpdated = true;
			HandleInventoryUpdate(onSuccess);
		}

		private void HandleInventoryUpdate(Action callback)
		{
			AllItems.Clear();
			AllItems.AddRange(VirtualItems.Cast<ItemModel>());
			AllItems.AddRange(Subscriptions.Cast<ItemModel>());

			if (UpdateVirtualCurrencyBalanceEvent != null)
				UpdateVirtualCurrencyBalanceEvent.Invoke(Balance);

			if (UpdateItemsEvent != null)
				UpdateItemsEvent.Invoke(VirtualItems);

			if (UpdateSubscriptionsEvent != null)
				UpdateSubscriptionsEvent.Invoke(Subscriptions);

			if (RefreshEvent != null)
				RefreshEvent.Invoke();

			if (callback != null)
				callback.Invoke();
		}
	}
}
