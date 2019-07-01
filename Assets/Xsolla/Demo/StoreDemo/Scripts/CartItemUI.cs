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
	
	CartItem _itemInformation;
	
	StoreController _storeController;

	void Awake()
	{
		_storeController = FindObjectOfType<StoreController>();
		var itemsController = FindObjectOfType<ItemsController>();
		var cartItemContainer = FindObjectOfType<CartItemContainer>();

		addButton.onClick = (() =>
		{
			XsollaStore.Instance.AddItemToCart(_storeController.Cart.id, _itemInformation.sku, _itemInformation.quantity + 1,
				() => { itemsController.RefreshCartContainer(); },
				error => { Debug.Log(error.ToString()); });
		});
		
		delButton.onClick = (() =>
		{
			if (_itemInformation.quantity - 1 <= 0)
			{
				XsollaStore.Instance.RemoveItemFromCart(_storeController.Cart.id, _itemInformation.sku, () =>
					{
						FindObjectOfType<CartGroupUI>().DecreaseCounter(_itemInformation.quantity);
					
						if (_storeController.cartItems.Contains(_itemInformation.sku))
						{
							_storeController.cartItems.Remove(_itemInformation.sku);
						}

						if (!_storeController.cartItems.Any())
						{
							cartItemContainer.ClearCartItems();
						}
					
						itemsController.RefreshCartContainer();
					},
					error => { Debug.Log(error.ToString()); });
			}
			else
			{
				XsollaStore.Instance.AddItemToCart(_storeController.Cart.id, _itemInformation.sku, _itemInformation.quantity - 1,
					() => { itemsController.RefreshCartContainer(); },
					error => { Debug.Log(error.ToString()); });
			}
		});
		
	}

	public void Initialize(CartItem itemInformation)
	{
		_itemInformation = itemInformation;

		if (_itemInformation.price != null)
		{
			itemPrice.text = _itemInformation.price.amount + " " + _itemInformation.price.currency;
		}

		itemName.text = _itemInformation.name;
		
		itemQuantity.text = _itemInformation.quantity.ToString();
		
		if (_loadingRoutine == null && itemImage.sprite == null && _itemInformation.image_url != "")
		{
			if (StoreController.ItemIcons.ContainsKey(_itemInformation.image_url))
			{
				itemImage.sprite = StoreController.ItemIcons[_itemInformation.image_url];
			}
			else
			{
				_loadingRoutine = StartCoroutine(LoadImage(_itemInformation.image_url));
			}
		}
	}

	void OnEnable()
	{

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