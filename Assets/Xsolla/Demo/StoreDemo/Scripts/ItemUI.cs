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

		var cartGroup = FindObjectOfType<CartGroupUI>();

		buyButton.onClick = (() =>
		{
			var purchaseParams = new PurchaseParams();
			purchaseParams.currency = _itemInformation.prices[0].currency;
			XsollaStore.Instance.BuyItem(_itemInformation.sku, print);
		});

		addToCartButton.onClick = (bSelected =>
		{
			if (bSelected)
			{
				_storeController.CartModel.AddCartItem(_itemInformation);
				cartGroup.IncreaseCounter();
			}
			else
			{
				_storeController.CartModel.RemoveCartItem(_itemInformation.sku); 
				cartGroup.DecreaseCounter();
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
		if (_loadingRoutine == null && itemImage.sprite == null && !string.IsNullOrEmpty(_itemInformation.image_url))
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
		addToCartButton.Select(_storeController.CartModel.CartItems.ContainsKey(_itemInformation.sku));
	}

	IEnumerator LoadImage(string url)
	{
		using (var www = new WWW(url))
		{
			yield return www;
			
			var sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
			
			itemImage.sprite = sprite;

			if (!StoreController.ItemIcons.ContainsKey(url))
			{
				StoreController.ItemIcons.Add(url, sprite);
			}
		}
	}
}