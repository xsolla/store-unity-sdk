using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

public class InventoryItemUI : MonoBehaviour
{
	[SerializeField] private Image itemImage;
	[SerializeField] private GameObject loadingCircle;
	[SerializeField] private Text itemName;
	[SerializeField] private Text itemDescription;
	[SerializeField] private GameObject itemQuantityImage;
	[SerializeField] private Text itemQuantityText;
	[SerializeField] private ConsumeButton consumeButton;
	[SerializeField] private Text notPurchasedText;
	[SerializeField] private GameObject expirationTimeObject;
	[SerializeField] private Text expirationTimeText;

	private ItemModel _itemInformation;
	private IDemoImplementation _demoImplementation;

	private void Awake()
	{
		DisableConsumeButton();
	}

	public void Initialize(ItemModel itemInformation, IDemoImplementation demoImplementation)
	{
		_demoImplementation = demoImplementation;
		_itemInformation = itemInformation;

		itemName.text = _itemInformation.Name;
		itemDescription.text = _itemInformation.Description;

		LoadImage(_itemInformation.ImageUrl);
	}

	private void LoadImage(string url)
	{
		if (!string.IsNullOrEmpty(url))
			ImageLoader.Instance.GetImageAsync(url, LoadImageCallback);
		else
		{
			Debug.LogError($"Inventory item with sku = '{_itemInformation.Sku}' have not image!");
			LoadImageCallback(string.Empty, null);
		}
	}

	private void LoadImageCallback(string url, Sprite image)
	{
		loadingCircle.SetActive(false);
		itemImage.sprite = image;

		RefreshUi();
	}

	private void RefreshUi()
	{
		DisableQuantityImage();
		DisableConsumeButton();
		DisableExpirationText();
		DisableNotPurchasedText();
		
		if (_itemInformation.IsSubscription())
			DrawSubscriptionItem();
		else
		{
			if (_itemInformation.IsConsumable)
				DrawConsumableVirtualItem();
			else
				DrawNonConsumableVirtualItem();
		}
	}

	private void DrawSubscriptionItem()
	{
		var model = UserInventory.Instance.Subscriptions.First(i => i.Sku.Equals(_itemInformation.Sku));
		if (model.Status != UserSubscriptionModel.SubscriptionStatusType.None && model.Expired.HasValue)
		{
			var expired = model.Expired.Value.ToString("dd/MM/yyyy hh:mm:tt");
			EnableExpirationText(
				(model.Status == UserSubscriptionModel.SubscriptionStatusType.Active && model.Expired > DateTime.Now) 
				? GetRemainingTime(model.Expired.Value)
				: $"Expired at {expired}");
		}else
			EnableNotPurchasedText();
	}

	private string GetRemainingTime(DateTime expiredDateTime)
	{
		var timeLeft = expiredDateTime - DateTime.Now;
		StartCoroutine(RemainingTimeCoroutine(timeLeft.TotalSeconds > 60 ? 60 : 1));
		if (timeLeft.TotalDays >= 30)
			return $"{(int)(timeLeft.TotalDays / 30)} month{(timeLeft.TotalDays > 60 ? "s" : "")} remaining";
		if (timeLeft.TotalDays > 1)
			return $"{(uint)(timeLeft.TotalDays)} day{(timeLeft.TotalDays > 1 ? "s" : "")} remaining";
		if (timeLeft.TotalHours > 1)
			return $"{(uint)(timeLeft.TotalHours)} hour{(timeLeft.TotalHours > 1 ? "s" : "")} remaining";
		if (timeLeft.TotalMinutes > 1)
			return $"{(uint)(timeLeft.TotalMinutes)} minute{(timeLeft.TotalMinutes > 1 ? "s" : "")} remaining";
		return $"{(uint)(timeLeft.TotalSeconds)} second{(timeLeft.TotalSeconds > 1 ? "s" : "")} remaining";
	}

	private IEnumerator RemainingTimeCoroutine(float waitSeconds)
	{
		yield return new WaitForSeconds(waitSeconds);
		RefreshUi();
	}

	private void DrawConsumableVirtualItem()
	{
		var model = UserInventory.Instance.VirtualItems.First(i => i.Sku.Equals(_itemInformation.Sku));
		DrawItemsCount(model);
		EnableConsumeButton();
	}
	
	private void DrawNonConsumableVirtualItem()
	{
		var model = UserInventory.Instance.VirtualItems.First(i => i.Sku.Equals(_itemInformation.Sku));
		DrawItemsCount(model);
		EnableNotPurchasedText();
	}
	
	private void DrawItemsCount(InventoryItemModel model)
	{
		if (model.RemainingUses == null || itemQuantityImage == null) return;
		EnableQuantityImage();
		itemQuantityText.text = model.RemainingUses.Value.ToString();
	}

	private void EnableQuantityImage()
	{
		itemQuantityImage.SetActive(true);
	}
	
	private void DisableQuantityImage()
	{
		itemQuantityImage.SetActive(false);
	}

	private void EnableNotPurchasedText()
	{
		if(notPurchasedText != null)
			notPurchasedText.gameObject.SetActive(true);
	}
	
	private void DisableNotPurchasedText()
	{
		if(notPurchasedText != null)
			notPurchasedText.gameObject.SetActive(false);
	}

	private void EnableExpirationText(string text)
	{
		expirationTimeObject.SetActive(true);
		expirationTimeText.text = text;
	}
	
	private void DisableExpirationText()
	{
		expirationTimeObject.SetActive(false);
	}

	private void EnableConsumeButton()
	{
		consumeButton.gameObject.SetActive(true);
		consumeButton.onClick = ConsumeHandler;
		if (consumeButton.counter < 1)
			consumeButton.counter.IncreaseValue(1 - consumeButton.counter.GetValue());
		consumeButton.counter.ValueChanged += Counter_ValueChanged;
	}
	
	private void DisableConsumeButton()
	{
		consumeButton.counter.ValueChanged -= Counter_ValueChanged;
		consumeButton.gameObject.SetActive(false);
	}

	private void ConsumeHandler()
	{
		loadingCircle.SetActive(true);
		DisableConsumeButton();
		var model = UserInventory.Instance.VirtualItems.First(i => i.Sku.Equals(_itemInformation.Sku));
		_demoImplementation.ConsumeInventoryItem(
			model, (uint) consumeButton.counter.GetValue(), _ => ConsumeItemsSuccess(), _ => ConsumeItemsFailed());
	}

	private void ConsumeItemsSuccess()
	{
		UserInventory.Instance.Refresh();
	}

	private void ConsumeItemsFailed()
	{
		EnableConsumeButton();
		loadingCircle.SetActive(false);
	}
	
	private void Counter_ValueChanged(int newValue)
	{
		var model = UserInventory.Instance.VirtualItems.First(i => i.Sku.Equals(_itemInformation.Sku));
		if (newValue > model.RemainingUses)
			StartCoroutine(ChangeConsumeQuantityCoroutine((-1) * ((int)model.RemainingUses - newValue)));
		else
		{
			if (newValue == 0)
				StartCoroutine(ChangeConsumeQuantityCoroutine(1));
		}
	}

	private IEnumerator ChangeConsumeQuantityCoroutine(int deltaValue)
	{
		yield return new WaitForEndOfFrame();
		if(deltaValue < 0)
			consumeButton.counter.DecreaseValue(deltaValue * (-1));
		else
			consumeButton.counter.IncreaseValue(deltaValue);
	}
}