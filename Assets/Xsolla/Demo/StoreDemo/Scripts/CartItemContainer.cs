using System.Collections.Generic;
using UnityEngine;
using Xsolla.Store;

public class CartItemContainer : MonoBehaviour
{
	[SerializeField]
	GameObject _cart_item_Prefab;

	[SerializeField]
	GameObject _cart_controls_Prefab;
	
	[SerializeField]
	Transform _item_Parent;

	[SerializeField]
	SimpleButton clearCartButton;

	List<GameObject> cartItems;

	GameObject cartControls;
	
	StoreController _storeController;

	void Awake()
	{
		_storeController = FindObjectOfType<StoreController>();
		
		cartItems = new List<GameObject>();

		clearCartButton.onClick = ClearCart;
	}

	void ClearCart()
	{
		XsollaStore.Instance.ClearCart(_storeController.Cart.id, () =>
		{
			ClearCartItems();
			_storeController.cartItems.Clear();
			FindObjectOfType<CartGroupUI>().ResetCounter();
		}, error => { print(error.ToString());});
	}

	public void AddCartItem(CartItemModel itemInformation)
	{
		GameObject newItem = Instantiate(_cart_item_Prefab, _item_Parent);
		cartItems.Add(newItem);
		newItem.GetComponent<CartItemUI>().Initialize(itemInformation);
	}

	public void AddControls(CartPrice price)
	{
		cartControls = Instantiate(_cart_controls_Prefab, _item_Parent);
		cartControls.GetComponent<CartControls>().Initialize(price); 
	}

	public void ClearCartItems()
	{
		foreach (var item in cartItems)
		{
			Destroy(item);
		}
		
		cartItems.Clear();

		Destroy(cartControls);
	}
}