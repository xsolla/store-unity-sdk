using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Store;

public class ItemUI : MonoBehaviour
{
	[SerializeField]
	Image itemImage;
	[SerializeField]
	GameObject loadingCircle;
	[SerializeField]
	Text itemName;
	[SerializeField]
	Text itemDescription;
	[SerializeField]
	SimpleTextButton buyButton;
	[SerializeField]
	AddToCartButton addToCartButton;
	
	StoreItem _itemInformation;
	StoreController _storeController;
	ItemsController _itemsController;

	Sprite _itemImage;

	void Awake()
	{
		_storeController = FindObjectOfType<StoreController>();
		_itemsController = FindObjectOfType<ItemsController>();

		var cartGroup = FindObjectOfType<CartGroupUI>();

		buyButton.onClick = (() =>
		{
			var purchaseParams = new PurchaseParams();
			purchaseParams.currency = _itemInformation.prices[0].currency;
			XsollaStore.Instance.BuyItem(XsollaSettings.StoreProjectId, _itemInformation.sku, data =>
			{
				XsollaStore.Instance.OpenPurchaseUi(data);
				_storeController.ProcessOrder(data.order_id, () =>
				{
					_itemsController.RefreshActiveContainer();
				});
			}, _storeController.ShowError);
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
			var itemPrice = _itemInformation.prices.First(x => x.currency == RegionalCurrency.CurrencyCode);

			if (itemPrice != null)
			{
				buyButton.Text = FormatBuyButtonText(RegionalCurrency.CurrencySymbol, itemPrice.amount);
			}
			else
			{
				var currency = RegionalCurrency.GetCurrencySymbol(_itemInformation.prices[0].currency);
				if (string.IsNullOrEmpty(currency))
				{
					// if there is no symbol for specified currency then display currency code instead
					currency = _itemInformation.prices[0].currency;
				}
				
				buyButton.Text = FormatBuyButtonText(currency, _itemInformation.prices[0].amount);
			}
		}

		itemName.text = _itemInformation.name;
		itemDescription.text = _itemInformation.description;
	}

	string FormatBuyButtonText(string currency, float price)
	{
		return string.Format("BUY FOR {0}{1}", currency, price);
	}

	void OnEnable()
	{
		if (_itemImage == null && !string.IsNullOrEmpty(_itemInformation.image_url))
		{
			if (StoreController.ItemIcons.ContainsKey(_itemInformation.image_url))
			{
				loadingCircle.SetActive(false);
				itemImage.sprite = StoreController.ItemIcons[_itemInformation.image_url];
			}
			else
			{
				StartCoroutine(LoadImage(_itemInformation.image_url));
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
			
			yield return new WaitForSeconds(Random.Range(0.0f, 1.5f));
			
			var sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));

			_itemImage = sprite;
			
			loadingCircle.SetActive(false);
			itemImage.sprite = sprite;

			if (!StoreController.ItemIcons.ContainsKey(url))
			{
				StoreController.ItemIcons.Add(url, sprite);
			}
		}
	}
}