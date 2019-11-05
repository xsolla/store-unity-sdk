using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
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
	[SerializeField]
	ConsumeButton consumeButton;
	
	InventoryItem _itemInformation;
	StoreController _storeController;

	Sprite _itemImage;

	private void Awake()
	{
		_storeController = FindObjectOfType<StoreController>();

		DisableConsumeButton();
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

		RefreshConsumeButton();
	}

	void RefreshConsumeButton()
	{
		if (_itemInformation.remaining_uses != null) {
			EnableConsumeButton();
		} else {
			DisableConsumeButton();
		}
	}

	void EnableConsumeButton()
	{
		consumeButton.transform.parent.gameObject.SetActive(true);
		consumeButton.onClick = ConsumeHandler;
	}

	void ConsumeHandler()
	{
		_storeController.ShowConfirm(
			ConsumeConfirmCase,
			null,
			"Item '" + _itemInformation.name + "' x " + consumeButton.counter + " will be consumed. Are you sure?"
		);
	}

	void ConsumeConfirmCase()
	{
		XsollaStore.Instance.ConsumeInventoryItem(XsollaSettings.StoreProjectId,
			new ConsumeItem() {
				sku = _itemInformation.sku,
				quantity = consumeButton.counter
			},
			() => {
				_storeController.ShowSuccess();
				_storeController.RefreshInventory();
			},
			_storeController.ShowError
		);
	}

	void DisableConsumeButton()
	{
		consumeButton.transform.parent.gameObject.SetActive(false);
	}
}