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

		/// <summary>
		/// Creates a new cart on the Xsolla side.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get current user's cart</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/cart/get-user-cart/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void CreateNewCart(string projectId, [NotNull] Action<Cart> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CART_CREATE_NEW, projectId)).Append(AnalyticUrlAddition);
			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), AuthAndAnalyticHeaders, onSuccess, onError, Error.CreateCartErrors);
		}

		/// <summary>
		/// Fills the current cart with items. If the cart already has an item,
		/// the existing item will be replaced by the given value.
		/// </summary>
		/// <remarks> Swagger method name:<c>Fill cart with items</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/cart/cart-fill/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="items">Items for purchase.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void FillCart(string projectId, List<CartFillItem> items, [NotNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CART_CURRENT_FILL, projectId)).Append(AnalyticUrlAddition);
			var entity = new CartFillEntity {items = items};
			WebRequestHelper.Instance.PutRequest(urlBuilder.ToString(), entity, AuthAndAnalyticHeaders, onSuccess, onError, Error.CreateCartErrors);
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
			var urlBuilder = new StringBuilder(string.Format(URL_CART_SPECIFIC_FILL, projectId, cartId)).Append(AnalyticUrlAddition);
			var entity = new CartFillEntity {items = items};
			WebRequestHelper.Instance.PutRequest(urlBuilder.ToString(), entity, AuthAndAnalyticHeaders, onSuccess, onError, Error.CreateCartErrors);
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
			var urlBuilder = new StringBuilder(string.Format(URL_CART_CURRENT_ITEM_UPDATE, projectId, itemSku)).Append(AnalyticUrlAddition);
			var jsonObject = new Quantity {quantity = quantity};
			WebRequestHelper.Instance.PutRequest<Quantity>(urlBuilder.ToString(), jsonObject, AuthAndAnalyticHeaders, onSuccess, onError, Error.AddToCartCartErrors);
		}

		/// <summary>
		/// Updates an existing item or creates the one in the specific cart.
		/// </summary>
		/// <remarks> Swagger method name:<c>Update cart line item by cart ID</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/cart/put-item-by-cart-id/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="itemSku">SKU of item for purchase.</param>
		/// <param name="quantity">Quantity of purchased item.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="CreateNewCart"/>
		public void UpdateItemInCart(string projectId, string cartId, string itemSku, int quantity, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CART_SPECIFIC_ITEM_UPDATE, projectId, cartId, itemSku)).Append(AnalyticUrlAddition);
			var jsonObject = new Quantity {quantity = quantity};
			WebRequestHelper.Instance.PutRequest<Quantity>(urlBuilder.ToString(), jsonObject, AuthAndAnalyticHeaders, onSuccess, onError, Error.AddToCartCartErrors);
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
			var urlBuilder = new StringBuilder(string.Format(URL_CART_CURRENT_CLEAR, projectId)).Append(AnalyticUrlAddition);
			WebRequestHelper.Instance.PutRequest<Quantity>(urlBuilder.ToString(), null, AuthAndAnalyticHeaders, onSuccess, onError, Error.AddToCartCartErrors);
		}

		/// <summary>
		/// Deletes all specific cart items.
		/// </summary>
		/// <remarks> Swagger method name:<c>Delete all cart line items by cart ID</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/cart/cart-clear-by-id/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void ClearCart(string projectId, string cartId, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CART_SPECIFIC_CLEAR, projectId, cartId)).Append(AnalyticUrlAddition);
			WebRequestHelper.Instance.PutRequest<Quantity>(urlBuilder.ToString(), null, AuthAndAnalyticHeaders, onSuccess, onError, Error.AddToCartCartErrors);
		}

		/// <summary>
		/// Returns a user’s cart by ID.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get cart by ID</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/cart/get-cart-by-id/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Defines localization of item's text fields.</param>
		/// <param name="currency">Defines currency of item's price.</param>
		public void GetCartItems(string projectId, string cartId, [NotNull] Action<CartItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, [CanBeNull] string currency = null)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CART_GET_ITEMS, projectId, cartId)).Append(AnalyticUrlAddition);
			urlBuilder.Append(GetLocaleUrlParam(locale));
			urlBuilder.Append(GetCurrencyUrlParam(currency));
			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), AuthAndAnalyticHeaders, onSuccess, onError, Error.GetCartItemsErrors);
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
			var urlBuilder = new StringBuilder(string.Format(URL_CART_CURRENT_ITEM_REMOVE, projectId, itemSku)).Append(AnalyticUrlAddition);
			WebRequestHelper.Instance.DeleteRequest(urlBuilder.ToString(), AuthAndAnalyticHeaders, onSuccess, onError, Error.DeleteFromCartErrors);
		}

		/// <summary>
		/// Delete item from the specific cart.
		/// </summary>
		/// <remarks> Swagger method name:<c>Delete cart line item by cart ID</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/cart/delete-item-by-cart-id/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="itemSku">Item SKU to delete.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void RemoveItemFromCart(string projectId, string cartId, string itemSku, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CART_SPECIFIC_ITEM_REMOVE, projectId, cartId, itemSku)).Append(AnalyticUrlAddition);
			WebRequestHelper.Instance.DeleteRequest(urlBuilder.ToString(), AuthAndAnalyticHeaders, onSuccess, onError, Error.DeleteFromCartErrors);
		}
	}
}