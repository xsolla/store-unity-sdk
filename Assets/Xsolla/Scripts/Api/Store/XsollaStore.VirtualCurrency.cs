using System;
using System.Text;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		private const string URL_VIRTUAL_CURRENCY_BALANCE = BASE_STORE_API_URL + "/user/virtual_currency_balance";
		private const string URL_VIRTUAL_CURRENCY_LIST = BASE_STORE_API_URL + "/items/virtual_currency?offset={1}&limit={2}";
		private const string URL_VIRTUAL_CURRENCY_PACKAGES_IN_PROJECT = BASE_STORE_API_URL + "/items/virtual_currency/package?offset={1}&limit={2}";

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
			var urlBuilder = new StringBuilder(string.Format(URL_VIRTUAL_CURRENCY_BALANCE, projectId)).Append(AnalyticUrlAddition);
			urlBuilder.Append(GetLocaleUrlParam(locale));
			urlBuilder.Append(GetPlatformUrlParam());

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), AuthAndAnalyticHeaders, onSuccess, onError, Error.ItemsListErrors);
		}

		/// <summary>
		/// Returns info for all virtual currencies.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get virtual currency list</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/items/get-virtual-currency"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Defines localization of item's text fields.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		public void GetVirtualCurrencyList(string projectId, [NotNull] Action<VirtualCurrencyItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, int offset = 0, int limit = 50)
		{
			var analyticUrlAddition = AnalyticUrlAddition.Replace('?', '&');
			var urlBuilder = new StringBuilder(string.Format(URL_VIRTUAL_CURRENCY_LIST, projectId, offset, limit)).Append(analyticUrlAddition);
			urlBuilder.Append(GetLocaleUrlParam(locale));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), AuthAndAnalyticHeaders, onSuccess, onError, Error.ItemsListErrors);
		}

		/// <summary>
		/// Returns virtual currency packages list.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get virtual currency package list</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/items/get-virtual-currency-package"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Defines localization of item's text fields.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		public void GetVirtualCurrencyPackagesList(string projectId, [NotNull] Action<VirtualCurrencyPackages> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, int offset = 0, int limit = 50)
		{
			var analyticUrlAddition = AnalyticUrlAddition.Replace('?', '&');
			var urlBuilder = new StringBuilder(string.Format(URL_VIRTUAL_CURRENCY_PACKAGES_IN_PROJECT, projectId, offset, limit)).Append(analyticUrlAddition);
			urlBuilder.Append(GetLocaleUrlParam(locale));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), AuthAndAnalyticHeaders, onSuccess, onError, Error.ItemsListErrors);
		}
	}
}
