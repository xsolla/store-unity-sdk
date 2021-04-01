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
		private int _itemGetAttempts = 0;

		protected override Action<CatalogItemModel> OnSuccessPurchase
			=> (purchasedItem => ConsumeLevelUp(purchasedItem, _levelDelta, UpUserLevel, error => StoreDemoPopup.ShowError(error)));

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
						ConsumeInventoryItem(unusedItem, (int)useCount, UpUserLevel, error => StoreDemoPopup.ShowError(error));
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
			var onItemFound = new Action<InventoryItemModel>( item =>
			{
				var itemRemainingUses = item.RemainingUses.HasValue ? (int)item.RemainingUses.Value : 0;

				if (itemRemainingUses == count)
				{
					_itemGetAttempts = 0;
					ConsumeInventoryItem(item, count, onSuccess, onError);
				}
				else
				{
					_itemGetAttempts++;
					Debug.LogWarning($"Item remaining uses and target consume count supposed to be equal, something went wrong. " +
						$"Remainig uses: '{itemRemainingUses}'. Consume count: '{count}'");

					if (_itemGetAttempts <= BattlePassConstants.MAX_INVENTORY_REFRESH_ATTEMPTS)
					{
						Debug.LogWarning($"Attempt to refresh item data #{_itemGetAttempts}");
						UserInventory.Instance.Refresh();
						ConsumeLevelUp(levelUpItem, count, onSuccess, onError);
					}
					else
					{
						Debug.LogWarning($"Give up and consume what we've got: Remaining uses: {itemRemainingUses}");
						_itemGetAttempts = 0;
						ConsumeInventoryItem(item, itemRemainingUses, onSuccess, onError);
					}
				}
			});

			var onItemAbsence = new Action(() =>
			{
				var errorMessage = $"Could not find inventory item with sku: '{levelUpItem.Sku}'";
				Debug.LogError(errorMessage);
				onError?.Invoke(new Error(errorType: ErrorType.ProductDoesNotExist, errorMessage: errorMessage));
			});

			InventoryFinder.FindInInventory(levelUpItem.Sku, BattlePassConstants.MAX_INVENTORY_REFRESH_ATTEMPTS,
				onItemFound, onItemAbsence);
		}

		private void ConsumeInventoryItem(InventoryItemModel item, int count, Action<int> onSuccess, Action<Error> onError)
		{
			var onConsumeSuccess = new Action<InventoryItemModel>( _ =>
			{
				UserInventory.Instance.Refresh();
				onSuccess?.Invoke(count);
			});

			var onConsumeError = new Action<InventoryItemModel>( _ =>
			{
				UserInventory.Instance.Refresh();
				onError?.Invoke(new Error(errorMessage: "Could not consume level up"));
			});

			DemoController.Instance.InventoryDemo.ConsumeInventoryItem(item, count, onConsumeSuccess, onConsumeError, isConfirmationRequired: false);
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
