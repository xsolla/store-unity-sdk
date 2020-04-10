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
	StoreController _storeController;
	GroupsController _groupsController;

	Sprite _itemImage;

	void Awake()
	{
		_storeController = FindObjectOfType<StoreController>();
		_groupsController = FindObjectOfType<GroupsController>();

		var cartGroup = FindObjectOfType<CartGroupUI>();

		buyButton.onClick = (() => {
			if (_itemInformation.virtual_prices.Any()) {
				_storeController.ShowConfirm(
					() => {
						XsollaStore.Instance.ItemPurchaseForVirtualCurrency(
							XsollaSettings.StoreProjectId,
							_itemInformation.sku,
							GetVirtualPrice().sku,
							(PurchaseData data) => VirtualCurrencyPurchaseComplete(_itemInformation.name),
							_storeController.ShowError,
							null);
					}, null);
			} else {
				bool isItemVirtualCurrency = _groupsController?.GetSelectedGroup().Name == Constants.CurrencyGroupName;
				XsollaStore.Instance.ItemPurchase(XsollaSettings.StoreProjectId, _itemInformation.sku, data => {
					XsollaStore.Instance.OpenPurchaseUi(data);
					_storeController.ProcessOrder(data.order_id, () => {
						if (isItemVirtualCurrency)
							_storeController.RefreshVirtualCurrencyBalance();
					});
				}, _storeController.ShowError);
			}
		});

		addToCartButton.onClick = (bSelected => {
			if (bSelected) {
				UserCart.Instance.AddItem(_itemInformation);
				//_storeController.CartModel.AddCartItem(_itemInformation);
				cartGroup.IncreaseCounter();
			} else {
				UserCart.Instance.RemoveItem(_itemInformation);
				//_storeController.CartModel.RemoveCartItem(_itemInformation.sku);
				cartGroup.DecreaseCounter();
			}
		});
	}

	void VirtualCurrencyPurchaseComplete(string itemName)
	{
		_storeController.RefreshInventory();
		_storeController.RefreshVirtualCurrencyBalance();
		PopupFactory.Instance.CreateSuccess().
			SetMessage(String.Format("You are purchased `{0}`!", itemName));
	}

	public void Initialize(StoreItem itemInformation)
	{
		_itemInformation = itemInformation;
		string currency;
		string price;
		string text = "";

		if (_itemInformation.virtual_prices.Any()) {
			StoreItem.VirtualPrice virtualPrice = GetVirtualPrice();
			price = virtualPrice.amount;
			currency = virtualPrice.name;
			text = FormatVirtualCurrencyBuyButtonText(currency, price);

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
	}

	public string GetSku()
	{
		return _itemInformation.sku;
	}

	public bool IsConsumable()
	{
		return _itemInformation.inventory_options.consumable != null;
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
		return string.Format("BUY FOR {0}{1}", currency, price);
	}

	string FormatVirtualCurrencyBuyButtonText(string currency, string price)
	{
		return string.Format("BUY FOR" + System.Environment.NewLine + "{0} {1}", price, currency);
	}

	void OnEnable()
	{
		if (_itemImage == null && !string.IsNullOrEmpty(_itemInformation.image_url)) {
			ImageLoader.Instance.GetImageAsync(_itemInformation.image_url, LoadImageCallback);
		}
	}

	void LoadImageCallback(string url, Sprite image)
	{
		loadingCircle.SetActive(false);
		itemImage.sprite = _itemImage = image;
	}

	public void Refresh()
	{
		if (addToCartButton.gameObject.activeInHierarchy)
			addToCartButton.Select(_storeController.CartModel.CartItems.ContainsKey(_itemInformation.sku));
	}
}
