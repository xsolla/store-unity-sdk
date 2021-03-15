using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;

namespace Xsolla.Demo
{
	public class BattlePassLevelUpController : MonoBehaviour
    {
		[SerializeField] private SimpleButton LevelUpButton = default;
		[SerializeField] private BattlePassLevelUpDataProvider LevelUpDataProvider = default;
		[SerializeField] private BattlePassItemsManager ItemsManager = default;
		[SerializeField] private BattlePassPopupController PopupController = default;
		[SerializeField] private BaseBattlePassUserStatManager UserStatManager = default;

		private int _battlePassLevels;
		private int _userLevel;
		private CatalogItemModel _levelUpUtil;
		private LevelUpPriceData _priceData;
		private int _levelDelta;
		private bool _waitingPopupNeeded = false;

		private void Awake()
		{
			LevelUpButton.onClick += ShowLevelUpDialog;
			LevelUpDataProvider.LevelUpUtilArrived += OnLevelUpUtilArrived;
		}

		public void SetBattlePassLevels(int levels) => _battlePassLevels = levels;
		public void SetUserLevel(int userLevel) => _userLevel = userLevel;

		private void ShowLevelUpDialog()
		{
			if (_battlePassLevels == default(int))
			{
				Debug.LogWarning("BattlePass levels were not set");
				return;
			}

			if (_userLevel == default(int))
			{
				Debug.LogWarning("User level was not set");
				return;
			}

			var levelUpPopup = PopupController.ShowLevelUp();
			levelUpPopup.UserInput += levelDelta => OnUserInput(levelUpPopup, levelDelta);
			levelUpPopup.BuyButtonClick += OnBuyButton;
			levelUpPopup.Initialize(_battlePassLevels - _userLevel);
		}

		private void OnLevelUpUtilArrived(CatalogItemModel levelUpUtil)
		{
			_levelUpUtil = levelUpUtil;
			_priceData = new LevelUpPriceDataExtractor().ExtractPriceData(levelUpUtil);
		}

		private void OnUserInput(BattlePassLevelUpPopup levelUpPopup, int levelDelta)
		{
			_levelDelta = levelDelta;
			var itemsToShow = ItemsManager.GetFutureLockedItems(levelDelta);
			levelUpPopup.ShowItems(itemsToShow);

			if (_priceData == null)
			{
				Debug.LogError("Price data is missing");
				return;
			}

			if (_priceData is LevelUpRealPriceData realPriceData)
			{
				var price = realPriceData.price * (float)levelDelta;
				var formattedPrice = PriceFormatter.FormatPrice(realPriceData.currency, price);
				levelUpPopup.ShowPrice(formattedPrice);
			}
			else if (_priceData is LevelUpVirtualPriceData virtualPriceData)
			{
				var price = virtualPriceData.price * levelDelta;
				var userCurrencyValue = GetUserCurrencyValue(virtualPriceData.currencySku);

				levelUpPopup.ShowPrice(virtualPriceData.currencyImageUrl, price, userCurrencyValue);
			}
		}

		private int GetUserCurrencyValue(string targetSku)
		{
			var allUserCurrencies = UserInventory.Instance.Balance;
			var targetCurrency = default(VirtualCurrencyBalanceModel);

			foreach (var currency in allUserCurrencies)
			{
				if (currency.Sku == targetSku)
				{
					targetCurrency = currency;
					break;
				}
			}

			if (targetCurrency == null)//User balance does not contain defined currency, is OK if this is a new user who did not purchase VC yet
				return default(int);
			else
				return (int)targetCurrency.Amount;
		}

