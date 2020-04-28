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

	private void Awake()
	{
		DisableConsumeButton();
	}

	public void Initialize(InventoryItem itemInformation)
	{
		_itemInformation = itemInformation;

		itemName.text = _itemInformation.name;
		itemDescription.text = _itemInformation.description;
		itemQuantity.text = _itemInformation.quantity.ToString();

		if(!string.IsNullOrEmpty(_itemInformation.image_url))
			ImageLoader.Instance.GetImageAsync(_itemInformation.image_url, LoadImageCallback);
		else
		{
			loadingCircle.SetActive(false);
			itemImage.sprite = null;
		}
	}

	void LoadImageCallback(string url, Sprite image)
	{
		loadingCircle.SetActive(false);
		itemImage.sprite = image;

		RefreshConsumeButton();
	}

	void RefreshConsumeButton()
	{
		if (_itemInformation.IsConsumable()) {
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
	
	void DisableConsumeButton()
	{
		consumeButton.gameObject.SetActive(false);
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

	void ConsumeHandler()
	{
		if(consumeButton.counter > 1) {
			Debug.LogWarning(
				"Sorry, but Xsolla_API can consume only one item at time, " +
				"so we send " + consumeButton.counter.GetValue() + " requests."
			);
		}
		StoreDemoPopup.ShowConfirm(
			() => {
				loadingCircle.SetActive(true);
				DisableConsumeButton();
				ConsumeConfirmCase(consumeButton.counter.GetValue() - 1);
			},
			null,
			$"Item '{_itemInformation.name}' x {consumeButton.counter} will be consumed. Are you sure?"
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

	private void SendConsumeRequest(Action callback)
	{
		XsollaStore.Instance.ConsumeInventoryItem(XsollaSettings.StoreProjectId,
			GetConsumeItemBySku(_itemInformation.sku),
			callback,
			ConsumeItemsFailed
		);
	}

	private ConsumeItem GetConsumeItemBySku(string sku)
	{
		return new ConsumeItem() {
			sku = sku,
			quantity = 1
		};
	}

	private void ConsumeItemsSuccess()
	{
		EnableConsumeButton();
		loadingCircle.SetActive(false);
		StoreDemoPopup.ShowSuccess();
		UserInventory.Instance.UpdateVirtualItems();
	}

	private void ConsumeItemsFailed(Error error)
	{
		EnableConsumeButton();
		loadingCircle.SetActive(false);
		StoreDemoPopup.ShowError(error);
	}
}