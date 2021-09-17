using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Demo
{
	public abstract class BaseBattlePassRewarder : BaseBattlePassSelectedItemSubscriber
	{
		[SerializeField] protected BaseBattlePassUserStatManager UserStatManager = default;
		[Space]
		[SerializeField] private SimpleButton[] CollectButtons = default;
		[SerializeField] private SimpleButton[] CollectAllButtons = default;
		[Space]
		[SerializeField] private BattlePassPopupFactory PopupFactory = default;

		protected BattlePassItemClickEventArgs _selectedItemEventArgs;
		protected BattlePassItemDescription[] _itemsToCollect;

		private void Awake()
		{
			foreach (var button in CollectButtons)
				button.onClick += CollectSelectedItem;

			foreach (var button in CollectAllButtons)
				button.onClick += CollectAllItems;
		}

		public override void OnItemSelected(ItemSelectedEventArgs eventArgs)
		{
			_selectedItemEventArgs = eventArgs.SelectedItemInfo;
			_itemsToCollect = eventArgs.AllItemsInCollectState;
		}

		private void CollectSelectedItem()
		{
			if (_selectedItemEventArgs.ItemState != BattlePassItemState.Collect)
			{
				Debug.LogWarning($"Selected item is in '{_selectedItemEventArgs.ItemState.ToString()}' state. Can not collect it.");
				return;
			}

			StartCoroutine(CollectItems(_selectedItemEventArgs.ItemDescription));
		}

		private void CollectAllItems()
		{
			StartCoroutine(CollectItems(_itemsToCollect));
		}

		private IEnumerator CollectItems(params BattlePassItemDescription[] itemsToCollect)
		{
			var collectedFreeItemsTiers = new List<int>();
			var collectedPremiumItemsTiers = new List<int>();

			var collectedItemsDescriptions = new List<BattlePassItemDescription>();

			foreach (var item in itemsToCollect)
			{
				var itemBusy = true;

				var onSuccess = new Action( () =>
				{
					itemBusy = false;

					if (item.IsPremium)
						collectedPremiumItemsTiers.Add(item.Tier);
					else
						collectedFreeItemsTiers.Add(item.Tier);

					collectedItemsDescriptions.Add(item);
				});

				var onError = new Action( () =>
				{
					itemBusy = false;
					var isPremiumFlag = item.IsPremium ? "premium" : "regular";
					Debug.LogError($"Could not collect {isPremiumFlag} item; Tier: '{item.Tier}'; SKU: '{item.Sku}'");
				});

				CollectReward(item, onSuccess, onError);
				yield return new WaitWhile(() => itemBusy);
			}

			UserInventory.Instance.Refresh(onError: StoreDemoPopup.ShowError);
			UserStatManager.AddObtainedItems(collectedFreeItemsTiers.ToArray(), collectedPremiumItemsTiers.ToArray());
			PopupFactory.CreateRewardsPopup(collectedItemsDescriptions.ToArray());
		}

		public abstract void CollectReward(BattlePassItemDescription itemDescription, Action onSuccess, Action onError);
	}
}