		private void OnBuyButton()
		{
			if (_levelDelta <= 0)
			{
				Debug.LogWarning($"Attempt to buy zero or negative levels, quantity was: '{_levelDelta}'");
				return;
			}

			_waitingPopupNeeded = true;
			PopupFactory.Instance.CreateWaiting().SetCloseCondition( () => !_waitingPopupNeeded);

			var onSuccess = new Action<CatalogItemModel>( returnedItemModel =>
			{
				ConsumeLevelUp(returnedItemModel, _levelDelta, UpUserLevel, error => StoreDemoPopup.ShowError(error));
				_waitingPopupNeeded = false;
			});

			var onError = new Action<Error>( error =>
			{
				if (error != null)
					Debug.LogError(error.errorMessage);

				_waitingPopupNeeded = false;
			});

			if (_priceData is LevelUpRealPriceData)
				PurchaseForRealMoney(_levelUpUtil, _levelDelta, onSuccess, onError);
			else if (_priceData is LevelUpVirtualPriceData)
				StartCoroutine(PurchaseForVirtualCurrency(_levelUpUtil, _levelDelta, onSuccess, onError));
		}

		private void PurchaseForRealMoney(CatalogItemModel itemModel, int quantity, Action<CatalogItemModel> onSuccess, Action<Error> onError)
		{
			var cartItem = new UserCartItem(itemModel);
			cartItem.Quantity = quantity;

			var onCartSuccess = new Action<List<UserCartItem>>( _ =>
			{
				onSuccess?.Invoke(itemModel);
			});

			_waitingPopupNeeded = false;
			DemoController.Instance.StoreDemo.PurchaseCart(new List<UserCartItem>{ cartItem }, onCartSuccess, onError, isSetPreviousDemoState: false);
		}

		private IEnumerator PurchaseForVirtualCurrency(CatalogItemModel itemModel, int quantity, Action<CatalogItemModel> onSuccess, Action<Error> onError)
		{
			var backwardCounter = quantity;
			var successCounter = 0;

			var onPurchaseSuccess = new Action<CatalogItemModel>( _ =>
			{
				successCounter++;
				backwardCounter--;
			});

			var onPurchaseError = new Action<Error>( error =>
			{
				onError?.Invoke(error);
				backwardCounter--;
			});

			for (int i = 0; i < quantity; i++)
				DemoController.Instance.StoreDemo.PurchaseForVirtualCurrency(itemModel, onPurchaseSuccess, onPurchaseError, isConfirmationRequired: false, isShowResultToUser: false);

			yield return new WaitWhile( () => backwardCounter > 0);

			if (successCounter > 0)
				onSuccess?.Invoke(itemModel);
		}

		private void ConsumeLevelUp(CatalogItemModel levelUpItem, int count, Action<int> onSuccess, Action<Error> onError)
		{
			var onRefreshSuccess = new Action( () =>
			{
				var inventoryItem = GetInventoryItem(levelUpItem.Sku);
				if (inventoryItem != null)
					ConsumeInventoryItem(inventoryItem, count, onSuccess, onError);
				else
				{
					var errorMessage = $"Could not find inventory item with sku: '{levelUpItem.Sku}'";
					Debug.LogError(errorMessage);
					onError?.Invoke(new Error(errorType: ErrorType.ProductDoesNotExist, errorMessage: errorMessage));
				}
			});

			UserInventory.Instance.Refresh(onRefreshSuccess, onError);
		}

		private InventoryItemModel GetInventoryItem(string itemSku)
		{
			foreach (var item in UserInventory.Instance.VirtualItems)
				if (item.Sku == itemSku)
					return item;
			//else
			return null;
		}

		private void ConsumeInventoryItem(InventoryItemModel item, int count, Action<int> onSuccess, Action<Error> onError)
		{
			var itemRemainingUses = item.RemainingUses.HasValue ? (int)item.RemainingUses.Value : 0;
			
			if (count != itemRemainingUses)
				Debug.LogWarning($"Item remaining uses and consume count supposed to be equal, something went wrong. Remainig uses: '{itemRemainingUses}'. Consume count: '{count}'");

			var consumeCount = count <= itemRemainingUses ? count : itemRemainingUses;

			var onConsumeSuccess = new Action<InventoryItemModel>( _ =>
			{
				onSuccess?.Invoke(consumeCount);
			});

			var onConsumeError = new Action<InventoryItemModel>( _ =>
			{
				onError?.Invoke(new Error(errorMessage: "Could not consume level up"));
			});

			DemoController.Instance.InventoryDemo.ConsumeInventoryItem(item, consumeCount, onConsumeSuccess, onConsumeError, isConfirmationRequired: false);
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
