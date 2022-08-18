using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Catalog
{
	public partial class XsollaCatalog : MonoSingleton<XsollaCatalog>
	{
		private const string URL_CATALOG_GET_ALL_VIRTUAL_ITEMS = Constants.BASE_STORE_API_URL + "/items/virtual_items/all";
		private const string URL_CATALOG_GET_ITEMS = Constants.BASE_STORE_API_URL + "/items/virtual_items";
		private const string URL_CATALOG_GET_BUNDLE = Constants.BASE_STORE_API_URL + "/items/bundle/sku/{1}";
		private const string URL_CATALOG_GET_BUNDLES = Constants.BASE_STORE_API_URL + "/items/bundle";
		private const string URL_CATALOG_GET_ITEMS_IN_GROUP = Constants.BASE_STORE_API_URL + "/items/virtual_items/group/{1}";
		private const string URL_CATALOG_GET_GROUPS = Constants.BASE_STORE_API_URL + "/items/groups";

		private const string URL_VIRTUAL_CURRENCY_LIST = Constants.BASE_STORE_API_URL + "/items/virtual_currency";
		private const string URL_VIRTUAL_CURRENCY_PACKAGES_IN_PROJECT = Constants.BASE_STORE_API_URL + "/items/virtual_currency/package";

		private const string URL_INVENTORY_REDEEM_COUPON = Constants.BASE_STORE_API_URL + "/coupon/redeem";
		private const string URL_INVENTORY_GET_COUPON_REWARDS = Constants.BASE_STORE_API_URL + "/coupon/code/{1}/rewards";

		private const string URL_BUY_ITEM = Constants.BASE_STORE_API_URL + "/payment/item/{1}";
		private const string URL_BUY_ITEM_FOR_VC = Constants.BASE_STORE_API_URL + "/payment/item/{1}/virtual/{2}";

		/// <summary>
		/// Returns all items in catalog.
/*TEXTREVIEW*/
		/// If used after authorization will also return items available for this specific user.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get virtual items list</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/items/get-virtual-items"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Response language. Two-letter lowercase language code per ISO 639-1.</param>
		/// <param name="additionalFields">The list of additional fields. These fields will be in a response if you send them in a request. Available fields 'media_list', 'order', and 'long_description'.</param>
		/// <param name="country">Country used to calculate regional prices and restrictions for the catalog. Two-letter uppercase country code per ISO 3166-1 alpha-2. If you do not specify the country explicitly, it will be defined by the user IP address.</param>
		public void GetCatalog(string projectId, [NotNull] Action<StoreItems> onSuccess, [CanBeNull] Action<Error> onError, int limit = 50, int offset = 0, [CanBeNull] string locale = null, string additionalFields = "long_description", [CanBeNull] string country = null)
		{
			var url = string.Format(URL_CATALOG_GET_ITEMS, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, limit: limit, offset: offset, locale: locale, additionalFields: additionalFields, country: country);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, GetPersonalizationHeader(), onSuccess, onError, ErrorCheckType.ItemsListErrors);
		}

		/// <summary>
		/// Gets a list of all virtual items for searching on client-side.
/*TEXTREVIEW*/
		/// If used after authorization will also return items available for this specific user.
		/// </summary>
		/// <see cref="https://developers.xsolla.com/in-game-store-buy-button-api/virtual-items-currency/catalog/get-all-virtual-items/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="locale">Response language. Two-letter lowercase language code per ISO 639-1.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetCatalogSimplified(string projectId, Action<StoreItemShortCollection> onSuccess, Action<Error> onError = null, string locale = null)
		{
			var url = string.Format(URL_CATALOG_GET_ALL_VIRTUAL_ITEMS, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, locale: locale);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, GetPersonalizationHeader(), onSuccess, onError, ErrorCheckType.ItemsListErrors);
		}

		/// <summary>
		/// Returns items in a specific group.
