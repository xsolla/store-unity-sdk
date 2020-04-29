using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Core.Popup;
using Xsolla.Store;

public class ItemUI : MonoBehaviour
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
	SimpleTextButton buyButton;
	[SerializeField]
	AddToCartButton addToCartButton;

	StoreItem _itemInformation;
	GroupsController _groupsController;

	void Awake()
	{
		_groupsController = FindObjectOfType<GroupsController>();

		buyButton.onClick = (() => {
			if (_itemInformation.virtual_prices.Any()) {
				StoreDemoPopup.ShowConfirm(
					() => {
						XsollaStore.Instance.ItemPurchaseForVirtualCurrency(
							XsollaSettings.StoreProjectId,
							_itemInformation.sku,
							GetVirtualPrice().sku,
							data => VirtualCurrencyPurchaseComplete(_itemInformation.name),
							StoreDemoPopup.ShowError);
					});
			} else {
				bool isItemVirtualCurrency = _groupsController.GetSelectedGroup().Name == Constants.CurrencyGroupName;
				XsollaStore.Instance.ItemPurchase(XsollaSettings.StoreProjectId, _itemInformation.sku, data => {
					XsollaStore.Instance.OpenPurchaseUi(data);
					XsollaStore.Instance.ProcessOrder(XsollaSettings.StoreProjectId, data.order_id, () =>
					{
						if (isItemVirtualCurrency)
							UserInventory.Instance.UpdateVirtualCurrencyBalance();
						else
							UserInventory.Instance.UpdateVirtualItems();
						StoreDemoPopup.ShowSuccess();
					});
				}, StoreDemoPopup.ShowError);
			}
		});

		addToCartButton.onClick = (bSelected => {
			if (bSelected) {
				UserCart.Instance.AddItem(_itemInformation);
			} else {
				UserCart.Instance.RemoveItem(_itemInformation);
			}
		});
	}

	void VirtualCurrencyPurchaseComplete(string currencyName)
	{
		UserInventory.Instance.UpdateVirtualItems();
		UserInventory.Instance.UpdateVirtualCurrencyBalance();
		StoreDemoPopup.ShowSuccess($"You are purchased `{currencyName}`!");
	}

	public void Initialize(StoreItem itemInformation)
	{
		_itemInformation = itemInformation;
		string currency;
		string price;
		string text = "";

		if (_itemInformation.virtual_prices.Any()) {
			StoreItem.VirtualPrice virtualPrice = GetVirtualPrice();
			text = FormatVirtualCurrencyBuyButtonText(virtualPrice.name, virtualPrice.amount);
			addToCartButton.gameObject.SetActive(false);
		} else {
			if (_itemInformation.price != null) {
				price = _itemInformation.price.amount.ToString("F2");
				if (_itemInformation.price.currency == RegionalCurrency.CurrencyCode) {
					currency = RegionalCurrency.CurrencySymbol;
				} else {
					currency = RegionalCurrency.GetCurrencySymbol(_itemInformation.price.currency);
					if (string.IsNullOrEmpty(currency)) {
						currency = _itemInformation.price.currency;// if there is no symbol for specified currency then display currency code instead
					}
				}
				text = FormatBuyButtonText(currency, price);
			}
		}
		buyButton.Text = text;
		itemName.text = _itemInformation.name;
		itemDescription.text = _itemInformation.description;

		gameObject.name = "Item_" + _itemInformation.name.Replace(" ", "");
		ImageLoader.Instance.GetImageAsync(_itemInformation.image_url, LoadImageCallback);
	}

	private void LoadImageCallback(string url, Sprite image)
	{
		loadingCircle.SetActive(false);
		itemImage.sprite = image;
	}
	
	public string GetSku()
	{
		return _itemInformation.sku;
	}
	
	public void Lock()
	{
		buyButton.Text = "Purchased";
		buyButton.Lock();
		addToCartButton.gameObject.SetActive(false);
	}

	StoreItem.VirtualPrice GetVirtualPrice()
	{
		List<StoreItem.VirtualPrice> prices = _itemInformation.virtual_prices.ToList();
		return (prices.Count(p => p.is_default) > 0) ? prices.First(p => p.is_default) : prices.First();
	}

	string FormatBuyButtonText(string currency, string price)
	{
		return $"BUY FOR {currency}{price}";
	}

	string FormatVirtualCurrencyBuyButtonText(string currency, string price)
	{
		return string.Format("BUY FOR" + Environment.NewLine + "{0} {1}", price, currency);
	}

	public void Refresh()
	{
		if (addToCartButton.gameObject.activeInHierarchy)
			addToCartButton.Select(UserCart.Instance.Contains(_itemInformation));
	}
}
