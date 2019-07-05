using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Store;

public class CartItemUI : MonoBehaviour
{
	[SerializeField]
	Image itemImage;
	[SerializeField]
	Text itemName;
	[SerializeField]
	Text itemPrice;

	[SerializeField]
	SimpleButton addButton;
	[SerializeField]
	SimpleButton delButton;
	
	[SerializeField]
	Text itemQuantity;
	
	Coroutine _loadingRoutine;
	
	CartItemModel _itemInformation;
	
	StoreController _storeController;

	void Awake()
	{
		_storeController = FindObjectOfType<StoreController>();
		var itemsController = FindObjectOfType<ItemsController>();
		var cartItemContainer = FindObjectOfType<CartItemContainer>();

		addButton.onClick = (() =>
		{
			_storeController.CartModel.IncrementCartItem(_itemInformation.Sku);
			itemsController.RefreshCartContainer();
		});
		
		delButton.onClick = (() =>
		{
			if (_itemInformation.Quantity - 1 <= 0)
			{
				_storeController.CartModel.DecrementCartItem(_itemInformation.Sku);
				
				FindObjectOfType<CartGroupUI>().DecreaseCounter(_itemInformation.Quantity);
			
				if (_storeController.cartItems.Contains(_itemInformation.Sku))
				{
					_storeController.cartItems.Remove(_itemInformation.Sku);
				}

				if (!_storeController.cartItems.Any())
				{
					cartItemContainer.ClearCartItems();
				}
			
				itemsController.RefreshCartContainer();
			}
			else
			{
				_storeController.CartModel.DecrementCartItem(_itemInformation.Sku);
				itemsController.RefreshCartContainer();
			}
		});
		
	}

	public void Initialize(CartItemModel itemInformation)
	{
		_itemInformation = itemInformation;

		itemPrice.text = _itemInformation.Price + " " + _itemInformation.Currency;

		itemName.text = _itemInformation.Name;
		
		itemQuantity.text = _itemInformation.Quantity.ToString();
		
		if (_loadingRoutine == null && itemImage.sprite == null && _itemInformation.ImgUrl != "")
		{
			if (StoreController.ItemIcons.ContainsKey(_itemInformation.ImgUrl))
			{
				itemImage.sprite = StoreController.ItemIcons[_itemInformation.ImgUrl];
			}
			else
			{
				_loadingRoutine = StartCoroutine(LoadImage(_itemInformation.ImgUrl));
			}
		}
	}

	IEnumerator LoadImage(string url)
	{
		using (WWW www = new WWW(url))
		{
			yield return www;
			var sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
			itemImage.sprite = sprite;
			
			StoreController.ItemIcons.Add(url, sprite);
		}
	}
}