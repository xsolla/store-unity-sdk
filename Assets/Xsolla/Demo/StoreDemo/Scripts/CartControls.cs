using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Store;

public class CartControls : MonoBehaviour
{
	[SerializeField]
	SimpleButton buyButton;
	[SerializeField]
	SimpleButton clearCartButton;
	[SerializeField]
	Text priceText;

	int _totalItems;
	int _completedRequests;
	
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
			XsollaStore.Instance.ClearCart(XsollaSettings.StoreProjectId, _storeController.Cart.cart_id, () =>
			{
				_totalItems = _storeController.CartModel.CartItems.Count;
				_completedRequests = 0;
			
				foreach (var cartItem in _storeController.CartModel.CartItems)
				{
					XsollaStore.Instance.UpdateItemInCart(XsollaSettings.StoreProjectId, _storeController.Cart.cart_id, cartItem.Key, cartItem.Value.Quantity,
						() =>
						{
							_completedRequests++;
						}, error =>
						{
							_completedRequests++;
							_storeController.ShowError(error); 
						});
				}

				StartCoroutine(BuyCart());
			}, _storeController.ShowError);
		});
	}

	IEnumerator BuyCart()
	{
		yield return new WaitUntil(() => _completedRequests == _totalItems);

		XsollaStore.Instance.BuyCart(XsollaSettings.StoreProjectId, _storeController.Cart.cart_id, data =>
		{
			XsollaStore.Instance.OpenPurchaseUi(data);

			_storeController.ProcessOrder(data.order_id, () =>
			{
				_storeController.ResetCart();
				_storeController.RefreshVirtualCurrencyBalance();
			});
		}, _storeController.ShowError);
	}
	
	public void Initialize(float price)
	{
		priceText.text = "$" + price.ToString("F2");
	}
}