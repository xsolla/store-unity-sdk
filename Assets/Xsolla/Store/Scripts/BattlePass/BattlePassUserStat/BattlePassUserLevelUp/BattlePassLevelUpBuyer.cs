using System;
using System.Collections;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class BattlePassLevelUpBuyer : BaseBattlePassBuyer
	{
		[SerializeField] private BattlePassLevelUpDataProvider LevelUpDataProvider = default;
		[SerializeField] private BattlePassItemsManager ItemsManager = default;
		[SerializeField] private BaseBattlePassUserStatManager UserStatManager = default;
		[SerializeField] private InventoryFinder InventoryFinder = default;

		private bool _isBatttlePassExpired;
		private int _battlePassLevels;
		private bool _isUserStatArrived;
		private int _userLevel;
		private int _levelDelta;

		protected override Action<CatalogItemModel> OnSuccessPurchase
			=> (purchasedItem => ConsumeLevelUp(purchasedItem, _levelDelta, UpUserLevel, StoreDemoPopup.ShowError));

		public void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			_isBatttlePassExpired = battlePassDescription.IsExpired;
			_battlePassLevels = battlePassDescription.Levels.Length;
		}

		public void OnUserStatArrived(BattlePassUserStat userStat)
		{
			_userLevel = userStat.Level;
			_isUserStatArrived = true;
		}

		protected override void OnAwake()
		{
			LevelUpDataProvider.LevelUpUtilArrived += OnLevelUpUtilArrived;
		}

		protected override bool IsShowDialogValid()
		{
			if (_isBatttlePassExpired)
			{
				Debug.LogWarning("BattlePass expired, level up is not available");
				return false;
			}

			if (_battlePassLevels == default(int))
			{
				Debug.LogWarning("BattlePass levels were not set");
				return false;
			}

			if (_userLevel == default(int))
			{
				Debug.LogWarning("User level was not set");
				return false;
			}
			//else
			return true;
		}

		protected override void OnShowDialog<T>(T dialogController)
		{
			var levelUpPopupController = dialogController as BattlePassLevelUpPopup;
			levelUpPopupController.UserInput += levelDelta => OnUserInput(levelUpPopupController, levelDelta);
			levelUpPopupController.Initialize(_userLevel, _battlePassLevels - _userLevel);
		}

		protected override bool IsBuyValid()
		{
			if (_isBatttlePassExpired)
			{
				Debug.LogWarning("BattlePass expired, level up is not available");
				return false;
			}

			if (_levelDelta <= 0)
			{
				Debug.LogWarning($"Attempt to buy zero or negative levels, quantity was: '{_levelDelta}'");
				return false;
			}
			//else
			return true;
		}

		protected override int GetBuyItemsCount()
		{
			return _levelDelta;
		}

		private void OnLevelUpUtilArrived(CatalogItemModel levelUpUtil)
		{
			base.ItemToBuy = levelUpUtil;
			base.PriceData = new PriceDataExtractor().ExtractPriceData(levelUpUtil);

			//Check if user for some reason already has this item in their inventory
			//That would mean that something went wrong on levelup purchase
			StartCoroutine(CheckForUnusedLevelUps(levelUpUtil.Sku));
		}

		private IEnumerator CheckForUnusedLevelUps(string levelUpSku)
		{
			yield return new WaitWhile(() => !_isUserStatArrived);

			InventoryFinder.FindInInventory(levelUpSku, maxAttempts: 1,
				onFound: unusedItem =>
				{
					var useCount = unusedItem.RemainingUses.HasValue ? unusedItem.RemainingUses.Value : 0;

					if (useCount != 0)
						ConsumeInventoryItem(unusedItem, (int)useCount, UpUserLevel, StoreDemoPopup.ShowError);
					else
						Debug.LogWarning($"Unused item with zero remaining uses");
				});
		}

		private void OnUserInput(BattlePassLevelUpPopup levelUpPopup, int levelDelta)
		{
			_levelDelta = levelDelta;
			var itemsToShow = ItemsManager.GetFutureLockedItems(levelDelta);
			levelUpPopup.ShowItems(itemsToShow);

			if (base.PriceData == null)
			{
				Debug.LogError("Price data is missing");
				return;
			}

			if (base.PriceData is RealPriceData realPriceData)
			{
				var price = realPriceData.price * (float)levelDelta;
				var formattedPrice = PriceFormatter.FormatPrice(realPriceData.currency, price);
				levelUpPopup.ShowPrice(formattedPrice);
			}
			else if (base.PriceData is VirtualPriceData virtualPriceData)
			{
				var price = virtualPriceData.price * levelDelta;
				var userCurrencyValue = base.GetUserCurrencyValue(virtualPriceData.currencySku);
				levelUpPopup.ShowPrice(virtualPriceData.currencyImageUrl, virtualPriceData.currencyName, price, userCurrencyValue);
			}
		}

		private void ConsumeLevelUp(CatalogItemModel levelUpItem, int count, Action<int> onSuccess, Action<Error> onError)
		{
			//We just need to send consume request to backend
			//In case of consume fail, item will be consumed on the next BP page open, see OnLevelUpUtilArrived above
			var itemToConsume = new InventoryItemModel();
			itemToConsume.RemainingUses = (uint)count;
			itemToConsume.Sku = levelUpItem.Sku;
			itemToConsume.IsConsumable = true;
			ConsumeInventoryItem(itemToConsume, count, onSuccess, onError);
		}

		private void ConsumeInventoryItem(InventoryItemModel item, int count, Action<int> onSuccess, Action<Error> onError)
		{
			var onConsumeSuccess = new Action<InventoryItemModel>( _ =>
			{
				UserInventory.Instance.Refresh(onError: StoreDemoPopup.ShowError);
				onSuccess?.Invoke(count);
			});

			var onConsumeError = new Action<Error>( error =>
			{
				UserInventory.Instance.Refresh(onError: StoreDemoPopup.ShowError);
				onError?.Invoke(new Error(errorMessage: "Could not consume level up"));
			});

			DemoInventory.Instance.ConsumeInventoryItem(item, count, onConsumeSuccess, onConsumeError, isConfirmationRequired: false);
		}

		private void UpUserLevel(int levels)
		{
			if (levels <= 0)
			{
				Debug.LogWarning($"Attmept to up user level with levels value: '{levels}'");
				return;
			}

			UserStatManager.AddLevel(levels);
		}
	}
}
