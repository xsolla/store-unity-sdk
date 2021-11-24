using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public partial class InventoryItemUI : MonoBehaviour
	{
		[SerializeField] private Image itemImage;
		[SerializeField] private GameObject loadingCircle;
		[SerializeField] private Text itemName;
		[SerializeField] private Text itemDescription;
		[SerializeField] private GameObject itemQuantityImage;
		[SerializeField] private Text itemQuantityText;
		[SerializeField] private ConsumeButton consumeButton;
		[SerializeField] private Text purchasedStatusText;
		[SerializeField] private GameObject remainingTimeObject;
		[SerializeField] private Text remainingTimeTimeText;
		[SerializeField] private GameObject expiredTimeObject;
		[SerializeField] private Text expiredTimeText;
		[SerializeField] private bool shortSubsctiptionTimeFormat = false;
#pragma warning disable 0414
		[SerializeField] private SimpleTextButton renewSubscriptionButton;
#pragma warning restore 0414

		private ItemModel _itemInformation;

		public event Action<ItemModel> OnInitialized;

		private void Awake()
		{
			DisableConsumeButton();
		}

		public void Initialize(ItemModel itemInformation)
		{
			_itemInformation = itemInformation;

			if (itemName != null)
				itemName.text = _itemInformation.Name;
			if (itemDescription != null)
				itemDescription.text = _itemInformation.Description;

			LoadImage(_itemInformation.ImageUrl);

			if (_itemInformation.IsSubscription())
				AttachRenewSubscriptionHandler();

			if (OnInitialized != null)
				OnInitialized.Invoke(itemInformation);
		}

		partial void AttachRenewSubscriptionHandler();

		private void LoadImage(string url)
		{
			if (!string.IsNullOrEmpty(url))
				ImageLoader.Instance.GetImageAsync(url, LoadImageCallback);
			else
			{
				Debug.LogError(string.Format("Inventory item with sku = '{0}' have not image!", _itemInformation.Sku));
				LoadImageCallback(string.Empty, null);
			}
		}

		private void LoadImageCallback(string url, Sprite image)
		{
			if (!itemImage)
				return;
			
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
			UserSubscriptionModel model;
			if (TryDowncastTo<UserSubscriptionModel>(_itemInformation, out model))
			{
				if (model.Status != UserSubscriptionModel.SubscriptionStatusType.None && model.Expired.HasValue)
				{
					var isExpired = model.Status == UserSubscriptionModel.SubscriptionStatusType.Expired || model.Expired <= DateTime.Now;
					if (isExpired)
						EnableExpiredTimeText(GetPassedTime(model.Expired.Value));
					else
						EnableRemainingTimeText(GetRemainingTime(model.Expired.Value));
				}
				else
					EnablePurchasedStatusText(isPurchased: false);
			}
		}

		private void EnableRemainingTimeText(string text)
		{
			if (remainingTimeObject != null)
				remainingTimeObject.SetActive(true);
			if (expiredTimeObject != null)
				expiredTimeObject.SetActive(false);
			if (renewSubscriptionButton != null)
				renewSubscriptionButton.gameObject.SetActive(false);

			if (remainingTimeTimeText != null)
				remainingTimeTimeText.text = text;
		}

		private void EnableExpiredTimeText(string text)
		{
			if (remainingTimeObject != null)
				remainingTimeObject.SetActive(false);
			if (expiredTimeObject != null)
				expiredTimeObject.SetActive(true);
			if (renewSubscriptionButton != null)
				renewSubscriptionButton.gameObject.SetActive(true);

			if (expiredTimeText != null)
				expiredTimeText.text = string.Format("Expired {0}", text);
		}

		private string GetRemainingTime(DateTime expiredDateTime)
		{
			var timeLeft = expiredDateTime - DateTime.Now;
			StartCoroutine(RemainingTimeCoroutine(timeLeft.TotalSeconds > 60 ? 60 : 1));
			var suffix = shortSubsctiptionTimeFormat ? string.Empty : "remaining";
			if (timeLeft.TotalDays >= 30)
				return string.Format("{0} month{1} {2}", (int)(timeLeft.TotalDays / 30), (timeLeft.TotalDays > 60 ? "s" : ""),suffix);
			if (timeLeft.TotalDays > 1)
				return string.Format("{0} day{1} {2}", (uint)(timeLeft.TotalDays), (timeLeft.TotalDays > 1 ? "s" : ""), suffix);
			if (timeLeft.TotalHours > 1)
				return string.Format("{0} hour{1} {2}", (uint)(timeLeft.TotalHours), (timeLeft.TotalHours > 1 ? "s" : ""), suffix);
			if (timeLeft.TotalMinutes > 1)
				return string.Format("{0} minute{1} {2}", (uint)(timeLeft.TotalMinutes), (timeLeft.TotalMinutes > 1 ? "s" : ""), suffix);
			return string.Format("{0} second{1} {2}", (uint)(timeLeft.TotalSeconds), (timeLeft.TotalSeconds > 1 ? "s" : ""), suffix);
		}

		private string GetPassedTime(DateTime expiredDateTime)
		{
			var timePassed = DateTime.Now - expiredDateTime;
			StartCoroutine(RemainingTimeCoroutine(timePassed.TotalSeconds > 60 ? 60 : 1));
			var suffix = shortSubsctiptionTimeFormat ? string.Empty : "ago";
			if (timePassed.TotalDays >= 30)
				return string.Format("{0} month{1} {2}", (int)(timePassed.TotalDays / 30), (timePassed.TotalDays > 60 ? "s" : ""), suffix);
			if (timePassed.TotalDays > 1)
				return string.Format("{0} day{1} {2}", (uint)(timePassed.TotalDays), (timePassed.TotalDays > 1 ? "s" : ""), suffix);
			if (timePassed.TotalHours > 1)
				return string.Format("{0} hour{1} {2}", (uint)(timePassed.TotalHours), (timePassed.TotalHours > 1 ? "s" : ""), suffix);
			if (timePassed.TotalMinutes > 1)
				return string.Format("{0} minute{1} {2}", (uint)(timePassed.TotalMinutes), (timePassed.TotalMinutes > 1 ? "s" : ""), suffix);
			return string.Format("{0} second{1} {2}", (uint)(timePassed.TotalSeconds), (timePassed.TotalSeconds > 1 ? "s" : ""), suffix);
		}

		private IEnumerator RemainingTimeCoroutine(float waitSeconds)
		{
			yield return new WaitForSeconds(waitSeconds);
			RefreshUi();
		}

		private void DrawConsumableVirtualItem()
		{
			InventoryItemModel model;
			if (TryDowncastTo<InventoryItemModel>(_itemInformation, out model))
			{
				DrawItemsCount(model);
				EnableConsumeButton();
			}
		}

		private void DrawNonConsumableVirtualItem()
		{
			InventoryItemModel model;
			if (TryDowncastTo<InventoryItemModel>(_itemInformation, out model))
			{
				DrawItemsCount(model);
				EnablePurchasedStatusText(isPurchased: true);
			}
		}

		private void DrawItemsCount(InventoryItemModel model)
		{
			if (model.RemainingUses == null || itemQuantityImage == null || itemQuantityText == null) return;
			EnableQuantityImage();
			itemQuantityText.text = model.RemainingUses.Value.ToString(); 
		}

		private void EnableQuantityImage()
		{
			if (itemQuantityImage != null)
				itemQuantityImage.SetActive(true);
		}

		private void DisableQuantityImage()
		{
			if (itemQuantityImage != null)
				itemQuantityImage.SetActive(false);
		}

		private void EnablePurchasedStatusText(bool isPurchased)
		{
			if (purchasedStatusText != null)
			{
				purchasedStatusText.text = isPurchased ? "Purchased" : "Not purchased";
				purchasedStatusText.gameObject.SetActive(true);
			}
		}

		private void DisablePurchasedStatusText()
		{
			if (purchasedStatusText != null)
				purchasedStatusText.gameObject.SetActive(false);
		}

		private void DisableExpirationText()
		{
			if (remainingTimeObject != null)
				remainingTimeObject.SetActive(false);
			if (expiredTimeObject != null)
				expiredTimeObject.SetActive(false);
			if (renewSubscriptionButton != null)
				renewSubscriptionButton.gameObject.SetActive(false);
		}

		private void EnableConsumeButton()
		{
			if (consumeButton != null)
			{
				consumeButton.gameObject.SetActive(true);
				consumeButton.onClick = ConsumeHandler;
				if (consumeButton.counter < 1)
					consumeButton.counter.IncreaseValue(1 - consumeButton.counter.GetValue());
				consumeButton.counter.ValueChanged += Counter_ValueChanged;
			}
		}

		private void DisableConsumeButton()
		{
			if (consumeButton != null)
			{
				consumeButton.counter.ValueChanged -= Counter_ValueChanged;
				consumeButton.gameObject.SetActive(false);
			}
		}

		private void ConsumeHandler()
		{
			InventoryItemModel model;
			if (TryDowncastTo<InventoryItemModel>(_itemInformation, out model))
			{
				DisableConsumeButton();
				DemoInventory.Instance.ConsumeInventoryItem(model, consumeButton.counter.GetValue(),
				_ =>
				{
					UserInventory.Instance.Refresh(onError: StoreDemoPopup.ShowError);
					consumeButton.counter.ResetValue();
				}, _ => EnableConsumeButton());
			}
		}

		private void Counter_ValueChanged(int newValue)
		{
			InventoryItemModel model;
			if (TryDowncastTo<InventoryItemModel>(_itemInformation, out model))
			{
				if (newValue > model.RemainingUses)
				{
					var delta = (int) model.RemainingUses - newValue;
					StartCoroutine(ChangeConsumeQuantityCoroutine(delta));
				}
				else
				{
					if (newValue == 0)
						StartCoroutine(ChangeConsumeQuantityCoroutine(1));
				}
			}
		}

		private IEnumerator ChangeConsumeQuantityCoroutine(int deltaValue)
		{
			yield return new WaitForEndOfFrame();
			if (deltaValue < 0)
				consumeButton.counter.DecreaseValue(-deltaValue);
			else
				consumeButton.counter.IncreaseValue(deltaValue);
		}

		private bool TryDowncastTo<T>(ItemModel itemModel, out T result) where T : ItemModel
		{
			if (itemModel is T)
			{
				result = (T)itemModel;
				return true;
			}
			else
			{
				//TEXTREVIEW
				Debug.LogError(string.Format("Item model was incorrect for item with sku '{0}', expected type '{1}'", itemModel.Sku, typeof(T)));
				result = null;
				return false;
			}
		}
	}
}
