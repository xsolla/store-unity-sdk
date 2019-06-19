using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Xsolla;

public class ItemUI : MonoBehaviour
{
	[SerializeField]
	Image itemImage;
	[SerializeField]
	Text itemName;
	[SerializeField]
	Text itemDescription;
	[SerializeField]
	Button buyButton;
	[SerializeField]
	AddToCartButton addToCartButton;

	Coroutine _loadingRoutine;
	StoreItem _itemInformation;
	StoreController _storeController;

	void Awake()
	{
		_storeController = FindObjectOfType<StoreController>();

		buyButton.onClick.AddListener(() =>
		{
			XsollaStore.Instance.BuyItem(_itemInformation.sku, error => { Debug.Log(error.ToString()); });
		});

		addToCartButton.onClick = (bSelected =>
		{
			var cart = _storeController.Cart;
			if (cart != null)
			{
				if (bSelected)
				{
					XsollaStore.Instance.AddItemToCart(cart, _itemInformation.sku, new Quantity {quantity = 1},
						() =>
						{
							FindObjectOfType<CartGroupUI>().IncreaseCounter();
							
							if (!_storeController.cartItems.Contains(_itemInformation.sku))
							{
								_storeController.cartItems.Add(_itemInformation.sku);
							}
						}, error => print(error.ToString()));
				}
				else
				{
					XsollaStore.Instance.RemoveItemFromCart(cart, _itemInformation.sku,
						() =>
						{
							FindObjectOfType<CartGroupUI>().DecreaseCounter();
							
							if (_storeController.cartItems.Contains(_itemInformation.sku))
							{
								_storeController.cartItems.Remove(_itemInformation.sku);
							}
						}, error => print(error.ToString()));
				}
			}
		});
	}

	public void Initialize(StoreItem itemInformation)
	{
		_itemInformation = itemInformation;

//        if (_itemInformation.prices.Length != 0)
//        _item_Price.text = _itemInformation.prices[0].amount + " " + _itemInformation.prices[0].currency;

		itemName.text = _itemInformation.name;
		itemDescription.text = _itemInformation.description;
	}

	void OnEnable()
	{
		if (_loadingRoutine == null && itemImage.sprite == null && _itemInformation.image_url != "")
			_loadingRoutine = StartCoroutine(LoadImage(_itemInformation.image_url));
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
			Sprite sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height),
				new Vector2(0.5f, 0.5f));
			itemImage.sprite = sprite;
		}
	}
}