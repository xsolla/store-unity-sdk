using System;
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
			ImageLoader.Instance.GetImageAsync(_itemInformation.image_url, LoadImageCallback);
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
		consumeButton.gameObject.SetActive(true);
		consumeButton.onClick = ConsumeHandler;
		consumeButton.counter.ValueChanged += Counter_ValueChanged;
	}

	private void Counter_ValueChanged(int newValue)
	{
		if(newValue > _itemInformation.quantity) {
			StartCoroutine(DecreaseConsumeQuantityCoroutine());
		}
	}

	IEnumerator DecreaseConsumeQuantityCoroutine()
	{
		yield return new WaitForEndOfFrame();
		consumeButton.counter.DecreaseValue(1);
	}

	void DisableConsumeButton()
	{
		consumeButton.gameObject.SetActive(false);
	}

	void ConsumeHandler()
	{
		if(consumeButton.counter > 1) {
			Debug.LogWarning(
				"Sorry, but Xsolla_API can consume only one item at time, " +
				"so we send " + consumeButton.counter.GetValue() + " requests."
			);
		}
		
		_storeController.ShowConfirm(
			() => {
				loadingCircle.SetActive(true);
				DisableConsumeButton();
				ConsumeConfirmCase(consumeButton.counter.GetValue() - 1);
			},
			null,
			"Item '" + _itemInformation.name + "' x " + consumeButton.counter + " will be consumed. Are you sure?"
		);
	}

	void ConsumeConfirmCase(int attemptsLeft)
	{
		if(attemptsLeft > 0) {
			SendConsumeRequest(() => ConsumeConfirmCase(--attemptsLeft));
		} else {
			SendConsumeRequest(ConsumeItemsSuccess);
		}
	}

	void SendConsumeRequest(Action callback)
	{
		XsollaStore.Instance.ConsumeInventoryItem(XsollaSettings.StoreProjectId,
			GetConsumeItemBySku(_itemInformation.sku),
			callback,
			ConsumeItemsFailed
		);
	}

	ConsumeItem GetConsumeItemBySku(string sku)
	{
		return new ConsumeItem() {
			sku = sku,
			quantity = 1
		};
	}

	void ConsumeItemsSuccess()
	{
		EnableConsumeButton();
		loadingCircle.SetActive(false);
		_storeController.ShowSuccess();
		_storeController.RefreshInventory();
	}

	void ConsumeItemsFailed(Error error)
	{
		EnableConsumeButton();
		loadingCircle.SetActive(false);
		_storeController.ShowError(error);
	}
}