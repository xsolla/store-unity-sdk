using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Store;

public class CartControls : MonoBehaviour
{
	[SerializeField]
	SimpleButton buyButton;
	[SerializeField]
	Text priceText;

	CartPrice cartPrice;

	int totalItems;
	int completedRequests;
	
	StoreController _storeController;

	void Awake()
	{
		_storeController = FindObjectOfType<StoreController>();

		buyButton.onClick = (() =>
		{
			totalItems = _storeController.CartModel.CartItems.Count;
			completedRequests = 0;
			
			foreach (var cartItem in _storeController.CartModel.CartItems)
			{
				XsollaStore.Instance.AddItemToCart(_storeController.Cart.id, cartItem.Key, cartItem.Value.Quantity,
					() =>
					{
						completedRequests++;
					}, error =>
					{
						completedRequests++;Debug.Log(error.ToString()); 
					});
			}

			StartCoroutine(BuyCart());
		});
	}

	IEnumerator BuyCart()
	{
		yield return new WaitUntil(() => completedRequests == totalItems);
		
		XsollaStore.Instance.BuyCart(_storeController.Cart.id, error => { Debug.Log(error.ToString()); });
	}
	
	public void Initialize(CartPrice price)
	{
		cartPrice = price;
		
		priceText.text = price.amount + " " + price.currency;
	}
}