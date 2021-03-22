using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
    public abstract class BaseBattlePassBuyer : MonoBehaviour
    {
		[SerializeField] private SimpleButton[] BuyButtons = default;
		[SerializeField] private BattlePassPopupFactory PopupFactory = default;

		private bool _waitingPopupNeeded = false;

		protected CatalogItemModel ItemToBuy { get; set; }
		protected PriceData PriceData { get; set; }

		protected abstract Action<CatalogItemModel> OnSuccessPurchase { get; }

		private void Awake()
		{
			foreach (var buyButton in BuyButtons)
				buyButton.onClick += ShowBuyDialog;

			OnAwake();
		}

		protected abstract void OnAwake();
		protected abstract bool IsShowDialogValid();
		protected abstract void OnShowDialog<T>(T dialogController) where T : BaseBattlePassBuyPopup;
		protected abstract bool IsBuyValid();
		protected abstract int GetBuyItemsCount();

		protected int GetUserCurrencyValue(string targetSku)
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

			//User balance does not contain defined currency
			//is OK if this is a new user who did not purchase VC yet
			if (targetCurrency == null)
				return default(int);
			else
				return (int)targetCurrency.Amount;
		}

		private void ShowBuyDialog()
		{
			if (!IsShowDialogValid())
				return;

			var dialog = default(BaseBattlePassBuyPopup);

			if (this is BattlePassLevelUpBuyer)
				dialog = PopupFactory.CreateLevelUpPopup();
			else if (this is BattlePassPremiumBuyer)
				dialog = PopupFactory.CreateBuyPremiumPopup();
			else
			{
				Debug.LogError($"Unexpected buyer type: '{this.GetType()}'");
				return;
			}

			dialog.BuyButtonClick += OnBuyButton;
			OnShowDialog(dialog);
		}

		private void OnBuyButton()
		{
			if (!IsBuyValid())
				return;

			if (ItemToBuy == null)
			{
				Debug.LogError("Item to buy is missing");
				return;
			}

			if (PriceData == null)
			{
				Debug.LogError("Price data is missing");
				return;
			}

			_waitingPopupNeeded = true;
			Core.Popup.PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => !_waitingPopupNeeded);

			var itemsCount = GetBuyItemsCount();

			var onSuccessPurchase = new Action<CatalogItemModel>(returnedItemModel =>
			{
				OnSuccessPurchase?.Invoke(returnedItemModel);
				_waitingPopupNeeded = false;
			});

			var onError = new Action<Error>(error =>
			{
				if (error != null)
					Debug.LogError(error.errorMessage);

				_waitingPopupNeeded = false;
			});

			if (PriceData is RealPriceData)
				PurchaseForRealMoney(ItemToBuy, itemsCount, onSuccessPurchase, onError);
			else if (PriceData is VirtualPriceData)
				StartCoroutine(PurchaseForVirtualCurrency(ItemToBuy, itemsCount, onSuccessPurchase, onError));
		}

		private void PurchaseForRealMoney(CatalogItemModel itemModel, int quantity, Action<CatalogItemModel> onSuccess, Action<Error> onError)
		{
			var cartItem = new UserCartItem(itemModel);
			cartItem.Quantity = quantity;

			var onCartSuccess = new Action<List<UserCartItem>>(_ =>
			{
				onSuccess?.Invoke(itemModel);
			});

			_waitingPopupNeeded = false;
			DemoController.Instance.StoreDemo.PurchaseCart(new List<UserCartItem> { cartItem }, onCartSuccess, onError, isSetPreviousDemoState: false, isShowResultToUser: false);
		}

		private IEnumerator PurchaseForVirtualCurrency(CatalogItemModel itemModel, int quantity, Action<CatalogItemModel> onSuccess, Action<Error> onError)
		{
			var backwardCounter = quantity;
			var successCounter = 0;

			var onPurchaseSuccess = new Action<CatalogItemModel>(_ =>
			{
				successCounter++;
				backwardCounter--;
			});

			var onPurchaseError = new Action<Error>(error =>
			{
				onError?.Invoke(error);
				backwardCounter--;
			});

			for (int i = 0; i < quantity; i++)
				DemoController.Instance.StoreDemo.PurchaseForVirtualCurrency(itemModel, onPurchaseSuccess, onPurchaseError, isConfirmationRequired: false, isShowResultToUser: false);

			yield return new WaitWhile(() => backwardCounter > 0);

			if (successCounter > 0)
				onSuccess?.Invoke(itemModel);
		}
	}
}
