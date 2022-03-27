using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Cart
{
	public partial class XsollaCart : MonoSingleton<XsollaCart>
	{
		private const string URL_CART_CURRENT_GET_ITEMS = Constants.BASE_STORE_API_URL + "/cart";
		private const string URL_CART_GET_ITEMS = Constants.BASE_STORE_API_URL + "/cart/{1}";

		private const string URL_CART_CURRENT_ITEM_UPDATE = Constants.BASE_STORE_API_URL + "/cart/item/{1}";
		private const string URL_CART_SPECIFIC_ITEM_UPDATE = Constants.BASE_STORE_API_URL + "/cart/{1}/item/{2}";

		private const string URL_CART_CURRENT_ITEM_REMOVE = Constants.BASE_STORE_API_URL + "/cart/item/{1}";
		private const string URL_CART_SPECIFIC_ITEM_REMOVE = Constants.BASE_STORE_API_URL + "/cart/{1}/item/{2}";

		private const string URL_CART_CURRENT_CLEAR = Constants.BASE_STORE_API_URL + "/cart/clear";
		private const string URL_CART_SPECIFIC_CLEAR = Constants.BASE_STORE_API_URL + "/cart/{1}/clear";

		private const string URL_CART_CURRENT_FILL = Constants.BASE_STORE_API_URL + "/cart/fill";
		private const string URL_CART_SPECIFIC_FILL = Constants.BASE_STORE_API_URL + "/cart/{1}/fill";

		private const string URL_REDEEM_PROMOCODE = Constants.BASE_STORE_API_URL + "/promocode/redeem";
		private const string URL_GET_PROMOCODE_REWARD = Constants.BASE_STORE_API_URL + "/promocode/code/{1}/rewards";
		private const string URL_REMOVE_PROMOCODE_FROM_CART = Constants.BASE_STORE_API_URL + "/promocode/remove";

		private const string URL_BUY_CURRENT_CART = Constants.BASE_STORE_API_URL + "/payment/cart";
		private const string URL_BUY_SPECIFIC_CART = Constants.BASE_STORE_API_URL + "/payment/cart/{1}";

		/// <summary>
		/// Returns a current user's cart.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get current user's cart</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/cart/get-user-cart/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Defines localization of item's text fields.</param>
		/// <param name="currency">Defines currency of item's price.</param>
		public void GetCartItems(string projectId, [NotNull] Action<Cart> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, [CanBeNull] string currency = null)
		{
			var url = string.Format(URL_CART_CURRENT_GET_ITEMS, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, locale: locale, currency: currency);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.HandleError(error, onError, () => GetCartItems(projectId, onSuccess, onError, locale, currency)),
				ErrorCheckType.CreateCartErrors);
		}

		/// <summary>
		/// Returns user’s cart by cart ID.
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
			url = UrlParameterizer.ConcatUrlAndParams(url, locale: locale, currency: currency);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.HandleError(error, onError, () => GetCartItems(projectId, cartId, onSuccess, onError, locale, currency)),
				ErrorCheckType.GetCartItemsErrors);
		}

		/// <summary>
		/// Fills the cart with items. If the cart already has an item with the same SKU,
		/// the existing item will be replaced by the passed value.
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
			var entity = new CartFillEntity { items = items };
			WebRequestHelper.Instance.PutRequest(SdkType.Store, url, entity, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.HandleError(error, onError, () => FillCart(projectId, items, onSuccess, onError)),
				ErrorCheckType.CreateCartErrors);
		}

		/// <summary>
		/// Fills the specific cart with items. If the cart already has an item with the same SKU,
		/// the existing item position will be replaced by the passed value.
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
			var entity = new CartFillEntity { items = items };
			WebRequestHelper.Instance.PutRequest(SdkType.Store, url, entity, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.HandleError(error, onError, () => FillCart(projectId, cartId, items, onSuccess, onError)),
				ErrorCheckType.CreateCartErrors);
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
		/// <seealso cref="GetCartItems(string,System.Action{Xsolla.Store.Cart},System.Action{Xsolla.Core.Error},string,string)"/>
		public void UpdateItemInCart(string projectId, string itemSku, int quantity, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_CART_CURRENT_ITEM_UPDATE, projectId, itemSku);
			var jsonObject = new Quantity { quantity = quantity };
			WebRequestHelper.Instance.PutRequest<Quantity>(SdkType.Store, url, jsonObject, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.HandleError(error, onError, () => UpdateItemInCart(projectId, itemSku, quantity, onSuccess, onError)),
				ErrorCheckType.AddToCartCartErrors);
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
		/// <seealso cref="GetCartItems(string,System.Action{Xsolla.Store.Cart},System.Action{Xsolla.Core.Error},string,string)"/>
		public void UpdateItemInCart(string projectId, string cartId, string itemSku, int quantity, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_CART_SPECIFIC_ITEM_UPDATE, projectId, cartId, itemSku);
			var jsonObject = new Quantity { quantity = quantity };
			WebRequestHelper.Instance.PutRequest<Quantity>(SdkType.Store, url, jsonObject, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.HandleError(error, onError, () => UpdateItemInCart(projectId, cartId, itemSku, quantity, onSuccess, onError)),
				ErrorCheckType.AddToCartCartErrors);
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
			WebRequestHelper.Instance.DeleteRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.HandleError(error, onError, () => RemoveItemFromCart(projectId, itemSku, onSuccess, onError)),
				ErrorCheckType.DeleteFromCartErrors);
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
			WebRequestHelper.Instance.DeleteRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.HandleError(error, onError, () => RemoveItemFromCart(projectId, cartId, itemSku, onSuccess, onError)),
				ErrorCheckType.DeleteFromCartErrors);
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
			WebRequestHelper.Instance.PutRequest<Quantity>(SdkType.Store, url, null, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.HandleError(error, onError, () => ClearCart(projectId, onSuccess, onError)),
				ErrorCheckType.AddToCartCartErrors);
		}

		/// <summary>
		/// Deletes all cart items.
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
			WebRequestHelper.Instance.PutRequest<Quantity>(SdkType.Store, url, null, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.HandleError(error, onError, () => ClearCart(projectId, cartId, onSuccess, onError)),
				ErrorCheckType.AddToCartCartErrors);
		}

		/// <summary>
		/// Redeems a code of promo code promotion. After redeeming a promo code, the user will get free items and/or the price of the cart and/or particular items will be decreased.
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
				cart = string.IsNullOrEmpty(cartId) ? null : new RedeemPromocodeRequest.Cart { id = cartId }
			};
			WebRequestHelper.Instance.PostRequest(SdkType.Store, url, request, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.HandleError(error, onError, () => RedeemPromocode(projectId, promocode, cartId, onSuccess, onError)),
				ErrorCheckType.DeleteFromCartErrors);
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

			WebRequestHelper.Instance.PostRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.HandleError(error, onError, () => GetPromocodeReward(projectId, promocode, onSuccess, onError)),
				ErrorCheckType.DeleteFromCartErrors);
		}

		/// <summary>
		/// Removes a promo code from a cart. 
		/// After the promo code is removed, the total price of all items in the cart will be recalculated without bonuses and discounts provided by a promo code.
		/// </summary>
		/// <see cref="https://developers.xsolla.com/in-game-store-buy-button-api/promotions/promo-codes/remove-cart-promo-code/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="cartId">Cart ID.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void RemovePromocodeFromCart(string projectId, string cartId, Action<RemovePromocodeFromCartResponse> onSuccess, Action<Error> onError = null)
		{
			var data = new RemovePromocodeFromCartRequest(cartId);
			var url = string.Format(URL_REMOVE_PROMOCODE_FROM_CART, projectId);

			WebRequestHelper.Instance.PutRequest(SdkType.Store, url, data, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.HandleError(error, onError, () => RemovePromocodeFromCart(projectId, cartId, onSuccess, onError)),
				ErrorCheckType.DeleteFromCartErrors);
		}

		/// <summary>
		/// Creates an order with all items from the cart. The created order will get a 'new' order status.
		/// </summary>
		/// <remarks> Swagger method name:<c>Creates an order with all items from the current cart</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/payment/create-order/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c> and <c>currency</c>.</param>
		/// <param name="customHeaders">Custom web request headers.</param>
		public void PurchaseCart(string projectId, [CanBeNull] Action<PurchaseData> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null, Dictionary<string, string> customHeaders = null)
		{
			var tempPurchaseParams = PurchaseParamsGenerator.GenerateTempPurchaseParams(purchaseParams);
			var url = string.Format(URL_BUY_CURRENT_CART, projectId);
			var paymentHeaders = PurchaseParamsGenerator.GetPaymentHeaders(Token.Instance, customHeaders);
			WebRequestHelper.Instance.PostRequest<PurchaseData, TempPurchaseParams>(SdkType.Store, url, tempPurchaseParams, paymentHeaders, onSuccess,
				onError: error => TokenRefresh.HandleError(error, onError, () => PurchaseCart(projectId, onSuccess, onError, purchaseParams, customHeaders)),
				ErrorCheckType.BuyCartErrors);
		}

		/// <summary>
		/// Creates an order with all items from the particular cart. The created order will get a 'new' order status.
		/// </summary>
		/// <remarks> Swagger method name:<c>Creates an order with all items from the particular cart</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/payment/create-order-by-cart-id/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c> and <c>currency</c>.</param>
		/// <param name="customHeaders">Custom web request headers.</param>
		public void PurchaseCart(string projectId, string cartId, [CanBeNull] Action<PurchaseData> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null, Dictionary<string, string> customHeaders = null)
		{
			var tempPurchaseParams = PurchaseParamsGenerator.GenerateTempPurchaseParams(purchaseParams);
			var url = string.Format(URL_BUY_SPECIFIC_CART, projectId, cartId);
			var paymentHeaders = PurchaseParamsGenerator.GetPaymentHeaders(Token.Instance, customHeaders);
			WebRequestHelper.Instance.PostRequest<PurchaseData, TempPurchaseParams>(SdkType.Store, url, tempPurchaseParams, paymentHeaders, onSuccess,
				onError: error => TokenRefresh.HandleError(error, onError, () => PurchaseCart(projectId, cartId, onSuccess, onError, purchaseParams, customHeaders)),
				ErrorCheckType.BuyCartErrors);
		}
	}
}
