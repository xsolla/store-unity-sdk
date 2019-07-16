using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

	void Awake()
	{
		var storeController = FindObjectOfType<StoreController>();
		var cartItemContainer = FindObjectOfType<CartItemContainer>();
		var cartGroup = FindObjectOfType<CartGroupUI>();

		addButton.onClick = (() =>
		{
			storeController.CartModel.IncrementCartItem(_itemInformation.Sku);
			cartItemContainer.Refresh();
		});
		
		delButton.onClick = (() =>
		{
			if (_itemInformation.Quantity - 1 <= 0)
			{
				cartGroup.DecreaseCounter(_itemInformation.Quantity);
			}

			storeController.CartModel.DecrementCartItem(_itemInformation.Sku);
			cartItemContainer.Refresh();
		});
	}

	public void Initialize(CartItemModel itemInformation)
	{
		_itemInformation = itemInformation;

		itemPrice.text = "$" + _itemInformation.Price;
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