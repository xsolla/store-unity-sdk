using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Store;

public class InventoryItemUI : MonoBehaviour
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
	Text itemQuantity;
	
	InventoryItem _itemInformation;
	StoreController _storeController;

	Sprite _itemImage;

	private void Awake()
	{
		_storeController = FindObjectOfType<StoreController>();
	}

	public void Initialize(InventoryItem itemInformation)
	{
		_itemInformation = itemInformation;

		itemName.text = _itemInformation.name;
		itemDescription.text = _itemInformation.description;
		itemQuantity.text = _itemInformation.quantity.ToString();

		if (_itemImage == null && !string.IsNullOrEmpty(_itemInformation.image_url))
		{
			_storeController.GetImageAsync(_itemInformation.image_url, LoadImageCallback);
		}
	}

	void LoadImageCallback(string url, Sprite image)
	{
		loadingCircle.SetActive(false);
		itemImage.sprite = _itemImage = image;
	}
}