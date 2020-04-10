using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Store;

public class CartControls : MonoBehaviour
{
	[SerializeField]
	SimpleTextButton buyButton;
	[SerializeField]
	SimpleButton clearCartButton;
	[SerializeField]
	Text priceText;
	[SerializeField]
	GameObject LoaderPrefab;
	GameObject LoaderObject;

	int _requestsLeft;
	
	StoreController _storeController;

	public Action OnClearCart {
		set {
			clearCartButton.onClick = value;
		}
	}

	void Awake()
	{
		_storeController = FindObjectOfType<StoreController>();

		buyButton.onClick = (() =>
		{
			if (!buyButton.IsLocked()) {
				LockBuyButton();
				XsollaStore.Instance.ClearCart(XsollaSettings.StoreProjectId, _storeController.Cart.cart_id, () =>
				{
					_requestsLeft = _storeController.CartModel.CartItems.Count;

					foreach (var cartItem in _storeController.CartModel.CartItems) {
						XsollaStore.Instance.UpdateItemInCart(XsollaSettings.StoreProjectId, _storeController.Cart.cart_id, cartItem.Key, cartItem.Value.Quantity,
							() => {
								_requestsLeft--;
							}, error => {
								_requestsLeft--;
								_storeController.ShowError(error);
							});
					}

					StartCoroutine(BuyCartCoroutine());
				}, _storeController.ShowError);
			}
		});
	}

	IEnumerator BuyCartCoroutine()
	{
		yield return new WaitWhile(() => _requestsLeft > 0);

		XsollaStore.Instance.CartPurchase(XsollaSettings.StoreProjectId, _storeController.Cart.cart_id, data =>
		{
			XsollaStore.Instance.OpenPurchaseUi(data);

			_storeController.ProcessOrder(data.order_id, () =>
			{
				_storeController.ResetCart();
				_storeController.RefreshVirtualCurrencyBalance();
			});
			UnlockBuyButton();
		}, (Error error) => {
			_storeController.ShowError(error);
			UnlockBuyButton();
		});
	}

	private void LockBuyButton()
	{
		buyButton.Lock();
		if (LoaderPrefab != null) {
			LoaderObject = Instantiate(LoaderPrefab, buyButton.transform);
		}
	}

	private void UnlockBuyButton()
	{
		buyButton.Unlock();
		if (LoaderObject != null) {
			Destroy(LoaderObject);
			LoaderObject = null;
		}
	}

	public void Initialize(float price)
	{
		priceText.text = "$" + price.ToString("F2");
	}
}