using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Xsolla;

public class CartItemUI : MonoBehaviour
{
	[SerializeField]
	Image _item_Image;
	[SerializeField]
	Text _item_Name;

	[SerializeField]
	Text _item_Price;
	[SerializeField]
	Button _remove_Button;

	Coroutine _loading_Routine;

	CartItem _itemInformation;

	void Awake()
	{
		var storeController = FindObjectOfType<StoreController>();

		_remove_Button.onClick.AddListener(() =>
		{
			XsollaStore.Instance.RemoveItemFromCart(storeController.Cart, _itemInformation, () => { Destroy(gameObject); },
				error => { Debug.Log(error.ToString()); });
		});
	}

	public void Initialize(CartItem itemInformation)
	{
		_itemInformation = itemInformation;

		if (_itemInformation.price != null)
			_item_Price.text = _itemInformation.price.amount + " " + _itemInformation.price.currency;

		_item_Name.text = _itemInformation.name;
	}

	void OnEnable()
	{
		if (_loading_Routine == null && _item_Image.sprite == null && _itemInformation.image_url != "")
			_loading_Routine = StartCoroutine(LoadImage(_itemInformation.image_url));
	}

	IEnumerator LoadImage(string url)
	{
		using (WWW www = new WWW(url))
		{
			yield return www;
			Sprite sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height),
				new Vector2(0.5f, 0.5f));
			_item_Image.sprite = sprite;
		}
	}
}