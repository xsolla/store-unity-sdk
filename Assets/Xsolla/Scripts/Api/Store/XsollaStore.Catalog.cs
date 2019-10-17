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
		private const string URL_CATALOG_GET_ITEMS = "https://store.xsolla.com/api/v2/project/{0}/items/virtual_items";
		private const string URL_CATALOG_GET_ITEMS_IN_GROUP = "https://store.xsolla.com/api/v1/project/{0}/items/virtual_items/group/{1}";
		private const string URL_CATALOG_GET_GROUPS = "https://store.xsolla.com/api/v1/project/{0}/items/groups";

		public void GetListOfItems(string projectId, [NotNull] Action<StoreItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, [CanBeNull] string currency = null)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CATALOG_GET_ITEMS, projectId)).Append(AdditionalUrlParams);
			urlBuilder.Append(GetLocaleUrlParam(locale));
			urlBuilder.Append(GetCurrencyUrlParam(currency));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), null, onSuccess, onError, Error.ItemsListErrors);
		}

		public void GetListOfItemsByGroup(string projectId, string groupExternalId, [NotNull] Action<StoreItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, [CanBeNull] string currency = null)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CATALOG_GET_ITEMS_IN_GROUP, projectId, groupExternalId)).Append(AdditionalUrlParams);
			urlBuilder.Append(GetLocaleUrlParam(locale));
			urlBuilder.Append(GetCurrencyUrlParam(currency));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), null, onSuccess, onError, Error.ItemsListErrors);
		}

		public void GetListOfItemGroups(string projectId, [NotNull] Action<Groups> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CATALOG_GET_GROUPS, projectId)).Append(AdditionalUrlParams);
			urlBuilder.Append(GetLocaleUrlParam(locale));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), null, onSuccess, onError);
		}
	}
}