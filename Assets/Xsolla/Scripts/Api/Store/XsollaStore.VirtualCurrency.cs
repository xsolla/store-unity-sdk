using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		private const string URL_VIRTUAL_CURRENCY_BALANCE = "https://store.xsolla.com/api/v2/project/{0}/user/virtual_currency_balance";
		private const string URL_VIRTUAL_CURRENCY_LIST = "https://store.xsolla.com/api/v2/project/{0}/items/virtual_currency";
		private const string URL_VIRTUAL_CURRENCY_PACKAGES_IN_PROJECT = "https://store.xsolla.com/api/v2/project/{0}/items/virtual_currency/package";

		public void GetVirtualCurrencyBalance(string projectId, [NotNull] Action<UserVirtualCurrenciesBalance> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_VIRTUAL_CURRENCY_BALANCE, projectId)).Append(AdditionalUrlParams);
			urlBuilder.Append(GetLocaleUrlParam(locale));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.ItemsListErrors);
		}

		public void GetVirtualCurrencyList(string projectId, [NotNull] Action<StoreItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_VIRTUAL_CURRENCY_LIST, projectId)).Append(AdditionalUrlParams);
			urlBuilder.Append(GetLocaleUrlParam(locale));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.ItemsListErrors);
		}

		// TODO: coming soon.
		//private const string URL_VIRTUAL_CURRENCY_INFO = "https://store.xsolla.com/api/v2/project/{0}/items/virtual_currency/sku/{1}";
		//public void GetVirtualCurrencyInfo(string projectId, string sku, [NotNull] Action<InventoryItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null)
		//{
		//	var urlBuilder = new StringBuilder(string.Format(URL_VIRTUAL_CURRENCY_INFO, projectId, sku)).Append(AdditionalUrlParams);
		//	urlBuilder.Append(GetLocaleUrlParam(locale));

		//	WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.ItemsListErrors);
		//}

		public void GetVirtualCurrencyPackagesList(string projectId, [NotNull] Action<StoreItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_VIRTUAL_CURRENCY_PACKAGES_IN_PROJECT, projectId)).Append(AdditionalUrlParams);
			urlBuilder.Append(GetLocaleUrlParam(locale));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.ItemsListErrors);
		}

		// TODO: coming soon.
		//private const string URL_VIRTUAL_CURRENCY_PACKAGES = "https://store.xsolla.com/api/v2/project/{0}/items/virtual_currency/package/sku/{1}";
		//public void GetVirtualCurrencyPackagesBySku(string projectId, string sku, [NotNull] Action<InventoryItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null)
		//{
		//	var urlBuilder = new StringBuilder(string.Format(URL_VIRTUAL_CURRENCY_PACKAGES, projectId, sku)).Append(AdditionalUrlParams);
		//	urlBuilder.Append(GetLocaleUrlParam(locale));

		//	WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.ItemsListErrors);
		//}
	}
}