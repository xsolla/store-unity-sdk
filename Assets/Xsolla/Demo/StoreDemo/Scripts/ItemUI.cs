using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Store;

public class ItemUI : MonoBehaviour
{
	[SerializeField]
	Image itemImage;
	[SerializeField]
	Text itemName;
	[SerializeField]
	Text itemDescription;
	[SerializeField]
	SimpleTextButton buyButton;
	[SerializeField]
	AddToCartButton addToCartButton;

	Coroutine _loadingRoutine;
	StoreItem _itemInformation;
	StoreController _storeController;

	void Awake()
	{
		_storeController = FindObjectOfType<StoreController>();

		buyButton.onClick = (() =>
		{
			var purchaseParams = new PurchaseParams();
			purchaseParams.currency = _itemInformation.prices[0].currency;
			XsollaStore.Instance.BuyItem(_itemInformation.sku, error => { Debug.Log(error.ToString()); });
		});

		addToCartButton.onClick = (bSelected =>
		{
			var cart = _storeController.Cart;
			if (cart != null)
			{
				if (bSelected)
				{
					_storeController.CartModel.AddCartItem(_itemInformation);
					
					FindObjectOfType<CartGroupUI>().IncreaseCounter();

					if (!_storeController.cartItems.Contains(_itemInformation.sku))
					{
						_storeController.cartItems.Add(_itemInformation.sku);
					}
				}
				else
				{
					_storeController.CartModel.RemoveCartItem(_itemInformation);
					
					FindObjectOfType<CartGroupUI>().DecreaseCounter();

					if (_storeController.cartItems.Contains(_itemInformation.sku))
					{
						_storeController.cartItems.Remove(_itemInformation.sku);
					}
				}
			}
		});
	}

	public void Initialize(StoreItem itemInformation)
	{
		_itemInformation = itemInformation;

		if (_itemInformation.prices.Length != 0)
		{
			buyButton.Text = string.Format("BUY FOR ${0}", _itemInformation.prices[0].amount);
		}

		itemName.text = _itemInformation.name;
		itemDescription.text = _itemInformation.description;
	}

	void OnEnable()
	{
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

	public void Refresh()
	{
		addToCartButton.Select(_storeController.cartItems.Contains(_itemInformation.sku));
	}

	IEnumerator LoadImage(string url)
	{
		using (WWW www = new WWW(url))
		{
			yield return www;
			
			Sprite sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
			
			itemImage.sprite = sprite;

			StoreController.ItemIcons.Add(url, sprite);
		}
	}
}