/*TEXTREVIEW*/
		/// If used after authorization will also return items available for this specific user.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get items list by specified group</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/items/get-virtual-items-group"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="groupExternalId">Group external ID.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Response language. Two-letter lowercase language code per ISO 639-1.</param>
		/// <param name="additionalFields">The list of additional fields. This fields will be in a response if you send its in a request. Available fields 'media_list', 'order', 'long_description'.</param>
		/// <param name="country">Country used to calculate regional prices and restrictions for the catalog. Two-letter uppercase country code per ISO 3166-1 alpha-2. If you do not specify the country explicitly, it will be defined by the user IP address.</param>
		public void GetGroupItems(string projectId, string groupExternalId, [NotNull] Action<StoreItems> onSuccess, [CanBeNull] Action<Error> onError, int? limit = null, int? offset = null, [CanBeNull] string locale = null, string additionalFields = null, string country = null)
		{
			var url = string.Format(URL_CATALOG_GET_ITEMS_IN_GROUP, projectId, groupExternalId);
			url = UrlParameterizer.ConcatUrlAndParams(url, limit: limit, offset: offset, locale: locale, additionalFields: additionalFields, country: country);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, GetPersonalizationHeader(), onSuccess, onError, ErrorCheckType.ItemsListErrors);
		}

		/// <summary>
		/// Returns groups list.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get items groups list</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/groups/get-item-groups"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Defines localization of the item text fields.</param>
		public void GetItemGroups(string projectId, [NotNull] Action<Groups> onSuccess, [CanBeNull] Action<Error> onError, int limit = 50, int offset = 0, [CanBeNull] string locale = null)
		{
			var url = string.Format(URL_CATALOG_GET_GROUPS, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, limit: limit, offset: offset, locale: locale);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, onSuccess, onError);
		}

		/// <summary>
		/// Returns info for all virtual currencies.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get virtual currency list</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/items/get-virtual-currency"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Response language. Two-letter lowercase language code per ISO 639-1.</param>
		/// <param name="additionalFields">The list of additional fields. These fields will be in a response if you send them in a request. Available fields 'media_list', 'order', and 'long_description'.</param>
		/// <param name="country">Country used to calculate regional prices and restrictions for the catalog. Two-letter uppercase country code per ISO 3166-1 alpha-2. If you do not specify the country explicitly, it will be defined by the user IP address.</param>
		public void GetVirtualCurrencyList(string projectId, [NotNull] Action<VirtualCurrencyItems> onSuccess, [CanBeNull] Action<Error> onError, int limit = 50, int offset = 0, [CanBeNull] string locale = null, string additionalFields = null, string country = null)
		{
			var url = string.Format(URL_VIRTUAL_CURRENCY_LIST, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, limit: limit, offset: offset, locale: locale, additionalFields: additionalFields, country: country);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, onSuccess, onError, ErrorCheckType.ItemsListErrors);
		}

		/// <summary>
		/// Returns the list of virtual currency packages.
/*TEXTREVIEW*/
		/// If used after authorization will also return items available for this specific user.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get virtual currency package list</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/items/get-virtual-currency-package"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Response language. Two-letter lowercase language code per ISO 639-1.</param>
		/// <param name="additionalFields">The list of additional fields. These fields will be in a response if you send them in a request. Available fields 'media_list', 'order', and 'long_description'.</param>
		/// <param name="country">Country used to calculate regional prices and restrictions for the catalog. Two-letter uppercase country code per ISO 3166-1 alpha-2. If you do not specify the country explicitly, it will be defined by the user IP address.</param>
		public void GetVirtualCurrencyPackagesList(string projectId, [NotNull] Action<VirtualCurrencyPackages> onSuccess, [CanBeNull] Action<Error> onError, int limit = 50, int offset = 0, [CanBeNull] string locale = null, string additionalFields = null, string country = null)
		{
			var url = string.Format(URL_VIRTUAL_CURRENCY_PACKAGES_IN_PROJECT, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, limit: limit, offset: offset, locale: locale, additionalFields: additionalFields, country: country);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, GetPersonalizationHeader(), onSuccess, onError, ErrorCheckType.ItemsListErrors);
		}

		/// <summary>
		/// Returns all bundles in a catalog.
/*TEXTREVIEW*/
		/// If used after authorization will also return items available for this specific user.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get list of bundles</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/bundles/catalog/get-bundle-list"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Defines localization of the item text fields.</param>
		/// <param name="additionalFields">The list of additional fields. This fields will be in a response if you send its in a request. Available fields 'media_list', 'order', 'long_description'.</param>
		/// <param name="country">Country used to calculate regional prices and restrictions for the catalog. Two-letter uppercase country code per ISO 3166-1 alpha-2. If you do not specify the country explicitly, it will be defined by the user IP address.</param>
		public void GetBundles(string projectId, [NotNull] Action<BundleItems> onSuccess, [CanBeNull] Action<Error> onError, int limit = 50, int offset = 0, [CanBeNull] string locale = null, string additionalFields = null, [CanBeNull] string country = null)
		{
			var url = string.Format(URL_CATALOG_GET_BUNDLES, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, limit: limit, offset: offset, locale: locale, additionalFields: additionalFields, country: country);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, GetPersonalizationHeader(), onSuccess, onError, ErrorCheckType.ItemsListErrors);
		}

		/// <summary>
		/// Returns specified bundle.
