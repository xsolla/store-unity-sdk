using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Core.Popup;

public class InventoryItemUI : MonoBehaviour
{
	[SerializeField] private Image itemImage;
	[SerializeField] private GameObject loadingCircle;
	[SerializeField] private Text itemName;
	[SerializeField] private Text itemDescription;
	[SerializeField] private GameObject itemQuantityImage;
	[SerializeField] private Text itemQuantityText;
	[SerializeField] private ConsumeButton consumeButton;
	[SerializeField] private Text purchasedStatusText;
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
			var message = string.Format("Inventory item with sku = '{0}' have not image!", _itemInformation.Sku);
			Debug.LogError(message);
			LoadImageCallback(string.Empty, null);
		}
	}

	private void LoadImageCallback(string url, Sprite image)
	{
		loadingCircle.SetActive(false);
		itemImage.gameObject.SetActive(true);
		itemImage.sprite = image;

		RefreshUi();
	}

	private void RefreshUi()
	{
		DisableQuantityImage();
		DisableConsumeButton();
		DisableExpirationText();
		DisablePurchasedStatusText();
		
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
				: string.Format("Expired at {0}", expired));
		}else
			EnablePurchasedStatusText(isPurchased: false);
	}

	private string GetRemainingTime(DateTime expiredDateTime)
	{
		var timeLeft = expiredDateTime - DateTime.Now;
		StartCoroutine(RemainingTimeCoroutine(timeLeft.TotalSeconds > 60 ? 60 : 1));

		if (timeLeft.TotalDays >= 30)
			return string.Format ("{0} month{1} remaining", (int)(timeLeft.TotalDays / 30), (timeLeft.TotalDays > 60 ? "s" : ""));

		if (timeLeft.TotalDays > 1)
			return string.Format("{0} day{1} remaining", (uint)(timeLeft.TotalDays), (timeLeft.TotalDays > 1 ? "s" : ""));

		if (timeLeft.TotalHours > 1)
			return string.Format("{0} hour{1} remaining", (uint)(timeLeft.TotalHours), (timeLeft.TotalHours > 1 ? "s" : ""));

		if (timeLeft.TotalMinutes > 1)
			return string.Format("{0} minute{1} remaining", (uint)(timeLeft.TotalMinutes), (timeLeft.TotalMinutes > 1 ? "s" : ""));

		return string.Format("{0} second{1} remaining", (uint)(timeLeft.TotalSeconds), (timeLeft.TotalSeconds > 1 ? "s" : ""));
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
		EnablePurchasedStatusText(isPurchased: true);
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

	private void EnablePurchasedStatusText(bool isPurchased)
	{
		if(purchasedStatusText != null)
		{
			purchasedStatusText.text = isPurchased ? "Purchased" : "Not purchased";
			purchasedStatusText.gameObject.SetActive(true);
		}
	}
	
	private void DisablePurchasedStatusText()
	{
		if(purchasedStatusText != null)
			purchasedStatusText.gameObject.SetActive(false);
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
		var model = UserInventory.Instance.VirtualItems.First(i => i.Sku.Equals(_itemInformation.Sku));
		DisableConsumeButton();
		_demoImplementation.ConsumeInventoryItem(model, (uint) consumeButton.counter.GetValue(), 
			_ => UserInventory.Instance.Refresh(), _ => EnableConsumeButton());
	}

	private void Counter_ValueChanged(int newValue)
	{
		var model = UserInventory.Instance.VirtualItems.First(i => i.Sku.Equals(_itemInformation.Sku));
		if (newValue > model.RemainingUses)
		{
			var delta = (int)model.RemainingUses - newValue;
			StartCoroutine(ChangeConsumeQuantityCoroutine(delta));
		}
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
			consumeButton.counter.DecreaseValue(-deltaValue);
		else
			consumeButton.counter.IncreaseValue(deltaValue);
	}
}
