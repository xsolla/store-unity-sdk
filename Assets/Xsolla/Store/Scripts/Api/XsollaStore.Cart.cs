using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		private const string URL_CART_CREATE_NEW = BASE_STORE_API_URL + "/cart";

		private const string URL_CART_CURRENT_ITEM_UPDATE = BASE_STORE_API_URL + "/cart/item/{1}";
		private const string URL_CART_SPECIFIC_ITEM_UPDATE = BASE_STORE_API_URL + "/cart/{1}/item/{2}";

		private const string URL_CART_CURRENT_ITEM_REMOVE = BASE_STORE_API_URL + "/cart/item/{1}";
		private const string URL_CART_SPECIFIC_ITEM_REMOVE = BASE_STORE_API_URL + "/cart/{1}/item/{2}";

		private const string URL_CART_GET_ITEMS = BASE_STORE_API_URL + "/cart/{1}";

		private const string URL_CART_CURRENT_CLEAR = BASE_STORE_API_URL + "/cart/clear";
		private const string URL_CART_SPECIFIC_CLEAR = BASE_STORE_API_URL + "/cart/{1}/clear";

		private const string URL_CART_CURRENT_FILL = BASE_STORE_API_URL + "/cart/fill";
		private const string URL_CART_SPECIFIC_FILL = BASE_STORE_API_URL + "/cart/{1}/fill";

		private const string URL_REDEEM_PROMOCODE = BASE_STORE_API_URL + "/promocode/redeem";
		private const string URL_GET_PROMOCODE_REWARD = BASE_STORE_API_URL + "/promocode/code/{1}/rewards";

		/// <summary>
		/// Creates a new cart on the Xsolla side.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get current user's cart</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/cart/get-user-cart/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void CreateNewCart(string projectId, [NotNull] Action<Cart> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_CART_CREATE_NEW, projectId);
			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.CreateCartErrors);
		}

		/// <summary>
		/// Fills the current cart with items. If the cart already has an item,
		/// the existing item will be replaced by the given value.
		/// </summary>
		/// <remarks> Swagger method name:<c>Fill cart with items</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/cart/cart-fill/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="items">Items for purchase.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void FillCart(string projectId, List<CartFillItem> items, [NotNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_CART_CURRENT_FILL, projectId);
			var entity = new CartFillEntity {items = items};
			WebRequestHelper.Instance.PutRequest(SdkType.Store, url, entity, WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.CreateCartErrors);
		}

		/// <summary>
		/// Fills the specific cart with items. If the cart already has an item,
		/// the existing item will be replaced by the given value.
		/// </summary>
		/// <remarks> Swagger method name:<c>Fill specific cart with items</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/cart/cart-fill-by-id/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="items">Items for purchase.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void FillCart(string projectId, string cartId, List<CartFillItem> items, [NotNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_CART_SPECIFIC_FILL, projectId, cartId);
			var entity = new CartFillEntity {items = items};
			WebRequestHelper.Instance.PutRequest(SdkType.Store, url, entity, WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.CreateCartErrors);
		}

		/// <summary>
		/// Updates an existing item or creates the one in the current cart.
		/// </summary>
		/// <remarks> Swagger method name:<c>Update cart line item from current cart</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/cart/put-item/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="itemSku">SKU of item for purchase.</param>
		/// <param name="quantity">Quantity of purchased item.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="CreateNewCart"/>
		public void UpdateItemInCart(string projectId, string itemSku, int quantity, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_CART_CURRENT_ITEM_UPDATE, projectId, itemSku);
			var jsonObject = new Quantity {quantity = quantity};
			WebRequestHelper.Instance.PutRequest<Quantity>(SdkType.Store, url, jsonObject, WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.AddToCartCartErrors);
		}

		/// <summary>
		/// Updates an existing item or creates the one in the specific cart.
		/// </summary>
		/// <remarks> Swagger method name:<c>Update cart line item by cart ID</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/cart/put-item-by-cart-id/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="itemSku">SKU of item for purchase.</param>
		/// <param name="quantity">Quantity of purchased items.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="CreateNewCart"/>
		public void UpdateItemInCart(string projectId, string cartId, string itemSku, int quantity, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_CART_SPECIFIC_ITEM_UPDATE, projectId, cartId, itemSku);
			var jsonObject = new Quantity {quantity = quantity};
			WebRequestHelper.Instance.PutRequest<Quantity>(SdkType.Store, url, jsonObject, WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.AddToCartCartErrors);
		}

		/// <summary>
		/// Deletes all current cart items.
		/// </summary>
		/// <remarks> Swagger method name:<c>Delete all cart line items from current cart</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/cart/cart-clear/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void ClearCart(string projectId, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_CART_CURRENT_CLEAR, projectId);
			WebRequestHelper.Instance.PutRequest<Quantity>(SdkType.Store, url, null, WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.AddToCartCartErrors);
		}

		/// <summary>
		/// Deletes all specific cart items.
		/// </summary>
		/// <remarks> Swagger method name:<c>Delete all cart line items by cart ID</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/cart/cart-clear-by-id/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void ClearCart(string projectId, string cartId, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_CART_SPECIFIC_CLEAR, projectId, cartId);
			WebRequestHelper.Instance.PutRequest<Quantity>(SdkType.Store, url, null, WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.AddToCartCartErrors);
		}

		/// <summary>
		/// Returns a user’s cart by ID.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get cart by ID</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/cart/get-cart-by-id/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Defines localization of item's text fields.</param>
		/// <param name="currency">Defines currency of item's price.</param>
		public void GetCartItems(string projectId, string cartId, [NotNull] Action<CartItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, [CanBeNull] string currency = null)
		{
			var url = string.Format(URL_CART_GET_ITEMS, projectId, cartId);
			var localeParam = GetLocaleUrlParam(locale);
			var currencyParam = GetCurrencyUrlParam(currency);
			url = ConcatUrlAndParams(url, localeParam, currencyParam);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.GetCartItemsErrors);
		}

		/// <summary>
		/// Delete item from the current cart.
		/// </summary>
		/// <remarks> Swagger method name:<c>Delete cart line item from current cart</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/cart/delete-item/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="itemSku">Item SKU to delete.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void RemoveItemFromCart(string projectId, string itemSku, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_CART_CURRENT_ITEM_REMOVE, projectId, itemSku);
			WebRequestHelper.Instance.DeleteRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.DeleteFromCartErrors);
		}

		/// <summary>
		/// Deletes item from the specific cart.
		/// </summary>
		/// <remarks> Swagger method name:<c>Delete cart line item by cart ID</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/cart/delete-item-by-cart-id/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="itemSku">Item SKU to delete.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void RemoveItemFromCart(string projectId, string cartId, string itemSku, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_CART_SPECIFIC_ITEM_REMOVE, projectId, cartId, itemSku);
			WebRequestHelper.Instance.DeleteRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.DeleteFromCartErrors);
		}

		/// <summary>
		/// Redeems a code of promo code. After redeeming a promo code, the user will get free items and/or the price of cart will be decreased.
		/// </summary>
		/// <remarks> Swagger method name:<c>Redeem promo code</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/promotions/promo-codes/redeem-promo-code/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="promocode">Unique code of promocode. Contains letters and numbers.</param>
		/// <param name="cartId">Unique cart identifier. Optional.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void RedeemPromocode(string projectId, string promocode, string cartId, [NotNull] Action<CartItems> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_REDEEM_PROMOCODE, projectId);
			var request = new RedeemPromocodeRequest()
			{
				coupon_code = promocode,
				cart = string.IsNullOrEmpty(cartId) ? null : new RedeemPromocodeRequest.Cart {id = cartId}
			};
			WebRequestHelper.Instance.PostRequest(SdkType.Store, url, request, WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.DeleteFromCartErrors);
		}
		
		/// <summary>
		/// Gets promo code rewards by its code. Can be used to allow users to choose one of many items as a bonus.
		/// The usual case is choosing a DRM if the promo code contains a game as a bonus (type=unit).
		/// </summary>
		/// <remarks> Swagger method name:<c>Get promo code reward</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/promotions/promo-codes/get-promo-code-rewards-by-code/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="promocode">Unique code of promocode. Contains letters and numbers.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetPromocodeReward(string projectId, string promocode, [NotNull] Action<PromocodeReward> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_GET_PROMOCODE_REWARD, projectId, promocode);

			WebRequestHelper.Instance.PostRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.DeleteFromCartErrors);
		}
	}
}
