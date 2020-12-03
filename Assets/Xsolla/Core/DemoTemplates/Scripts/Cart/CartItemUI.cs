using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class CartItemUI : MonoBehaviour
	{
		[SerializeField]
		Image itemImage = default;
		[SerializeField]
		Text itemName = default;
		[SerializeField]
		Text itemPrice = default;
		[SerializeField]
		Text itemPriceWithoutDiscount = default;
		[SerializeField]
		GameObject discountIco = default;

		[SerializeField]
		SimpleButton addButton = default;
		[SerializeField]
		SimpleButton removeButton = default;
		[SerializeField]
		SimpleButton deleteButton = default;

		[SerializeField]
		Text itemQuantity = default;

		private UserCartItem _cartItem;

		private void Start()
		{
			addButton.onClick = () => UserCart.Instance.IncreaseCountOf(_cartItem.Item);
			removeButton.onClick = () => UserCart.Instance.DecreaseCountOf(_cartItem.Item);
			deleteButton.onClick = () => UserCart.Instance.RemoveItem(_cartItem.Item);

			UserCart.Instance.RemoveItemEvent += RemoveItemHandler;
			UserCart.Instance.UpdateItemEvent += UpdateItemHandler;
		}

		private void OnDestroy()
		{
			UserCart.Instance.RemoveItemEvent -= RemoveItemHandler;
			UserCart.Instance.UpdateItemEvent -= UpdateItemHandler;
		}

		private void RemoveItemHandler(UserCartItem item)
		{
			if (!item.Equals(_cartItem)) return;
			Destroy(gameObject, 0.1F);
		}

		private void UpdateItemHandler(UserCartItem item, int deltaCount)
		{
			if (!item.Equals(_cartItem)) return;
			itemQuantity.text = _cartItem.Quantity.ToString();
		}

		public void Initialize(UserCartItem cartItem)
		{
			_cartItem = cartItem;

			itemPrice.text = PriceFormatter.FormatPrice(_cartItem.Currency, _cartItem.Price);
			itemName.text = _cartItem.Item.Name;
			itemQuantity.text = _cartItem.Quantity.ToString();

			var priceWithoutDiscount = _cartItem.PriceWithoutDiscount;

			if (priceWithoutDiscount != default(float) && priceWithoutDiscount != _cartItem.Price)
			{
				itemPriceWithoutDiscount.text = PriceFormatter.FormatPrice(_cartItem.Currency, priceWithoutDiscount);
				discountIco.SetActive(true);
			}
			else
			{
				itemPriceWithoutDiscount.text = string.Empty;
				discountIco.SetActive(false);
			}
		
			if (itemImage.sprite != null)
			{
				if (!string.IsNullOrEmpty(_cartItem.ImageUrl))
				{
					ImageLoader.Instance.GetImageAsync(_cartItem.ImageUrl, LoadImageCallback);
				}
				else
				{
					Debug.LogError($"Cart item with sku = '{_cartItem.Sku}' without image!");
				}
			}
		}

		void LoadImageCallback(string url, Sprite image)
		{
			itemImage.sprite = image;
		}
	}
}
