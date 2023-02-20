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

		private const string URL_CREATE_FREE_ORDER_WITH_ITEM = Constants.BASE_STORE_API_URL + "/free/item/{1}";

		/// <summary>
		/// Returns a list of virtual items according to pagination settings.
		/// The list includes items for which display in the store is enabled in the settings. For each virtual item, complete data is returned.
		/// If used after user authentication, the method returns items that match the personalization rules for the current user.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Called after virtual items were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Response language.</br>
		/// The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).<br/>
		/// Leave empty to use the default value.</param>
		/// <param name="additionalFields">The list of additional fields. These fields will be in a response if you send them in a request. Available fields `media_list`, `order`, and `long_description`.</param>
		/// <param name="country">Country to calculate regional prices and restrictions to catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). Calculated based on the user's IP address if not specified.</param>
		public void GetCatalog(string projectId, [NotNull] Action<StoreItems> onSuccess, [CanBeNull] Action<Error> onError, int limit = 50, int offset = 0, [CanBeNull] string locale = null, string additionalFields = "long_description", [CanBeNull] string country = null)
		{
			var url = string.Format(URL_CATALOG_GET_ITEMS, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, limit: limit, offset: offset, locale: locale, additionalFields: additionalFields, country: country);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, GetPersonalizationHeader(), onSuccess, onError, ErrorCheckType.ItemsListErrors);
		}

		/// <summary>
		/// Returns a full list of virtual items. The list includes items for which display in the store is enabled in the settings. For each virtual item, the SKU, name, description, and data about the groups it belongs to are returned.
		/// If used after user authentication, the method returns items that match the personalization rules for the current user.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="locale">Response language. </br>
		/// The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).<br/>
		/// Leave empty to use the default value.</param>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void GetCatalogSimplified(string projectId, Action<StoreItemShortCollection> onSuccess, Action<Error> onError = null, string locale = null)
		{
			var url = string.Format(URL_CATALOG_GET_ALL_VIRTUAL_ITEMS, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, locale: locale);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, GetPersonalizationHeader(), onSuccess, onError, ErrorCheckType.ItemsListErrors);
		}

		/// <summary>
		/// Returns a list of items for the specified group according to pagination settings. The list includes items for which display in the store is enabled in the settings. In the settings of the group, the display in the store must be enabled.
		/// If used after user authentication, the method returns items that match the personalization rules for the current user.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="groupExternalId">Group external ID.</param>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Response language.<br/>
		/// The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).</param>
		/// <param name="additionalFields">The list of additional fields. This fields will be in a response if you send its in a request. Available fields `media_list`, `order`, `long_description`.</param>
		/// <param name="country">Country used to calculate regional prices and restrictions for the catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). By default, it is determined by the user's IP address.</param>
		public void GetGroupItems(string projectId, string groupExternalId, [NotNull] Action<StoreItems> onSuccess, [CanBeNull] Action<Error> onError, int? limit = null, int? offset = null, [CanBeNull] string locale = null, string additionalFields = null, string country = null)
		{
			var url = string.Format(URL_CATALOG_GET_ITEMS_IN_GROUP, projectId, groupExternalId);
			url = UrlParameterizer.ConcatUrlAndParams(url, limit: limit, offset: offset, locale: locale, additionalFields: additionalFields, country: country);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, GetPersonalizationHeader(), onSuccess, onError, ErrorCheckType.ItemsListErrors);
		}

		/// <summary>
		/// Returns a full list of virtual item groups. The list includes groups for which display in the store is enabled in the settings.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Called after virtual item groups were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Defines localization of the item text fields.<br/>
		/// The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).</param>
		public void GetItemGroups(string projectId, [NotNull] Action<Groups> onSuccess, [CanBeNull] Action<Error> onError, int limit = 50, int offset = 0, [CanBeNull] string locale = null)
		{
			var url = string.Format(URL_CATALOG_GET_GROUPS, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, limit: limit, offset: offset, locale: locale);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, onSuccess, onError);
		}

		/// <summary>
		/// Returns a list of virtual currencies according to pagination settings.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Called after virtual currencies were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Response language.<br/>
		/// The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).</param>
		/// <param name="additionalFields">The list of additional fields. These fields will be in a response if you send them in a request. Available fields `media_list`, `order`, and `long_description`.</param>
		/// <param name="country">Country used to calculate regional prices and restrictions for the catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). By default, it is determined by the user's IP address.</param>
		public void GetVirtualCurrencyList(string projectId, [NotNull] Action<VirtualCurrencyItems> onSuccess, [CanBeNull] Action<Error> onError, int limit = 50, int offset = 0, [CanBeNull] string locale = null, string additionalFields = null, string country = null)
		{
			var url = string.Format(URL_VIRTUAL_CURRENCY_LIST, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, limit: limit, offset: offset, locale: locale, additionalFields: additionalFields, country: country);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, onSuccess, onError, ErrorCheckType.ItemsListErrors);
		}

		/// <summary>
		/// Returns a list of virtual currency packages according to pagination settings. The list includes packages for which display in the store is enabled in the settings.
		/// If used after user authentication, the method returns packages that match the personalization rules for the current user.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Called after virtual currency packages were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Response language.<br/>
		/// The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).</param>
		/// <param name="additionalFields">The list of additional fields. These fields will be in a response if you send them in a request. Available fields `media_list`, `order`, and `long_description`.</param>
		/// <param name="country">Country used to calculate regional prices and restrictions for the catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). By default, it is determined by the user's IP address.</param>
		public void GetVirtualCurrencyPackagesList(string projectId, [NotNull] Action<VirtualCurrencyPackages> onSuccess, [CanBeNull] Action<Error> onError, int limit = 50, int offset = 0, [CanBeNull] string locale = null, string additionalFields = null, string country = null)
		{
			var url = string.Format(URL_VIRTUAL_CURRENCY_PACKAGES_IN_PROJECT, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, limit: limit, offset: offset, locale: locale, additionalFields: additionalFields, country: country);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, GetPersonalizationHeader(), onSuccess, onError, ErrorCheckType.ItemsListErrors);
		}

		/// <summary>
		/// Returns a list of bundles according to pagination settings. The list includes bundles for which display in the store is enabled in the settings.
		/// If used after user authentication, the method returns items that match the personalization rules for the current user.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/#unreal_engine_sdk_how_to_bundles).</remarks>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Called after bundles are successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Defines localization of the item text fields.<br/>
		/// The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).</param>
		/// <param name="additionalFields">The list of additional fields. This fields will be in a response if you send its in a request. Available fields `media_list`, `order`, `long_description`.</param>
		/// <param name="country">Country used to calculate regional prices and restrictions for the catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). By default, it is determined by the user's IP address.</param>
		public void GetBundles(string projectId, [NotNull] Action<BundleItems> onSuccess, [CanBeNull] Action<Error> onError, int limit = 50, int offset = 0, [CanBeNull] string locale = null, string additionalFields = null, [CanBeNull] string country = null)
		{
			var url = string.Format(URL_CATALOG_GET_BUNDLES, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, limit: limit, offset: offset, locale: locale, additionalFields: additionalFields, country: country);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, GetPersonalizationHeader(), onSuccess, onError, ErrorCheckType.ItemsListErrors);
		}

		/// <summary>
		/// Returns information about the contents of the specified bundle. In the bundle settings, display in the store must be enabled.
		/// If used after user authentication, the method returns items that match the personalization rules for the current user.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/#unreal_engine_sdk_how_to_bundles).</remarks>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="sku">Bundle SKU.</param>
		/// <param name="onSuccess">Called after the cart is successfully filled.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="locale">Defines localization of the item text fields.<br/>
		/// The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).</param>
		/// <param name="country">Country used to calculate regional prices and restrictions for the catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). By default, it is determined by the user's IP address.</param>
		public void GetBundle(string projectId, string sku, [NotNull] Action<BundleItem> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, [CanBeNull] string country = null)
		{
			var url = string.Format(URL_CATALOG_GET_BUNDLE, projectId, sku);
			url = UrlParameterizer.ConcatUrlAndParams(url, locale: locale, country: country);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, GetPersonalizationHeader(), onSuccess, onError, ErrorCheckType.ItemsListErrors);
		}

		/// <summary>
		/// Redeems the coupon code and delivers a reward to the user in one of the following ways:
		/// - to their inventory (virtual items, virtual currency packages, or bundles)
		/// - via email (game keys)
		/// - to the entitlement system (game keys)
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/promo/coupons).</remarks>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="couponCode">Unique case sensitive code. Contains letters and numbers.</param>
		/// <param name="onSuccess">Called after successful coupon redemption.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void RedeemCouponCode(string projectId, CouponCode couponCode, [NotNull] Action<CouponRedeemedItems> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_INVENTORY_REDEEM_COUPON, projectId);

			var headers = new List<WebRequestHeader>() { WebRequestHeader.AuthHeader(Token.Instance), WebRequestHeader.ContentTypeHeader() };
			WebRequestHelper.Instance.PostRequest(SdkType.Store, url, couponCode, headers, onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => RedeemCouponCode(projectId, couponCode, onSuccess, onError)),
				ErrorCheckType.CouponErrors);
		}

		/// <summary>
		/// Returns a list of items that can be credited to the user when the coupon is redeemed. Can be used to let users choose one of many items as a bonus. The usual case is choosing a DRM if the coupon contains a game as a bonus.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/promo/coupons).</remarks>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="couponCode">Unique case sensitive code. Contains letters and numbers.</param>
		/// <param name="onSuccess">Called after receiving coupon rewards successfully.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public void GetCouponRewards(string projectId, string couponCode, [NotNull] Action<CouponReward> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_INVENTORY_GET_COUPON_REWARDS, projectId, couponCode);

			var headers = new List<WebRequestHeader>() { WebRequestHeader.AuthHeader(Token.Instance), WebRequestHeader.ContentTypeHeader() };
			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, headers, onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => GetCouponRewards(projectId, couponCode, onSuccess, onError)),
				ErrorCheckType.CouponErrors);
		}

		/// <summary>
		/// Creates an order with a specified item. The created order will get a `new` order status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/one-click-purchase/).</remarks>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="itemSku">Item SKU to purchase.</param>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c>, and <c>currency</c>.</param>
		/// <param name="customHeaders">Custom web request headers</param>
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
		/// Creates an order with a specified item, returns unique identifier of the created order and the Pay Station token for the purchase of the specified product by virtual currency. The created order will get a `new` order status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/purchase-for-vc/).</remarks>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="itemSku">Item SKU to purchase.</param>
		/// <param name="priceSku">Virtual currency SKU.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c> and <c>currency</c>.</param>
		/// <param name="platform">Publishing platform the user plays on.<br/>
		/// Can be `xsolla` (default), `playstation_network`, `xbox_live`, `pc_standalone`, `nintendo_shop`, `google_play`, `app_store_ios`, `android_standalone`, `ios_standalone`, `android_other`, `ios_other`, or `pc_other`.</param>
		/// <param name="customHeaders">Custom web request headers.</param>
		public void PurchaseItemForVirtualCurrency(string projectId, string itemSku, string priceSku, [CanBeNull] Action<PurchaseData> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null, string platform = null, Dictionary<string, string> customHeaders = null)
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

		/// <summary>
		/// Create order with specified free item. The created order will get a `done` order status.
		/// </summary>
		/// <remarks><remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/promo/free-items/).</remarks>.</remarks>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="itemSku">Desired free item SKU.</param>
		/// <param name="onSuccess">Called after the payment was successfully completed..</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c>, and <c>currency</c>.</param>
		/// <param name="customHeaders">Custom web request headers</param>
		public void CreateOrderWithSpecifiedFreeItem(string projectId, string itemSku, [CanBeNull] Action<int> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null, Dictionary<string, string> customHeaders = null)
		{
			var tempPurchaseParams = PurchaseParamsGenerator.GenerateTempPurchaseParams(purchaseParams);
			var url = string.Format(URL_CREATE_FREE_ORDER_WITH_ITEM, projectId, itemSku);
			var paymentHeaders = PurchaseParamsGenerator.GetPaymentHeaders(Token.Instance, customHeaders);
			WebRequestHelper.Instance.PostRequest<PurchaseData, TempPurchaseParams>(SdkType.Store, url, tempPurchaseParams, paymentHeaders,
				onComplete: purchaseData => onSuccess?.Invoke(purchaseData.order_id),
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => CreateOrderWithSpecifiedFreeItem(projectId, itemSku, onSuccess, onError, purchaseParams, customHeaders)),
				ErrorCheckType.BuyItemErrors);
		}

		private WebRequestHeader GetPersonalizationHeader() => (Token.Instance != null) ? WebRequestHeader.AuthHeader(Token.Instance) : null;
	}
}
