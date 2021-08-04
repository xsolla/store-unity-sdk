using System;
using System.Text;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		private const string URL_VIRTUAL_CURRENCY_BALANCE = BASE_STORE_API_URL + "/user/virtual_currency_balance";
		private const string URL_VIRTUAL_CURRENCY_LIST = BASE_STORE_API_URL + "/items/virtual_currency?limit={1}&offset={2}";
		private const string URL_VIRTUAL_CURRENCY_PACKAGES_IN_PROJECT = BASE_STORE_API_URL + "/items/virtual_currency/package?limit={1}&offset={2}";

		/// <summary>
		/// Returns balance for all virtual currencies.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get the current user's virtual balance</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/inventory-client/get-user-virtual-balance"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Defines localization of item's text fields.</param>
		public void GetVirtualCurrencyBalance(string projectId, [NotNull] Action<VirtualCurrenciesBalance> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null)
		{
			var url = string.Format(URL_VIRTUAL_CURRENCY_BALANCE, projectId);
			var localeParam = GetLocaleUrlParam(locale);
			var platformParam = GetPlatformUrlParam();
			url = ConcatUrlAndParams(url, localeParam, platformParam);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError, Error.ItemsListErrors);
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
			var url = string.Format(URL_VIRTUAL_CURRENCY_LIST, projectId, limit, offset);
			var localeParam = GetLocaleUrlParam(locale);
			var additionalFieldsParam = GetAdditionalFieldsParam(additionalFields);
			var countryParam = GetCountryUrlParam(country);
			url = ConcatUrlAndParams(url, localeParam, additionalFieldsParam, countryParam);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError, Error.ItemsListErrors);
		}

		/// <summary>
		/// Returns the list of virtual currency packages.
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
			var url = string.Format(URL_VIRTUAL_CURRENCY_PACKAGES_IN_PROJECT, projectId, limit, offset);
			var localeParam = GetLocaleUrlParam(locale);
			var additionalFieldsParam = GetAdditionalFieldsParam(additionalFields);
			var countryParam = GetCountryUrlParam(country);
			url = ConcatUrlAndParams(url, localeParam, additionalFieldsParam, countryParam);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError, Error.ItemsListErrors);
		}
	}
}
