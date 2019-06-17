using UnityEngine;
using UnityEngine.UI;
using Xsolla;

public class CartControls : MonoBehaviour
{
	[SerializeField]
	Button buyButton;
	[SerializeField]
	Text priceText;

	CartPrice cartPrice;
	
	void Awake()
	{
		var storeController = FindObjectOfType<StoreController>();

		buyButton.onClick.AddListener(() =>
		{
			XsollaStore.Instance.BuyCart(storeController.Cart, error => { Debug.Log(error.ToString()); });
		});
	}
	
	public void Initialize(CartPrice price)
	{
		cartPrice = price;
		
		priceText.text = price.amount + " " + price.currency;
	}
}