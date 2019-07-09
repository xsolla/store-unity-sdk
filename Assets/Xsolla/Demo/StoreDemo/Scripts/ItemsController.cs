using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Store;

public class ItemsController : MonoBehaviour
{
	[SerializeField]
	GameObject _itemContainer_Prefab;

	[SerializeField]
	GameObject _cartContainer_Prefab;

	Dictionary<string, GameObject> _containers = new Dictionary<string, GameObject>();

	string currentContainer;

	public void AddItem(StoreItem itemInformation)
	{
		foreach (var group in itemInformation.groups)
		{
			if (!_containers.ContainsKey(group))
			{
				//create container
				GameObject newContainer = Instantiate(_itemContainer_Prefab, transform);
				newContainer.SetActive(false);
				_containers.Add(group, newContainer);
			}

			//add to container
			GameObject groupContainer = _containers[group];
			groupContainer.GetComponent<ItemContainer>().AddItem(itemInformation);
		}
	}

	public void CreateCart()
	{
		GameObject newContainer = Instantiate(_cartContainer_Prefab, transform);
		newContainer.SetActive(false);
		_containers.Add("CART", newContainer);
	}

	public void ActivateContainer(string groupId)
	{
		currentContainer = groupId;

		foreach (var container in _containers.Values)
		{
			container.SetActive(false);
		}

		if (_containers.ContainsKey(groupId))
		{
			if (groupId != "CART")
			{
				_containers[groupId].SetActive(true);
				_containers[groupId].GetComponent<ItemContainer>().Refresh();
			}
			else
			{
				RefreshCartContainer();
			}
		}
	}

	public void RefreshCartContainer()
	{
		var storeController = FindObjectOfType<StoreController>();

		if (currentContainer != "CART")
		{
			return;
		}

		var cartItemContainer = _containers["CART"].GetComponent<CartItemContainer>();

		cartItemContainer.ClearCartItems();

		_containers["CART"].SetActive(true);

		if (!storeController.CartModel.CartItems.Any())
		{
			return;
		}
		
		foreach (var item in storeController.CartModel.CartItems)
		{
			cartItemContainer.AddCartItem(item.Value);
		}

		var discount = storeController.CartModel.CalculateCartDiscount();
		if (discount > 0.0f)
		{
			cartItemContainer.AddDiscount(discount);
		}
			
		var p = new CartPrice();
		p.amount = storeController.CartModel.CalculateCartPrice();
		p.currency = "USD";
		
		cartItemContainer.AddControls(p);
		
	}
}