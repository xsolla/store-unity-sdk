using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Xsolla;

public class CartItemUI : MonoBehaviour
{
	[SerializeField]
	Image itemImage;
	[SerializeField]
	Text itemName;
	[SerializeField]
	Text itemPrice;
	[SerializeField]
	SimpleButton removeButton;

	Coroutine _loadingRoutine;
	
	CartItem _itemInformation;
	
	StoreController _storeController;

	void Awake()
	{
		_storeController = FindObjectOfType<StoreController>();

		removeButton.onClick = (() =>
		{
			XsollaStore.Instance.RemoveItemFromCart(_storeController.Cart, _itemInformation.sku, () =>
				{
					Destroy(gameObject);
					FindObjectOfType<CartGroupUI>().DecreaseCounter(_itemInformation.quantity);
					
					if (_storeController.cartItems.Contains(_itemInformation.sku))
					{
						_storeController.cartItems.Remove(_itemInformation.sku);
					}
				},
				error => { Debug.Log(error.ToString()); });
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
	}

	void OnEnable()
	{
		if (_loadingRoutine == null && itemImage.sprite == null && _itemInformation.image_url != "")
		{
			_loadingRoutine = StartCoroutine(LoadImage(_itemInformation.image_url));
		}
	}

	IEnumerator LoadImage(string url)
	{
		using (WWW www = new WWW(url))
		{
			yield return www;
			var sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
			itemImage.sprite = sprite;
		}
	}
}