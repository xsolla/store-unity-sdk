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
	Button removeButton;

	Coroutine _loadingRoutine;
	
	CartItem _itemInformation;

	void Awake()
	{
		var storeController = FindObjectOfType<StoreController>();

		removeButton.onClick.AddListener(() =>
		{
			XsollaStore.Instance.RemoveItemFromCart(storeController.Cart, _itemInformation, () =>
				{
					Destroy(gameObject);
					FindObjectOfType<CartGroupUI>().DecreaseCounter(_itemInformation.quantity);
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