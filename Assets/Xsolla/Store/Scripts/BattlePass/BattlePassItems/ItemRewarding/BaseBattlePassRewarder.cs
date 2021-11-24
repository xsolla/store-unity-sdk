using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Demo
{
	public abstract class BaseBattlePassRewarder : BaseBattlePassSelectedItemSubscriber
	{
		[SerializeField] protected BaseBattlePassUserStatManager UserStatManager;
		[Space]
		[SerializeField] private SimpleButton[] CollectButtons;
		[SerializeField] private SimpleButton[] CollectAllButtons;
		[Space]
		[SerializeField] private BattlePassPopupFactory PopupFactory;

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
				Debug.LogWarning(string.Format("Selected item is in '{0}' state. Can not collect it.", _selectedItemEventArgs.ItemState.ToString()));
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
					Debug.LogError(string.Format("Could not collect {0} item; Tier: '{1}'; SKU: '{2}'", isPremiumFlag, item.Tier, item.Sku));
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
