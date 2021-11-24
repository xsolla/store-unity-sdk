using System;
using UnityEngine;
using Xsolla.Core;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public partial class BattlePassItem : MonoBehaviour
    {
		[SerializeField] private Image ItemImage;
		[SerializeField] private GameObject LoadingCircle;
		[SerializeField] private GameObject QuantityLabel;
		[SerializeField] private Text QuantityText;
		[SerializeField] private GameObject SelectionObject;
		[SerializeField] private BattlePassImageRatioFitter ImageRatioFitter;

		private static IEnumerable<ItemModel> _allCatalogItems;
		private static IEnumerable<ItemModel> AllCatalogItems
		{
			get
			{
				if (_allCatalogItems == null)
				{
					IEnumerable<ItemModel> allCurrencies = UserCatalog.Instance.VirtualCurrencies.Cast<ItemModel>();
					IEnumerable<ItemModel> allItems = UserCatalog.Instance.AllItems.Cast<ItemModel>();
					_allCatalogItems = allCurrencies.Concat(allItems);
				}

				return _allCatalogItems;
			}
		}

		public void Initialize(BattlePassItemDescription itemDescription)
		{
			ItemDescription = itemDescription;

			if (itemDescription == null)
				SetEmpty();
			else
				SetItem(itemDescription);
		}

		private void SetEmpty()
		{
			BackgroundImage.color = BackgroundEmpty;
			SelectionObject.SetActive(false);
			LoadingCircle.SetActive(false);
			ItemButton.gameObject.SetActive(false);
		}

		private void SetItem(BattlePassItemDescription itemDescription)
		{
			if (TryCompleteItemDescription(itemDescription))
				LoadImage(itemDescription.ItemCatalogModel.ImageUrl, SetItemImage);
			else
				Debug.LogError(string.Format("Could not find corresponding ItemModel for item with sku: '{0}'", itemDescription.Sku));

			if (itemDescription.Quantity > 1)
				SetQuantity(itemDescription.Quantity);

			if (itemDescription.IsFinal)
				SetFinal();
		}

		//That's not very architecture-wise but it will decrease future number of operations for OnItemSelected
		private bool TryCompleteItemDescription(BattlePassItemDescription itemDescription)
		{
			foreach (ItemModel itemModel in AllCatalogItems)
			{
				if (itemModel.Sku == itemDescription.Sku)
				{
					itemDescription.ItemCatalogModel = itemModel;
					return true;
				}
			}
			//else
			return false;
		}

		private void LoadImage(string url, Action<Sprite> loadCallback)
		{
			if (!string.IsNullOrEmpty(url))
				ImageLoader.Instance.GetImageAsync(url, (_, image) => loadCallback(image));
			else
				Debug.LogError("Inventory item have no image!");
		}

		private void SetItemImage(Sprite image)
		{
			LoadingCircle.SetActive(false);
			ItemImage.gameObject.SetActive(true);
			ItemImage.sprite = image;
			_itemImageContainer.Image = image;
		}

		private void SetQuantity(int quantity)
		{
			QuantityText.text = quantity.ToString();
			QuantityLabel.SetActive(true);
		}

		private void SetFinal()
		{
			HighLight.SetActive(true);
			ImageRatioFitter.SetAspectRatio(_itemImageContainer);
		}
	}
}
