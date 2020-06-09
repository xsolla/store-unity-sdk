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
	[SerializeField]
	GameObject SubscriptionBadge;
	[SerializeField]
	Text SubscriptionPeriod;

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
						{
							UserInventory.Instance.UpdateVirtualCurrencyBalance();
						}
						else if (_itemInformation.IsSubscription())
						{
							UserSubscriptions.Instance.UpdateSupscriptions();
						}
						else
						{
							UserInventory.Instance.UpdateVirtualItems();
						}
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
		if (_itemInformation.IsSubscription())
			UserSubscriptions.Instance.UpdateSupscriptions();
		else
			UserInventory.Instance.UpdateVirtualItems();

		UserInventory.Instance.UpdateVirtualCurrencyBalance();
		StoreDemoPopup.ShowSuccess($"You are purchased `{currencyName}`!");
	}

	public void Initialize(StoreItem itemInformation)
	{
		_itemInformation = itemInformation;
		string text = "";
		if (_itemInformation.IsSubscription())
		{
			var expPeriod = _itemInformation.inventory_options.expiration_period;
			var pluralSuffix = expPeriod.value > 1 ? "s" : string.Empty;
			SubscriptionPeriod.text = $"{expPeriod.value} {expPeriod.type}{pluralSuffix}";
			SubscriptionBadge.SetActive(true);
		}
		if (_itemInformation.virtual_prices.Any()) {
			VirtualPrice virtualPrice = GetVirtualPrice();
			text = FormatVirtualCurrencyBuyButtonText(virtualPrice.name, virtualPrice.GetAmount());
			addToCartButton.gameObject.SetActive(false);
		} else {
			if (_itemInformation.price != null) {
				string currency;
				if (_itemInformation.price.currency == RegionalCurrency.CurrencyCode) {
					currency = RegionalCurrency.CurrencySymbol;
				} else {
					currency = RegionalCurrency.GetCurrencySymbol(_itemInformation.price.currency);
					if (string.IsNullOrEmpty(currency)) {
						currency = _itemInformation.price.currency;// if there is no symbol for specified currency then display currency code instead
					}
				}
				Price price = _itemInformation.price;
				text = FormatBuyButtonText(currency, price.GetAmount());
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

	VirtualPrice GetVirtualPrice()
	{
		List<VirtualPrice> prices = _itemInformation.virtual_prices.ToList();
		return (prices.Count(p => p.is_default) > 0) ? prices.First(p => p.is_default) : prices.First();
	}

	private string FormatBuyButtonText(string currency, float price)
	{
		return $"BUY FOR {currency}{price:F2}";
	}

	private string FormatVirtualCurrencyBuyButtonText(string currency, uint price)
	{
		return "BUY FOR" + Environment.NewLine + $"{price:D} {currency}";
	}

	public void Refresh()
	{
		if (addToCartButton.gameObject.activeInHierarchy)
			addToCartButton.Select(UserCart.Instance.Contains(_itemInformation));
	}
}
