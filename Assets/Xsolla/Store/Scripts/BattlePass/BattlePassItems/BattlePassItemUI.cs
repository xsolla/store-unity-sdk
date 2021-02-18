using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

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

		public void Initialize(BattlePassItemDescription itemDescription)
		{
			if (itemDescription == null)
				SetEmpty();
			else
				SetItem(itemDescription);
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

			if (itemDescription.Quantity > 0)
				SetQuantity(itemDescription.Quantity);

			//TODO
			// <== TOBECONTINUED = (a familiar tune is playing)
		}

		private bool TryGetImageUrl(string itemSku, out string imageUrl)
		{
			var item = UserCatalog.Instance.AllItems.Find(catalogItem => catalogItem.Sku == itemSku);

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