/*TEXTREVIEW*/
		/// If used after authorization will be able to return bundle available for this specific user.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get specified bundle</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/bundles/catalog/get-bundle"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="sku"></param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Defines localization of the item text fields.</param>
		/// <param name="country">Country used to calculate regional prices and restrictions for the catalog. Two-letter uppercase country code per ISO 3166-1 alpha-2. If you do not specify the country explicitly, it will be defined by the user IP address.</param>
		public void GetBundle(string projectId, string sku, [NotNull] Action<BundleItem> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, [CanBeNull] string country = null)
		{
			var url = string.Format(URL_CATALOG_GET_BUNDLE, projectId, sku);
			url = UrlParameterizer.ConcatUrlAndParams(url, locale: locale, country: country);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, GetPersonalizationHeader(), onSuccess, onError, ErrorCheckType.ItemsListErrors);
		}

		/// <summary>
		/// Redeems a coupon code.
		/// </summary>
		/// <remarks> Swagger method name:<c>Redeem coupon code</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/promotions/coupons/redeem-coupon/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="couponCode">Unique coupon code. Contains letters and numbers.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void RedeemCouponCode(string projectId, CouponCode couponCode, [NotNull] Action<CouponRedeemedItems> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_INVENTORY_REDEEM_COUPON, projectId);

			var headers = new List<WebRequestHeader>() { WebRequestHeader.AuthHeader(Token.Instance), WebRequestHeader.ContentTypeHeader() };
			WebRequestHelper.Instance.PostRequest(SdkType.Store, url, couponCode, headers, onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => RedeemCouponCode(projectId, couponCode, onSuccess, onError)),
				ErrorCheckType.CouponErrors);
		}

		/// <summary>
		/// Gets coupons rewards by its code. Can be used to allow users to choose one of many items as a bonus.
		/// The usual case is choosing a DRM if the coupon contains a game as a bonus (type=unit).
		/// </summary>
		/// <remarks> Swagger method name:<c>Get coupon rewards</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/promotions/coupons/get-coupon-rewards-by-code/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="couponCode">Unique case sensitive code. Contains letters and numbers.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetCouponRewards(string projectId, string couponCode, [NotNull] Action<CouponReward> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_INVENTORY_GET_COUPON_REWARDS, projectId, couponCode);

			var headers = new List<WebRequestHeader>() { WebRequestHeader.AuthHeader(Token.Instance), WebRequestHeader.ContentTypeHeader() };
			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, headers, onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => GetCouponRewards(projectId, couponCode, onSuccess, onError)),
				ErrorCheckType.CouponErrors);
		}

		/// <summary>
		/// Creates an order with a specified item. The created order will get a 'new' order status.
		/// </summary>
		/// <remarks> Swagger method name:<c>Create order with specified item</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/payment/create-order-with-item"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="itemSku">Item SKU to purchase.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c>, and <c>currency</c>.</param>
		/// <param name="customHeaders">Custom web request headers</param>
		/// <seealso cref="OpenPurchaseUi"/>
		public void PurchaseItem(string projectId, string itemSku, [CanBeNull] Action<PurchaseData> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null, Dictionary<string, string> customHeaders = null)
		{
			var tempPurchaseParams = PurchaseParamsGenerator.GenerateTempPurchaseParams(purchaseParams);
			var url = string.Format(URL_BUY_ITEM, projectId, itemSku);
			var paymentHeaders = PurchaseParamsGenerator.GetPaymentHeaders(Token.Instance, customHeaders);
			WebRequestHelper.Instance.PostRequest<PurchaseData, TempPurchaseParams>(SdkType.Store, url, tempPurchaseParams, paymentHeaders, onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => PurchaseItem(projectId, itemSku, onSuccess, onError, purchaseParams, customHeaders)),
				ErrorCheckType.BuyItemErrors);
		}

		/// <summary>
		/// Returns unique identifier of the created order and
		/// the Pay Station token for the purchase of the specified product by virtual currency.
		/// </summary>
		/// <remarks> Swagger method name:<c>Create order with specified item purchased by virtual currency</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/payment/create-order-with-item-for-virtual-currency"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="itemSku">Item SKU to purchase.</param>
		/// <param name="priceSku">Virtual currency SKU.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c> and <c>currency</c>.</param>
		/// <param name="platform">Publishing platform the user plays on.</param>
		/// <param name="customHeaders">>Custom web request headers.</param>
		/// <seealso cref="OpenPurchaseUi"/>
		public void PurchaseItemForVirtualCurrency(
			string projectId,
			string itemSku,
			string priceSku,
			[CanBeNull] Action<PurchaseData> onSuccess,
			[CanBeNull] Action<Error> onError,
			PurchaseParams purchaseParams = null,
			string platform = null,
			Dictionary<string, string> customHeaders = null)
		{
			TempPurchaseParams tempPurchaseParams = new TempPurchaseParams
			{
				sandbox = XsollaSettings.IsSandbox,
				custom_parameters = purchaseParams?.custom_parameters
			};

			var url = string.Format(URL_BUY_ITEM_FOR_VC, projectId, itemSku, priceSku);
			var platformParam = UrlParameterizer.GetPlatformUrlParam(platform);
			url = UrlParameterizer.ConcatUrlAndParams(url, platformParam);

			var paymentHeaders = PurchaseParamsGenerator.GetPaymentHeaders(Token.Instance, customHeaders);
			WebRequestHelper.Instance.PostRequest<PurchaseData, TempPurchaseParams>(SdkType.Store, url, tempPurchaseParams, paymentHeaders, onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => PurchaseItemForVirtualCurrency(projectId, itemSku, priceSku, onSuccess, onError, purchaseParams, platform, customHeaders)),
				ErrorCheckType.BuyItemErrors);
		}

		private WebRequestHeader GetPersonalizationHeader() => (Token.Instance != null) ? WebRequestHeader.AuthHeader(Token.Instance) : null;
	}
}
