using System;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		private const string URL_CATALOG_GET_ALL_VIRTUAL_ITEMS = BASE_STORE_API_URL + "/items/virtual_items/all";
		private const string URL_CATALOG_GET_ITEMS = BASE_STORE_API_URL + "/items/virtual_items?limit={1}&offset={2}";
		private const string URL_CATALOG_GET_BUNDLE = BASE_STORE_API_URL + "/items/bundle/sku/{1}";
		private const string URL_CATALOG_GET_BUNDLES = BASE_STORE_API_URL + "/items/bundle?limit={1}&offset={2}";
		private const string URL_CATALOG_GET_ITEMS_IN_GROUP = BASE_STORE_API_URL + "/items/virtual_items/group/{1}";
		private const string URL_CATALOG_GET_GROUPS = BASE_STORE_API_URL + "/items/groups?offset={1}&limit={2}";

		/// <summary>
		/// Gets a list of all virtual items for searching on client-side.
		/// </summary>
		/// <see cref="https://developers.xsolla.com/in-game-store-buy-button-api/virtual-items-currency/catalog/get-all-virtual-items/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="locale">Response language. Two-letter lowercase language code per ISO 639-1.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetCatalogShort(string projectId, Action<StoreItemShortCollection> onSuccess, Action<Error> onError = null, string locale = null)
		{
			var url = string.Format(URL_CATALOG_GET_ALL_VIRTUAL_ITEMS, projectId);
			var localeParam = GetLocaleUrlParam(locale);
			url = ConcatUrlAndParams(url, localeParam);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, onSuccess, onError, Error.ItemsListErrors);
		}

		/// <summary>
		/// Returns all items in catalog.
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
			var url = string.Format(URL_CATALOG_GET_ITEMS, projectId, limit, offset);
			var localeParam = GetLocaleUrlParam(locale);
			var additionalFieldsParam = GetAdditionalFieldsParam(additionalFields);
			var countryParam = GetCountryUrlParam(country);
			url = ConcatUrlAndParams(url, localeParam, additionalFieldsParam, countryParam);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, onSuccess, onError, Error.ItemsListErrors);
		}

		/// <summary>
		/// Returns specified bundle.
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
			var localeParam = GetLocaleUrlParam(locale);
			var countryParam = GetCountryUrlParam(country);
			url = ConcatUrlAndParams(url, localeParam, countryParam);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, onSuccess, onError, Error.ItemsListErrors);
		}

		/// <summary>
		/// Returns all bundles in a catalog.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get list of bundles</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/bundles/catalog/get-bundle-list"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Defines localization of the item text fields.</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="additionalFields">The list of additional fields. This fields will be in a response if you send its in a request. Available fields 'media_list', 'order', 'long_description'.</param>
		/// <param name="country">Country used to calculate regional prices and restrictions for the catalog. Two-letter uppercase country code per ISO 3166-1 alpha-2. If you do not specify the country explicitly, it will be defined by the user IP address.</param>
		public void GetBundles(string projectId, [NotNull] Action<BundleItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, int limit = 50, int offset = 0, string additionalFields = null, [CanBeNull] string country = null)
		{
			var url = string.Format(URL_CATALOG_GET_BUNDLES, projectId, limit, offset);
			var localeParam = GetLocaleUrlParam(locale);
			var countryParam = GetCountryUrlParam(country);
			var additionalFieldsParam = GetAdditionalFieldsParam(additionalFields);
			url = ConcatUrlAndParams(url, localeParam, countryParam, additionalFieldsParam);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, onSuccess, onError, Error.ItemsListErrors);
		}

		/// <summary>
		/// Returns items in a specific group.
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
			var limitParam = GetLimitUrlParam(limit);
			var offsetParam = GetOffsetUrlParam(offset);
			var localeParam = GetLocaleUrlParam(locale);
			var additionalFieldsParam = GetAdditionalFieldsParam(additionalFields);
			var countryParam = GetCountryUrlParam(country);
			url = ConcatUrlAndParams(url, limitParam, offsetParam, localeParam, additionalFieldsParam, countryParam);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, onSuccess, onError, Error.ItemsListErrors);
		}

		/// <summary>
		/// Returns groups list.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get items groups list</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/groups/get-item-groups"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Defines localization of the item text fields.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		public void GetItemGroups(string projectId, [NotNull] Action<Groups> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, int offset = 0, int limit = 50)
		{
			var url = string.Format(URL_CATALOG_GET_GROUPS, projectId, offset, limit);
			var localeParam = GetLocaleUrlParam(locale);
			url = ConcatUrlAndParams(url, localeParam);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, onSuccess, onError);
		}
	}
}
