using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.Catalog
{
	public static class XsollaCatalog
	{
		private static string BaseUrl => $"https://store.xsolla.com/api/v2/project/{XsollaSettings.StoreProjectId}";

		/// <summary>
		/// Returns a full list of virtual items.
		/// The list includes items for which display in the store is enabled in the settings. For each virtual item, complete data is returned.
		/// If called after user authentication, the method returns items that match the personalization rules for the current user.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="onSuccess">Called after virtual items were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="locale">Response language. [Two-letter lowercase language code](https://developers.xsolla.com/doc/pay-station/features/localization/). Leave empty to use the default value.</param>
		/// <param name="country">Country for which to calculate regional prices and restrictions in a catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). Calculations are based on the user's IP address if the country is not specified.  Check the documentation for detailed information about [countries supported by Xsolla](https://developers.xsolla.com/doc/in-game-store/references/supported-countries/).</param>
		/// <param name="additionalFields">The list of additional fields. These fields will be in a response if you send them in a request. Available fields `media_list`, `order`, and `long_description`.</param>
		/// <param name="sdkType">SDK type. Used for internal analytics.</param>
		public static void GetItems(Action<StoreItems> onSuccess, Action<Error> onError, string locale = null, string country = null, string additionalFields = "long_description", SdkType sdkType = SdkType.Store)
		{
			var items = new List<StoreItem>();
			const int limit = 50;
			var offset = 0;

			processRequest();
			return;

			void processRequest()
			{
				GetPaginatedItems(
					handleResponse,
					onError,
					limit,
					offset,
					locale,
					country,
					additionalFields,
					sdkType);
			}

			void handleResponse(StoreItems response)
			{
				items.AddRange(response.items);

				if (!response.has_more)
				{
					onSuccess(new StoreItems {
						has_more = false,
						items = items.ToArray()
					});
				}
				else
				{
					offset += limit;
					processRequest();
				}
			}
		}

		/// <summary>
		/// Returns a list of virtual items according to pagination settings.
		/// The list includes items for which display in the store is enabled in the settings. For each virtual item, complete data is returned.
		/// If called after user authentication, the method returns items that match the personalization rules for the current user.
		/// <b>Attention:</b> The number of items returned in a single response is limited. <b>The default and maximum value is 50 items per response</b>. To get more data page by page, use <code>limit</code> and <code>offset</code> fields.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="onSuccess">Called after virtual items were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page. The maximum number of elements on a page is 50.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Response language. [Two-letter lowercase language code](https://developers.xsolla.com/doc/pay-station/features/localization/). Leave empty to use the default value.</param>
		/// <param name="country">Country for which to calculate regional prices and restrictions in a catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). Calculations are based on the user's IP address if the country is not specified.  Check the documentation for detailed information about [countries supported by Xsolla](https://developers.xsolla.com/doc/in-game-store/references/supported-countries/).</param>
		/// <param name="additionalFields">The list of additional fields. These fields will be in a response if you send them in a request. Available fields `media_list`, `order`, and `long_description`.</param>
		/// <param name="sdkType">SDK type. Used for internal analytics.</param>
		public static void GetPaginatedItems(Action<StoreItems> onSuccess, Action<Error> onError, int limit, int offset, string locale = null, string country = null, string additionalFields = "long_description", SdkType sdkType = SdkType.Store)
		{
			var url = new UrlBuilder($"{BaseUrl}/items/virtual_items")
				.AddLimit(limit)
				.AddOffset(offset)
				.AddLocale(locale)
				.AddCountry(country)
				.AddAdditionalFields(additionalFields)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				sdkType,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetPaginatedItems(onSuccess, onError, limit, offset, locale, country, additionalFields)),
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Returns a full list of virtual items. The list includes items for which display in the store is enabled in the settings. For each virtual item, the SKU, name, description, and data about the groups it belongs to are returned.
		/// If used after user authentication, the method returns items that match the personalization rules for the current user.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="locale">Response language. [Two-letter lowercase language code](https://developers.xsolla.com/doc/pay-station/features/localization/). Leave empty to use the default value.</param>
		public static void GetCatalogSimplified(Action<StoreShortItems> onSuccess, Action<Error> onError, string locale = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/items/virtual_items/all")
				.AddLocale(locale)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetCatalogSimplified(onSuccess, onError, locale)),
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Returns a list of items for the specified group. The list includes items for which display in the store is enabled in the settings. In the settings of the group, the display in the store must be enabled.
		/// If called after user authentication, the method returns items that match the personalization rules for the current user.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="groupExternalId">Group external ID.</param>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="locale">Response language. [Two-letter lowercase language code](https://developers.xsolla.com/doc/pay-station/features/localization/). Leave empty to use the default value.</param>
		/// <param name="country">Country for which to calculate regional prices and restrictions in a catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). Calculations are based on the user's IP address if the country is not specified.  Check the documentation for detailed information about [countries supported by Xsolla](https://developers.xsolla.com/doc/in-game-store/references/supported-countries/).</param>
		/// <param name="additionalFields">The list of additional fields. This fields will be in a response if you send its in a request. Available fields `media_list`, `order`, `long_description`.</param>
		public static void GetGroupItems(string groupExternalId, Action<StoreItems> onSuccess, Action<Error> onError, string locale = null, string country = null, string additionalFields = null)
		{
			var items = new List<StoreItem>();
			const int limit = 50;
			var offset = 0;

			processRequest();
			return;

			void processRequest()
			{
				GetPaginatedGroupItems(
					groupExternalId,
					handleResponse,
					onError,
					limit,
					offset,
					locale,
					country,
					additionalFields);
			}

			void handleResponse(StoreItems response)
			{
				items.AddRange(response.items);

				if (!response.has_more)
				{
					onSuccess(new StoreItems {
						has_more = false,
						items = items.ToArray()
					});
				}
				else
				{
					offset += limit;
					processRequest();
				}
			}
		}

		/// <summary>
		/// Returns a list of items for the specified group according to pagination settings. The list includes items for which display in the store is enabled in the settings. In the settings of the group, the display in the store must be enabled.
		/// If called after user authentication, the method returns items that match the personalization rules for the current user.
		/// <b>Attention:</b> The number of items returned in a single response is limited. <b>The default and maximum value is 50 items per response</b>. To get more data page by page, use <code>limit</code> and <code>offset</code> fields.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="groupExternalId">Group external ID.</param>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page. The maximum number of elements on a page is 50.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Response language. [Two-letter lowercase language code](https://developers.xsolla.com/doc/pay-station/features/localization/). Leave empty to use the default value.</param>
		/// <param name="country">Country for which to calculate regional prices and restrictions in a catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). Calculations are based on the user's IP address if the country is not specified.  Check the documentation for detailed information about [countries supported by Xsolla](https://developers.xsolla.com/doc/in-game-store/references/supported-countries/).</param>
		/// <param name="additionalFields">The list of additional fields. This fields will be in a response if you send its in a request. Available fields `media_list`, `order`, `long_description`.</param>
		public static void GetPaginatedGroupItems(string groupExternalId, Action<StoreItems> onSuccess, Action<Error> onError, int limit, int offset, string locale = null, string country = null, string additionalFields = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/items/virtual_items/group/{groupExternalId}")
				.AddLimit(limit)
				.AddOffset(offset)
				.AddLocale(locale)
				.AddCountry(country)
				.AddAdditionalFields(additionalFields)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetPaginatedGroupItems(groupExternalId, onSuccess, onError, limit, offset, locale, country, additionalFields)),
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Returns a full list of virtual item groups. The list includes groups for which display in the store is enabled in the settings.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="onSuccess">Called after virtual item groups were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="locale">Defines localization of the item text fields.[Two-letter lowercase language code](https://developers.xsolla.com/doc/pay-station/features/localization/). Leave empty to use the default value.</param>
		/// <param name="promoCode">Promo code. Unique case-sensitive code. Contains letters and numbers.</param>
		public static void GetItemGroups(Action<Groups> onSuccess, Action<Error> onError, string locale = null, string promoCode = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/items/groups")
				.AddLocale(locale)
				.AddParam("promo_code", promoCode)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetItemGroups(onSuccess, onError, locale)));
		}

		/// <summary>
		/// Returns a full list of virtual currencies. The list includes currencies for which display in the store is enabled in settings.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="onSuccess">Called after virtual currencies were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="locale">Response language. [Two-letter lowercase language code](https://developers.xsolla.com/doc/pay-station/features/localization/). Leave empty to use the default value.</param>
		/// <param name="country">Country for which to calculate regional prices and restrictions in a catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). Calculations are based on the user's IP address if the country is not specified.  Check the documentation for detailed information about [countries supported by Xsolla](https://developers.xsolla.com/doc/in-game-store/references/supported-countries/).</param>
		/// <param name="additionalFields">The list of additional fields. These fields will be in a response if you send them in a request. Available fields `media_list`, `order`, and `long_description`.</param>
		public static void GetVirtualCurrencyList(Action<VirtualCurrencyItems> onSuccess, Action<Error> onError, string locale = null, string country = null, string additionalFields = null)
		{
			var items = new List<VirtualCurrencyItem>();
			const int limit = 50;
			var offset = 0;

			processRequest();
			return;

			void processRequest()
			{
				GetPaginatedVirtualCurrencyList(
					handleResponse,
					onError,
					limit,
					offset,
					locale,
					country,
					additionalFields);
			}

			void handleResponse(VirtualCurrencyItems response)
			{
				items.AddRange(response.items);

				if (!response.has_more)
				{
					onSuccess(new VirtualCurrencyItems {
						has_more = false,
						items = items.ToArray()
					});
				}
				else
				{
					offset += limit;
					processRequest();
				}
			}
		}

		/// <summary>
		/// Returns a list of virtual currencies according to pagination settings. The list includes currencies for which display in the store is enabled in settings.
		/// <b>Attention:</b> The number of currencies returned in a single response is limited. <b>The default and maximum value is 50 currencies per response</b>. To get more data page by page, use <code>limit</code> and <code>offset</code> fields.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="onSuccess">Called after virtual currencies were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page. The maximum number of elements on a page is 50.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Response language. [Two-letter lowercase language code](https://developers.xsolla.com/doc/pay-station/features/localization/). Leave empty to use the default value.</param>
		/// <param name="country">Country for which to calculate regional prices and restrictions in a catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). Calculations are based on the user's IP address if the country is not specified.  Check the documentation for detailed information about [countries supported by Xsolla](https://developers.xsolla.com/doc/in-game-store/references/supported-countries/).</param>
		/// <param name="additionalFields">The list of additional fields. These fields will be in a response if you send them in a request. Available fields `media_list`, `order`, and `long_description`.</param>
		public static void GetPaginatedVirtualCurrencyList(Action<VirtualCurrencyItems> onSuccess, Action<Error> onError, int limit, int offset, string locale = null, string country = null, string additionalFields = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/items/virtual_currency")
				.AddLimit(limit)
				.AddOffset(offset)
				.AddLocale(locale)
				.AddCountry(country)
				.AddAdditionalFields(additionalFields)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetPaginatedVirtualCurrencyList(onSuccess, onError, limit, offset, locale, country, additionalFields)),
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Returns a list of full virtual currency packages.
		/// The list includes packages for which display in the store is enabled in the settings.
		/// If called after user authentication, the method returns packages that match the personalization rules for the current user.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="onSuccess">Called after virtual currency packages were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="locale">Response language. [Two-letter lowercase language code](https://developers.xsolla.com/doc/pay-station/features/localization/). Leave empty to use the default value.</param>
		/// <param name="country">Country for which to calculate regional prices and restrictions in a catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). Calculations are based on the user's IP address if the country is not specified.  Check the documentation for detailed information about [countries supported by Xsolla](https://developers.xsolla.com/doc/in-game-store/references/supported-countries/).</param>
		/// <param name="additionalFields">The list of additional fields. These fields will be in a response if you send them in a request. Available fields `media_list`, `order`, and `long_description`.</param>
		/// <param name="sdkType">SDK type. Used for internal analytics.</param>
		public static void GetVirtualCurrencyPackagesList(Action<VirtualCurrencyPackages> onSuccess, Action<Error> onError, string locale = null, string country = null, string additionalFields = null, SdkType sdkType = SdkType.Store)
		{
			var items = new List<VirtualCurrencyPackage>();
			const int limit = 50;
			var offset = 0;

			processRequest();
			return;

			void processRequest()
			{
				GetPaginatedVirtualCurrencyPackagesList(
					handleResponse,
					onError,
					limit,
					offset,
					locale,
					country,
					additionalFields,
					sdkType);
			}

			void handleResponse(VirtualCurrencyPackages response)
			{
				items.AddRange(response.items);

				if (!response.has_more)
				{
					onSuccess(new VirtualCurrencyPackages {
						has_more = false,
						items = items.ToArray()
					});
				}
				else
				{
					offset += limit;
					processRequest();
				}
			}
		}

		/// <summary>
		/// Returns a list of virtual currency packages according to pagination settings.
		/// The list includes packages for which display in the store is enabled in the settings.
		/// If called after user authentication, the method returns packages that match the personalization rules for the current user.
		/// <b>Attention:</b> The number of packages returned in a single response is limited. <b>The default and maximum value is 50 packages per response</b>. To get more data page by page, use <code>limit</code> and <code>offset</code> fields.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="onSuccess">Called after virtual currency packages were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page. The maximum number of elements on a page is 50.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Response language. [Two-letter lowercase language code](https://developers.xsolla.com/doc/pay-station/features/localization/). Leave empty to use the default value.</param>
		/// <param name="country">Country for which to calculate regional prices and restrictions in a catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). Calculations are based on the user's IP address if the country is not specified.  Check the documentation for detailed information about [countries supported by Xsolla](https://developers.xsolla.com/doc/in-game-store/references/supported-countries/).</param>
		/// <param name="additionalFields">The list of additional fields. These fields will be in a response if you send them in a request. Available fields `media_list`, `order`, and `long_description`.</param>
		/// <param name="sdkType">SDK type. Used for internal analytics.</param>
		public static void GetPaginatedVirtualCurrencyPackagesList(Action<VirtualCurrencyPackages> onSuccess, Action<Error> onError, int limit, int offset, string locale = null, string country = null, string additionalFields = null, SdkType sdkType = SdkType.Store)
		{
			var url = new UrlBuilder($"{BaseUrl}/items/virtual_currency/package")
				.AddLimit(limit)
				.AddOffset(offset)
				.AddLocale(locale)
				.AddCountry(country)
				.AddAdditionalFields(additionalFields)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				sdkType,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetPaginatedVirtualCurrencyPackagesList(onSuccess, onError, limit, offset, locale, country, additionalFields)),
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Returns a full  list of bundles.
		/// The list includes bundles for which display in the store is enabled in the settings.
		/// If called after user authentication, the method returns items that match the personalization rules for the current user.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/#unreal_engine_sdk_how_to_bundles).</remarks>
		/// <param name="onSuccess">Called after bundles are successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="locale">Defines localization of the item text fields. [Two-letter lowercase language code](https://developers.xsolla.com/doc/pay-station/features/localization/). Leave empty to use the default value.</param>
		/// <param name="country">Country for which to calculate regional prices and restrictions in a catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). Calculations are based on the user's IP address if the country is not specified.  Check the documentation for detailed information about [countries supported by Xsolla](https://developers.xsolla.com/doc/in-game-store/references/supported-countries/).</param>
		/// <param name="additionalFields">The list of additional fields. This fields will be in a response if you send its in a request. Available fields `media_list`, `order`, `long_description`.</param>
		public static void GetBundles(Action<BundleItems> onSuccess, Action<Error> onError, string locale = null, string country = null, string additionalFields = null)
		{
			var items = new List<BundleItem>();
			const int limit = 50;
			var offset = 0;

			processRequest();
			return;

			void processRequest()
			{
				GetPaginatedBundles(
					handleResponse,
					onError,
					limit,
					offset,
					locale,
					country,
					additionalFields);
			}

			void handleResponse(BundleItems response)
			{
				items.AddRange(response.items);

				if (!response.has_more)
				{
					onSuccess(new BundleItems {
						has_more = false,
						items = items.ToArray()
					});
				}
				else
				{
					offset += limit;
					processRequest();
				}
			}
		}

		/// <summary>
		/// Returns a list of bundles according to pagination settings.
		/// The list includes bundles for which display in the store is enabled in the settings.
		/// If called after user authentication, the method returns items that match the personalization rules for the current user.
		/// <b>Attention:</b> The number of bundles returned in a single response is limited. <b>The default and maximum value is 50 bundles per response</b>. To get more data page by page, use <code>limit</code> and <code>offset</code> fields. 
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/#unreal_engine_sdk_how_to_bundles).</remarks>
		/// <param name="onSuccess">Called after bundles are successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page. The maximum number of elements on a page is 50.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Defines localization of the item text fields. [Two-letter lowercase language code](https://developers.xsolla.com/doc/pay-station/features/localization/). Leave empty to use the default value.</param>
		/// <param name="country">Country for which to calculate regional prices and restrictions in a catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). Calculations are based on the user's IP address if the country is not specified.  Check the documentation for detailed information about [countries supported by Xsolla](https://developers.xsolla.com/doc/in-game-store/references/supported-countries/).</param>
		/// <param name="additionalFields">The list of additional fields. This fields will be in a response if you send its in a request. Available fields `media_list`, `order`, `long_description`.</param>
		public static void GetPaginatedBundles(Action<BundleItems> onSuccess, Action<Error> onError, int limit, int offset, string locale = null, string country = null, string additionalFields = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/items/bundle")
				.AddLimit(limit)
				.AddOffset(offset)
				.AddLocale(locale)
				.AddCountry(country)
				.AddAdditionalFields(additionalFields)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetPaginatedBundles(onSuccess, onError, limit, offset, locale, country, additionalFields)),
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Returns information about the contents of the specified bundle. In the bundle settings, display in the store must be enabled.
		/// If used after user authentication, the method returns items that match the personalization rules for the current user.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/#unreal_engine_sdk_how_to_bundles).</remarks>
		/// <param name="sku">Bundle SKU.</param>
		/// <param name="onSuccess">Called after the cart is successfully filled.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="locale">Defines localization of the item text fields. [Two-letter lowercase language code](https://developers.xsolla.com/doc/pay-station/features/localization/). Leave empty to use the default value.</param>
		/// <param name="country">Country for which to calculate regional prices and restrictions in a catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). Calculations are based on the user's IP address if the country is not specified.  Check the documentation for detailed information about [countries supported by Xsolla](https://developers.xsolla.com/doc/in-game-store/references/supported-countries/).</param>
		public static void GetBundle(string sku, Action<BundleItem> onSuccess, Action<Error> onError, string locale = null, string country = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/items/bundle/sku/{sku}")
				.AddLocale(locale)
				.AddCountry(country)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetBundle(sku, onSuccess, onError, locale, country)),
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Redeems the coupon code and delivers a reward to the user in one of the following ways:
		/// - to their inventory (virtual items, virtual currency packages, or bundles)
		/// - via email (game keys)
		/// - to the entitlement system (game keys)
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/promo/coupons).</remarks>
		/// <param name="couponCode">Unique case sensitive code. Contains letters and numbers.</param>
		/// <param name="onSuccess">Called after successful coupon redemption.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void RedeemCouponCode(string couponCode, Action<CouponRedeemedItems> onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/coupon/redeem";

			var headers = new List<WebRequestHeader> {
				WebRequestHeader.AuthHeader(),
				WebRequestHeader.JsonContentTypeHeader()
			};

			var requestData = new CouponCodeRequest {
				coupon_code = couponCode
			};

			WebRequestHelper.Instance.PostRequest(
				SdkType.Store,
				url,
				requestData,
				headers,
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => RedeemCouponCode(couponCode, onSuccess, onError)),
				ErrorGroup.CouponErrors);
		}

		/// <summary>
		/// Returns a list of items that can be credited to the user when the coupon is redeemed. Can be used to let users choose one of many items as a bonus. The usual case is choosing a DRM if the coupon contains a game as a bonus.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/promo/coupons).</remarks>
		/// <param name="couponCode">Unique case sensitive code. Contains letters and numbers.</param>
		/// <param name="onSuccess">Called after receiving coupon rewards successfully.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void GetCouponRewards(string couponCode, Action<CouponReward> onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/coupon/code/{couponCode}/rewards";

			var headers = new List<WebRequestHeader> {
				WebRequestHeader.AuthHeader(),
				WebRequestHeader.JsonContentTypeHeader()
			};

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				headers,
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetCouponRewards(couponCode, onSuccess, onError)),
				ErrorGroup.CouponErrors);
		}

		/// <summary>
		/// Creates an order with a specified item. The created order will get a `new` order status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/one-click-purchase/).</remarks>
		/// <param name="itemSku">Item SKU to purchase.</param>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c>, and <c>currency</c>.</param>
		/// <param name="customHeaders">Custom web request headers</param>
		/// <param name="sdkType">SDK type. Used for internal analytics.</param>
		public static void CreateOrder(string itemSku, Action<OrderData> onSuccess, Action<Error> onError, PurchaseParams purchaseParams = null, Dictionary<string, string> customHeaders = null, SdkType sdkType = SdkType.Store)
		{
			if (CreateOrderCooldown.IsActive)
				return;

			CreateOrderCooldown.Start();

			var url = $"{BaseUrl}/payment/item/{itemSku}";
			var requestData = PurchaseParamsGenerator.GeneratePurchaseParamsRequest(purchaseParams);
			var headers = PurchaseParamsGenerator.GeneratePaymentHeaders(customHeaders);

			WebRequestHelper.Instance.PostRequest<OrderData, PurchaseParamsRequest>(
				sdkType,
				url,
				requestData,
				headers,
				orderData => {
					CreateOrderCooldown.Cancel();
					onSuccess?.Invoke(orderData);
				},
				error => {
					CreateOrderCooldown.Cancel();
					TokenAutoRefresher.Check(error, onError, () => CreateOrder(itemSku, onSuccess, onError, purchaseParams, customHeaders));
				},
				ErrorGroup.BuyItemErrors);
		}

		/// <summary>
		/// Creates an order with a specified item, returns unique identifier of the created order and the Pay Station token for the purchase of the specified product by virtual currency. The created order will get a `new` order status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/purchase-for-vc/).</remarks>
		/// <param name="itemSku">Item SKU to purchase.</param>
		/// <param name="priceSku">Virtual currency SKU.</param>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c>, <c>currency</c>, and <c>quantity</c>.</param>
		/// <param name="platform">Publishing platform the user plays on.<br/>
		///     Can be `xsolla` (default), `playstation_network`, `xbox_live`, `pc_standalone`, `nintendo_shop`, `google_play`, `app_store_ios`, `android_standalone`, `ios_standalone`, `android_other`, `ios_other`, or `pc_other`.</param>
		/// <param name="customHeaders">Custom HTTP request headers.</param>
		public static void CreateOrderByVirtualCurrency(string itemSku, string priceSku, Action<OrderId> onSuccess, Action<Error> onError, PurchaseParams purchaseParams = null, string platform = null, Dictionary<string, string> customHeaders = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/payment/item/{itemSku}/virtual/{priceSku}")
				.AddPlatform(platform)
				.Build();

			var requestData = new PurchaseParamsRequest {
				sandbox = XsollaSettings.IsSandbox,
				custom_parameters = purchaseParams?.custom_parameters
			};

			var headers = PurchaseParamsGenerator.GeneratePaymentHeaders(customHeaders);

			WebRequestHelper.Instance.PostRequest(
				SdkType.Store,
				url,
				requestData,
				headers,
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => CreateOrderByVirtualCurrency(itemSku, priceSku, onSuccess, onError, purchaseParams, platform, customHeaders)),
				ErrorGroup.BuyItemErrors);
		}

		/// <summary>
		/// Create order with specified free item. The created order will get a `done` order status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/promo/free-items/).</remarks>
		/// <param name="itemSku">Desired free item SKU.</param>
		/// <param name="onSuccess">Called after the order was successfully created.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c>, and <c>currency</c>.</param>
		/// <param name="customHeaders">Custom web request headers</param>
		public static void CreateOrderWithFreeItem(string itemSku, Action<OrderId> onSuccess, Action<Error> onError, PurchaseParams purchaseParams = null, Dictionary<string, string> customHeaders = null)
		{
			var url = $"{BaseUrl}/free/item/{itemSku}";
			var requestData = PurchaseParamsGenerator.GeneratePurchaseParamsRequest(purchaseParams);
			var headers = PurchaseParamsGenerator.GeneratePaymentHeaders(customHeaders);

			WebRequestHelper.Instance.PostRequest<OrderId, PurchaseParamsRequest>(
				SdkType.Store,
				url,
				requestData,
				headers,
				purchaseData => onSuccess?.Invoke(purchaseData),
				error => TokenAutoRefresher.Check(error, onError, () => CreateOrderWithFreeItem(itemSku, onSuccess, onError, purchaseParams, customHeaders)),
				ErrorGroup.BuyItemErrors);
		}

		/// <summary>
		/// Launches purchase process for a specified item. This method encapsulates methods for creating an order, opening a payment UI, and tracking the order status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/one-click-purchase/).</remarks>
		/// <param name="itemSku">Desired free item SKU.</param>
		/// <param name="onSuccess">Called after the order transitions to the 'done' status.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="onOrderCreated">Called after the order is created.</param>
		/// <param name="onBrowseClosed">Called after the browser is closed. The event is tracked only when the payment UI is opened in the built-in browser. External browser events can't be tracked.</param>
		/// <param name="purchaseParams">Purchase and payment UI parameters, such as <c>locale</c>, <c>currency</c>, etc.</param>
		/// <param name="customHeaders">Custom web request headers</param>
		/// <param name="platformSpecificAppearance">Additional settings of payment UI appearance for different platforms.</param>
		/// <param name="sdkType">SDK type. Used for internal analytics.</param>
		public static void Purchase(string itemSku, Action<OrderStatus> onSuccess, Action<Error> onError, Action<OrderData> onOrderCreated = null, Action<BrowserCloseInfo> onBrowseClosed = null, PurchaseParams purchaseParams = null, Dictionary<string, string> customHeaders = null, PlatformSpecificAppearance platformSpecificAppearance = null, SdkType sdkType = SdkType.Store)
		{
			CreateOrder(
				itemSku,
				orderData => {
					onOrderCreated?.Invoke(orderData);

					XsollaWebBrowser.OpenPurchaseUI(
						orderData.token,
						false,
						onBrowseClosed,
						platformSpecificAppearance,
						sdkType);

					OrderTrackingService.AddOrderForTracking(
						orderData.order_id,
						true,
						() => {
							XsollaWebBrowser.Close();
							OrderStatusService.GetOrderStatus(orderData.order_id, onSuccess, onError, sdkType);
						},
						onError,
						sdkType);
				},
				onError,
				purchaseParams,
				customHeaders,
				sdkType);
		}

		/// <summary>
		/// Launch purchase process for a specified item by virtual currency. This method encapsulates methods for creating an order and tracking the order status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/purchase-for-vc/).</remarks>
		/// <param name="itemSku">Desired free item SKU.</param>
		/// <param name="priceSku">Virtual currency SKU.</param>
		/// <param name="onSuccess">Called after the order transitions to the 'done' status.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="onOrderCreated">Called after the order is created.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c>, and <c>currency</c>.</param>
		/// <param name="platform">Publishing platform the user plays on.<br/>
		///     Can be `xsolla` (default), `playstation_network`, `xbox_live`, `pc_standalone`, `nintendo_shop`, `google_play`, `app_store_ios`, `android_standalone`, `ios_standalone`, `android_other`, `ios_other`, or `pc_other`.</param>
		/// <param name="customHeaders">Custom HTTP request headers.</param>
		/// <param name="sdkType">SDK type. Used for internal analytics.</param>
		public static void PurchaseForVirtualCurrency(string itemSku, string priceSku, Action<OrderStatus> onSuccess, Action<Error> onError, Action<OrderId> onOrderCreated = null, PurchaseParams purchaseParams = null, string platform = null, Dictionary<string, string> customHeaders = null, SdkType sdkType = SdkType.Store)
		{
			CreateOrderByVirtualCurrency(
				itemSku,
				priceSku,
				orderId => {
					onOrderCreated?.Invoke(orderId);

					OrderTrackingService.AddOrderForTracking(
						orderId.order_id,
						false,
						() => OrderStatusService.GetOrderStatus(orderId.order_id, onSuccess, onError, sdkType),
						onError,
						sdkType);
				},
				onError,
				purchaseParams,
				platform,
				customHeaders);
		}

		/// <summary>
		/// Launches purchase process for a specified free item. This method encapsulates methods for creating an order and tracking the order status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/promo/free-items/).</remarks>
		/// <param name="itemSku">Desired free item SKU.</param>
		/// <param name="onSuccess">Called after the order transitions to the 'done' status.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="onOrderCreated">Called after the order is created.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c>, and <c>currency</c>.</param>
		/// <param name="customHeaders">Custom HTTP request headers.</param>
		/// <param name="sdkType">SDK type. Used for internal analytics.</param>
		public static void PurchaseFreeItem(string itemSku, Action<OrderStatus> onSuccess, Action<Error> onError, Action<OrderId> onOrderCreated = null, PurchaseParams purchaseParams = null, Dictionary<string, string> customHeaders = null, SdkType sdkType = SdkType.Store)
		{
			CreateOrderWithFreeItem(
				itemSku,
				orderId => {
					onOrderCreated?.Invoke(orderId);

					OrderTrackingService.AddOrderForTracking(
						orderId.order_id,
						false,
						() => OrderStatusService.GetOrderStatus(orderId.order_id, onSuccess, onError, sdkType),
						onError,
						sdkType);
				},
				onError,
				purchaseParams,
				customHeaders);
		}

		/// <summary>
		/// [Obsolete. Use GetItems instead.] Returns a list of virtual items according to pagination settings.
		/// The list includes items for which display in the store is enabled in the settings. For each virtual item, complete data is returned.
		/// If used after user authentication, the method returns items that match the personalization rules for the current user.
		/// <b>Attention:</b> The number of items returned in a single response is limited. <b>The default and maximum value is 50 items per response</b>. To get more data page by page, use <code>limit</code> and <code>offset</code> fields.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="onSuccess">Called after virtual items were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page. The maximum number of elements on a page is 50.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Response language. [Two-letter lowercase language code](https://developers.xsolla.com/doc/pay-station/features/localization/). Leave empty to use the default value.</param>
		/// <param name="country">Country for which to calculate regional prices and restrictions in a catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). Calculations are based on the user's IP address if the country is not specified.  Check the documentation for detailed information about [countries supported by Xsolla](https://developers.xsolla.com/doc/in-game-store/references/supported-countries/).</param>
		/// <param name="additionalFields">The list of additional fields. These fields will be in a response if you send them in a request. Available fields `media_list`, `order`, and `long_description`.</param>
		[Obsolete("Use GetItems instead.")]
		public static void GetCatalog(Action<StoreItems> onSuccess, Action<Error> onError, int limit = 50, int offset = 0, string locale = null, string country = null, string additionalFields = "long_description")
		{
			var url = new UrlBuilder($"{BaseUrl}/items/virtual_items")
				.AddLimit(limit)
				.AddOffset(offset)
				.AddLocale(locale)
				.AddCountry(country)
				.AddAdditionalFields(additionalFields)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetCatalog(onSuccess, onError, limit, offset, locale, country, additionalFields)),
				ErrorGroup.ItemsListErrors);
		}
	}
}