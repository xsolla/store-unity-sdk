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

		private const string URL_BUY_CURRENT_FREE_CART = Constants.BASE_STORE_API_URL + "/free/cart";
		private const string URL_BUY_SPECIFIC_FREE_CART = Constants.BASE_STORE_API_URL + "/free/cart/{1}";

		/// <summary>
		/// Returns a list of items from the cart of the current user. For each item, complete data is returned.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/cart-purchase/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. </param>
		/// <param name="onSuccess">Called after local cache of cart items was successfully updated.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="locale">Defines localization of item's text fields.<br/>
		/// The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).</param>
		/// <param name="currency">The currency in which prices are displayed. Three-letter currency code per [ISO 4217](https://en.wikipedia.org/wiki/ISO_4217) (USD by default).</param>
		public void GetCartItems(string projectId, [NotNull] Action<Cart> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, [CanBeNull] string currency = null)
		{
			var url = string.Format(URL_CART_CURRENT_GET_ITEMS, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, locale: locale, currency: currency);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => GetCartItems(projectId, onSuccess, onError, locale, currency)),
				ErrorCheckType.CreateCartErrors);
		}

		/// <summary>
		/// Returns a list of items from the cart with the specified ID. For each item, complete data is returned.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/cart-purchase/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. </param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="onSuccess">Called after local cache of cart items was successfully updated.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="locale">Defines localization of item's text fields.<br/>
		/// The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).</param>
		/// <param name="currency">The currency in which prices are displayed. Three-letter currency code per [ISO 4217](https://en.wikipedia.org/wiki/ISO_4217) (USD by default).</param>
		public void GetCartItems(string projectId, string cartId, [NotNull] Action<CartItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, [CanBeNull] string currency = null)
		{
			var url = string.Format(URL_CART_GET_ITEMS, projectId, cartId);
			url = UrlParameterizer.ConcatUrlAndParams(url, locale: locale, currency: currency);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => GetCartItems(projectId, cartId, onSuccess, onError, locale, currency)),
				ErrorCheckType.GetCartItemsErrors);
		}

		/// <summary>
		/// Fills the cart of the current user with items. If there is already an item with the same SKU in the cart, the existing item position will be replaced by the passed value.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/cart-purchase/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. </param>
		/// <param name="items">Item for filling the cart. If there is already an item with the same SKU in the cart, the existing item position will be replaced by the passed value.</param>
		/// <param name="onSuccess">Called after cart is successfully filled.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void FillCart(string projectId, List<CartFillItem> items, [NotNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_CART_CURRENT_FILL, projectId);
			var entity = new CartFillEntity { items = items };
			WebRequestHelper.Instance.PutRequest(SdkType.Store, url, entity, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => FillCart(projectId, items, onSuccess, onError)),
				ErrorCheckType.CreateCartErrors);
		}

		/// <summary>
		/// Fills the cart with the specified ID with items. If there is already an item with the same SKU in the cart, the existing item position will be replaced by the passed value.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/cart-purchase/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. </param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="items">Item for filling the cart. If there is already an item with the same SKU in the cart, the existing item position will be replaced by the passed value.</param>
		/// <param name="onSuccess">Called after cart is successfully filled.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void FillCart(string projectId, string cartId, List<CartFillItem> items, [NotNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_CART_SPECIFIC_FILL, projectId, cartId);
			var entity = new CartFillEntity { items = items };
			WebRequestHelper.Instance.PutRequest(SdkType.Store, url, entity, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => FillCart(projectId, cartId, items, onSuccess, onError)),
				ErrorCheckType.CreateCartErrors);
		}

		/// <summary>
		/// Updates the quantity of a previously added item in the current user cart. If there is no item with the specified SKU in the cart, it will be added.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/cart-purchase/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. </param>
		/// <param name="itemSku">SKU of item for purchase.</param>
		/// <param name="quantity">Quantity of purchased item.</param>
		/// <param name="onSuccess">Called after successfully adding a new item to the cart.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso cref="GetCartItems(string,System.Action{Xsolla.Store.Cart},System.Action{Xsolla.Core.Error},string,string)"/>
		public void UpdateItemInCart(string projectId, string itemSku, int quantity, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_CART_CURRENT_ITEM_UPDATE, projectId, itemSku);
			var jsonObject = new Quantity { quantity = quantity };
			WebRequestHelper.Instance.PutRequest<Quantity>(SdkType.Store, url, jsonObject, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => UpdateItemInCart(projectId, itemSku, quantity, onSuccess, onError)),
				ErrorCheckType.AddToCartCartErrors);
		}

		/// <summary>
		/// Updates the quantity of a previously added item in the cart with the specified ID. If there is no item with the specified SKU in the cart, it will be added.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/cart-purchase/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. </param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="itemSku">SKU of item for purchase.</param>
		/// <param name="quantity">Quantity of purchased items.</param>
		/// <param name="onSuccess">Called after successfully adding a new item to the cart.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso cref="GetCartItems(string,System.Action{Xsolla.Store.Cart},System.Action{Xsolla.Core.Error},string,string)"/>
		public void UpdateItemInCart(string projectId, string cartId, string itemSku, int quantity, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_CART_SPECIFIC_ITEM_UPDATE, projectId, cartId, itemSku);
			var jsonObject = new Quantity { quantity = quantity };
			WebRequestHelper.Instance.PutRequest<Quantity>(SdkType.Store, url, jsonObject, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => UpdateItemInCart(projectId, cartId, itemSku, quantity, onSuccess, onError)),
				ErrorCheckType.AddToCartCartErrors);
		}

		/// <summary>
		/// Removes the item from the cart of the current user.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/cart-purchase/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. </param>
		/// <param name="itemSku">Item SKU to delete.</param>
		/// <param name="onSuccess">Called after successfully removing an item from the cart.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void RemoveItemFromCart(string projectId, string itemSku, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_CART_CURRENT_ITEM_REMOVE, projectId, itemSku);
			WebRequestHelper.Instance.DeleteRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => RemoveItemFromCart(projectId, itemSku, onSuccess, onError)),
				ErrorCheckType.DeleteFromCartErrors);
		}

		/// <summary>
		/// Removes the item from the cart with the specified ID.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/cart-purchase/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. </param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="itemSku">Item SKU to delete.</param>
		/// <param name="onSuccess">Called after successfully removing an item from the cart.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void RemoveItemFromCart(string projectId, string cartId, string itemSku, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_CART_SPECIFIC_ITEM_REMOVE, projectId, cartId, itemSku);
			WebRequestHelper.Instance.DeleteRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => RemoveItemFromCart(projectId, cartId, itemSku, onSuccess, onError)),
				ErrorCheckType.DeleteFromCartErrors);
		}

		/// <summary>
		/// Removes all items from the cart of the current user.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/cart-purchase/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. </param>
		/// <param name="onSuccess">Called after successful cart clearing.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void ClearCart(string projectId, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_CART_CURRENT_CLEAR, projectId);
			WebRequestHelper.Instance.PutRequest<Quantity>(SdkType.Store, url, null, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => ClearCart(projectId, onSuccess, onError)),
				ErrorCheckType.AddToCartCartErrors);
		}

		/// <summary>
		/// Removes all items from the cart with the specified ID.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/cart-purchase/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. </param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="onSuccess">Called after successful cart clearing.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void ClearCart(string projectId, string cartId, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_CART_SPECIFIC_CLEAR, projectId, cartId);
			WebRequestHelper.Instance.PutRequest<Quantity>(SdkType.Store, url, null, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => ClearCart(projectId, cartId, onSuccess, onError)),
				ErrorCheckType.AddToCartCartErrors);
		}

		/// <summary>
		/// Redeems a promo code. After activating the promo code, the user gets free items and/or the price of the cart is reduced.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/promo/promo-codes/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. </param>
		/// <param name="promocode">Unique code of promocode. Contains letters and numbers.</param>
		/// <param name="cartId">Unique cart identifier. The current user cart will be updated if empty.</param>
		/// <param name="onSuccess">Called after successful promocode redemption.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void RedeemPromocode(string projectId, string promocode, string cartId, [NotNull] Action<CartItems> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_REDEEM_PROMOCODE, projectId);
			var request = new RedeemPromocodeRequest()
			{
				coupon_code = promocode,
				cart = string.IsNullOrEmpty(cartId) ? null : new RedeemPromocodeRequest.Cart { id = cartId }
			};
			WebRequestHelper.Instance.PostRequest(SdkType.Store, url, request, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => RedeemPromocode(projectId, promocode, cartId, onSuccess, onError)),
				ErrorCheckType.DeleteFromCartErrors);
		}

		/// <summary>
		/// Returns a list of items that can be credited to the user when the promo code is activated. Allows users to choose from several available items.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/promo/promo-codes/#sdk_promo_codes).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. </param>
		/// <param name="promocode">Unique code of promocode. Contains letters and numbers.</param>
		/// <param name="onSuccess">Called after successfully receiving promocode rewards.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void GetPromocodeReward(string projectId, string promocode, [NotNull] Action<PromocodeReward> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_GET_PROMOCODE_REWARD, projectId, promocode);

			WebRequestHelper.Instance.PostRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => GetPromocodeReward(projectId, promocode, onSuccess, onError)),
				ErrorCheckType.DeleteFromCartErrors);
		}

		/// <summary>
		/// Removes a promo code from a cart. After the promo code is removed, the total price of all items in the cart will be recalculated without bonuses and discounts provided by a promo code.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/promo/promo-codes/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. </param>
		/// <param name="cartId">Cart ID. The current user cart will be updated if empty.</param>
		/// <param name="onSuccess">Called after the promo code  has been successful removed from cart.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void RemovePromocodeFromCart(string projectId, string cartId, Action<RemovePromocodeFromCartResponse> onSuccess, Action<Error> onError = null)
		{
			var data = new RemovePromocodeFromCartRequest(cartId);
			var url = string.Format(URL_REMOVE_PROMOCODE_FROM_CART, projectId);

			WebRequestHelper.Instance.PutRequest(SdkType.Store, url, data, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => RemovePromocodeFromCart(projectId, cartId, onSuccess, onError)),
				ErrorCheckType.DeleteFromCartErrors);
		}

		/// <summary>
		/// Creates an order with items from the cart of the current user. Returns the payment token and order ID. The created order will get a `new` order status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/cart-purchase/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. </param>
		/// <param name="onSuccess">Called after the payment token was successfully fetched.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c> and <c>currency</c>.</param>
		/// <param name="customHeaders">Custom web request headers.</param>
		public void PurchaseCart(string projectId, [CanBeNull] Action<PurchaseData> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null, Dictionary<string, string> customHeaders = null)
		{
			var tempPurchaseParams = PurchaseParamsGenerator.GenerateTempPurchaseParams(purchaseParams);
			var url = string.Format(URL_BUY_CURRENT_CART, projectId);
			var paymentHeaders = PurchaseParamsGenerator.GetPaymentHeaders(Token.Instance, customHeaders);
			WebRequestHelper.Instance.PostRequest<PurchaseData, TempPurchaseParams>(SdkType.Store, url, tempPurchaseParams, paymentHeaders, onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => PurchaseCart(projectId, onSuccess, onError, purchaseParams, customHeaders)),
				ErrorCheckType.BuyCartErrors);
		}

		/// <summary>
		/// Creates an order with items from the cart with the specified ID. Returns the payment token and order ID. The created order will get a `new` order status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/cart-purchase/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. </param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="onSuccess">Called after the payment token was successfully fetched.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c> and <c>currency</c>.</param>
		/// <param name="customHeaders">Custom web request headers.</param>
		public void PurchaseCart(string projectId, string cartId, [CanBeNull] Action<PurchaseData> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null, Dictionary<string, string> customHeaders = null)
		{
			var tempPurchaseParams = PurchaseParamsGenerator.GenerateTempPurchaseParams(purchaseParams);
			var url = string.Format(URL_BUY_SPECIFIC_CART, projectId, cartId);
			var paymentHeaders = PurchaseParamsGenerator.GetPaymentHeaders(Token.Instance, customHeaders);
			WebRequestHelper.Instance.PostRequest<PurchaseData, TempPurchaseParams>(SdkType.Store, url, tempPurchaseParams, paymentHeaders, onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => PurchaseCart(projectId, cartId, onSuccess, onError, purchaseParams, customHeaders)),
				ErrorCheckType.BuyCartErrors);
		}

		/// <summary>
		/// Create order with free cart. The created order will get a `done` order status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/promo/free-items/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. </param>
		/// <param name="onSuccess">Called after the payment was successfully completed.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c> and <c>currency</c>.</param>
		/// <param name="customHeaders">Custom web request headers.</param>
		public void CreateOrderWithFreeCart(string projectId, [CanBeNull] Action<int> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null, Dictionary<string, string> customHeaders = null)
		{
			var tempPurchaseParams = PurchaseParamsGenerator.GenerateTempPurchaseParams(purchaseParams);
			var url = string.Format(URL_BUY_CURRENT_FREE_CART, projectId);
			var paymentHeaders = PurchaseParamsGenerator.GetPaymentHeaders(Token.Instance, customHeaders);
			WebRequestHelper.Instance.PostRequest<PurchaseData, TempPurchaseParams>(SdkType.Store, url, tempPurchaseParams, paymentHeaders,
				onComplete: purchaseData => onSuccess?.Invoke(purchaseData.order_id),
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => CreateOrderWithFreeCart(projectId, onSuccess, onError, purchaseParams, customHeaders)),
				ErrorCheckType.BuyCartErrors);
		}

		/// <summary>
		/// Create order with particular free cart. The created order will get a `done` order status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/promo/free-items/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. </param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c> and <c>currency</c>.</param>
		/// <param name="customHeaders">Custom web request headers.</param>
		public void CreateOrderWithParticularFreeCart(string projectId, string cartId, [CanBeNull] Action<int> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null, Dictionary<string, string> customHeaders = null)
		{
			var tempPurchaseParams = PurchaseParamsGenerator.GenerateTempPurchaseParams(purchaseParams);
			var url = string.Format(URL_BUY_SPECIFIC_FREE_CART, projectId, cartId);
			var paymentHeaders = PurchaseParamsGenerator.GetPaymentHeaders(Token.Instance, customHeaders);
			WebRequestHelper.Instance.PostRequest<PurchaseData, TempPurchaseParams>(SdkType.Store, url, tempPurchaseParams, paymentHeaders,
				onComplete: purchaseData => onSuccess?.Invoke(purchaseData.order_id),
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => CreateOrderWithParticularFreeCart(projectId, cartId, onSuccess, onError, purchaseParams, customHeaders)),
				ErrorCheckType.BuyCartErrors);
		}
	}
}
