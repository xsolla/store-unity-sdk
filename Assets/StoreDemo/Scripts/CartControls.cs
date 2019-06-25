using UnityEngine;
using UnityEngine.UI;
using Xsolla;

public class CartControls : MonoBehaviour
{
	[SerializeField]
	SimpleButton buyButton;
	[SerializeField]
	Text priceText;

	CartPrice cartPrice;
	
	void Awake()
	{
		var storeController = FindObjectOfType<StoreController>();

		buyButton.onClick = (() =>
		{
			XsollaStore.Instance.BuyCart(storeController.Cart.id, error => { Debug.Log(error.ToString()); });
		});
	}
	
	public void Initialize(CartPrice price)
	{
		cartPrice = price;
		
		priceText.text = price.amount + " " + price.currency;
	}
}