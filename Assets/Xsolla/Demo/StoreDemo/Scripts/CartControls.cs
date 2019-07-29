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
	Text priceText;

	int totalItems;
	int completedRequests;
	
	StoreController _storeController;
	
	Coroutine _checkStatusCor;

	void Awake()
	{
		_storeController = FindObjectOfType<StoreController>();

		buyButton.onClick = (() =>
		{
			XsollaStore.Instance.ClearCart(XsollaSettings.StoreProjectId, _storeController.Cart.id, () =>
			{
				totalItems = _storeController.CartModel.CartItems.Count;
				completedRequests = 0;
			
				foreach (var cartItem in _storeController.CartModel.CartItems)
				{
					XsollaStore.Instance.AddItemToCart(XsollaSettings.StoreProjectId, _storeController.Cart.id, cartItem.Key, cartItem.Value.Quantity,
						() =>
						{
							completedRequests++;
						}, error =>
						{
							completedRequests++;
							Debug.Log(error.ToString()); 
						});
				}

				StartCoroutine(BuyCart());
			}, error => { Debug.Log(error.ToString()); });
		});
	}
	
	void OnDestroy()
	{
		if (_checkStatusCor != null)
		{
			StopCoroutine(_checkStatusCor);
		}
	}

	IEnumerator BuyCart()
	{
		yield return new WaitUntil(() => completedRequests == totalItems);

		XsollaStore.Instance.BuyCart(XsollaSettings.StoreProjectId, _storeController.Cart.id, data =>
		{
			XsollaStore.Instance.GetCartItems(XsollaSettings.StoreProjectId, _storeController.Cart.id, print, print);
			
			XsollaStore.Instance.OpenPurchaseUi(data);
			_checkStatusCor = StartCoroutine(CheckOrderStatus(data.order_id));
		},print);
	}
	
	public void Initialize(float price)
	{
		priceText.text = "$" + price.ToString("F2");
	}
	
	IEnumerator CheckOrderStatus(int orderId)
	{
		yield return new WaitForSeconds(5.0f);
		
		XsollaStore.Instance.CheckOrderStatus(XsollaSettings.StoreProjectId, orderId,status =>
		{
			if (status.Status != OrderStatusType.Paid)
			{
				print("Waiting for order to be processed...");
				_checkStatusCor = StartCoroutine(CheckOrderStatus(orderId));
			}
			else
			{
				print("Order was successfully processed!");
			}
		}, print);
	}
}