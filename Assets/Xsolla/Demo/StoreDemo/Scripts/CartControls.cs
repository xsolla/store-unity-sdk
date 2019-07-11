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

	int totalItems;
	int completedRequests;
	
	StoreController _storeController;

	void Awake()
	{
		_storeController = FindObjectOfType<StoreController>();

		buyButton.onClick = (() =>
		{
			XsollaStore.Instance.ClearCart(_storeController.Cart.id, () =>
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
							completedRequests++;
							Debug.Log(error.ToString()); 
						});
				}

				StartCoroutine(BuyCart());
			}, error => { Debug.Log(error.ToString()); });
		});
	}

	IEnumerator BuyCart()
	{
		yield return new WaitUntil(() => completedRequests == totalItems);
		
		XsollaStore.Instance.BuyCart(_storeController.Cart.id, error => { Debug.Log(error.ToString()); });
	}
	
	public void Initialize(float price)
	{
		priceText.text = "$" + price.ToString("F2");
	}
}