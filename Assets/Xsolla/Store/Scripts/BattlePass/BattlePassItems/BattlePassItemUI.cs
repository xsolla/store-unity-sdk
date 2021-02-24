using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using System.Linq;
using System.Collections.Generic;

namespace Xsolla.Demo
{
	public class BattlePassItemUI : MonoBehaviour
    {
		[SerializeField] private Image BackgroundImage = default;
		[SerializeField] private Sprite Background = default;
		[SerializeField] private Sprite BackgroundEmpty = default;
		[SerializeField] private Sprite BackgroundCurrentLevel = default;
		[SerializeField] private Image ItemImage = default;
		[SerializeField] private GameObject LoadingCircle = default;
		[SerializeField] private GameObject QuantityLabel = default;
		[SerializeField] private Text QuantityText = default;
		[SerializeField] private GameObject SelectionObject = default;

		[SerializeField] private GameObject[] StateObjects = default;

		private BattlePassItemDescription _currentItemDescription;

		public void Initialize(BattlePassItemDescription itemDescription)
		{
			_currentItemDescription = itemDescription;

			if (itemDescription == null)
				SetEmpty();
			else
				SetItem(itemDescription);
		}

		public void SetCurrent(bool isCurrent)
		{
			if (isCurrent)
				BackgroundImage.sprite = BackgroundCurrentLevel;
			else
			{
				if (_currentItemDescription != null)
					BackgroundImage.sprite = Background;
				else
					BackgroundImage.sprite = BackgroundEmpty;
			}
		}

		public void SetState(BattlePassItemState itemState)
		{
			if (_currentItemDescription == null)
			{
				Debug.LogWarning("Attempt to set item state for empty item");
				return;
			}

			foreach (var stateObject in StateObjects)
			{
				if (stateObject.activeSelf)
					stateObject.SetActive(false);
			}

			StateObjects[(int)itemState].SetActive(true);
		}

		private void SetEmpty()
		{
			BackgroundImage.sprite = BackgroundEmpty;
			SelectionObject.SetActive(false);
			LoadingCircle.SetActive(false);
		}

		private void SetItem(BattlePassItemDescription itemDescription)
		{
			if (TryGetImageUrl(itemDescription.Sku, out string imageUrl))
				LoadImage(imageUrl, SetItemImage);
			else
				Debug.LogError($"Could not find image url for item with sku: '{itemDescription.Sku}'");

			if (itemDescription.Quantity > 1)
				SetQuantity(itemDescription.Quantity);

			//TODO
			// <== TOBECONTINUED = (a familiar tune is playing)
		}

		private bool TryGetImageUrl(string itemSku, out string imageUrl)
		{
			var item = default(ItemModel);

			IEnumerable<ItemModel> allCurrencies = UserCatalog.Instance.VirtualCurrencies;
			IEnumerable<ItemModel> allItems = UserCatalog.Instance.AllItems;

			foreach (ItemModel catalogItem in allCurrencies.Concat(allItems))
			{
				if (catalogItem.Sku == itemSku)
				{
					item = catalogItem;
					break;
				}
			}

			if (item != null)
			{
				imageUrl = item.ImageUrl;
				return true;
			}
			else
			{
				imageUrl = default(string);
				return false;
			}
		}

		private void LoadImage(string url, Action<Sprite> loadCallback)
		{
			if (!string.IsNullOrEmpty(url))
				ImageLoader.Instance.GetImageAsync(url, (_, image) => loadCallback(image));
			else
				Debug.LogError($"Inventory item with sku = have not image!");
		}

		private void SetItemImage(Sprite image)
		{
			LoadingCircle.SetActive(false);
			ItemImage.gameObject.SetActive(true);
			ItemImage.sprite = image;
		}

		private void SetQuantity(int quantity)
		{
			QuantityLabel.SetActive(true);
			QuantityText.text = quantity.ToString();
		}
	}
}
