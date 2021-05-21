using System;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Store;

namespace Xsolla.Demo
{
	public partial class DemoImplementation : MonoBehaviour, IStoreDemoImplementation
	{
		public void GetCartItems(Action<UserCartModel> onSuccess, Action<Error> onError)
		{
			XsollaStore.Instance.GetCartItems(XsollaSettings.StoreProjectId, cart =>
			{
				var cartModel = new UserCartModel();
				var cartItems = cart.items;
				foreach (var cartItem in cartItems)
				{
					var realPrice = GetRealPrice(cartItem.price, out var realPriceWithoutDiscount);
					cartModel.GetCartItems().Add(new UserCartItem()
					{
						Sku = cartItem.sku,
						Name = cartItem.name,
						Price = realPrice?.Value ?? 0F,
						PriceWithoutDiscount = realPriceWithoutDiscount?.Value ?? 0F,
						Currency = realPrice?.Key,
						ImageUrl = cartItem.image_url,
						Quantity = cartItem.quantity
					});
				}
				onSuccess?.Invoke(cartModel);
			}, WrapErrorCallback(onError));
		}

		public void UpdateCartItem(string sku, int quantity, Action onSuccess, Action<Error> onError = null)
		{
			XsollaStore.Instance.UpdateItemInCart(XsollaSettings.StoreProjectId, sku, quantity, onSuccess, onError);
		}

		public void RemoveCartItem(string sku, Action onSuccess, Action<Error> onError = null)
		{
			XsollaStore.Instance.RemoveItemFromCart(XsollaSettings.StoreProjectId, sku, onSuccess, onError);
		}
	}
}