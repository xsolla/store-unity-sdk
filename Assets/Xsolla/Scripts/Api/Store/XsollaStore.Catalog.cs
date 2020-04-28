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
		private const string URL_CATALOG_GET_ITEMS_IN_GROUP = "https://store.xsolla.com/api/v2/project/{0}/items/virtual_items/group/{1}";
		private const string URL_CATALOG_GET_GROUPS = "https://store.xsolla.com/api/v2/project/{0}/items/groups";

		/// <summary>
		/// Returns all items in catalog.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get virtual items list</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/items/get-virtual-items"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Defines localization of item's text fields.</param>
		/// <param name="currency">Defines currency of item's price.</param>
		public void GetCatalog(string projectId, [NotNull] Action<StoreItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, [CanBeNull] string currency = null)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CATALOG_GET_ITEMS, projectId)).Append(AdditionalUrlParams);
			urlBuilder.Append(GetLocaleUrlParam(locale));
			urlBuilder.Append(GetCurrencyUrlParam(currency));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), null, onSuccess, onError, Error.ItemsListErrors);
		}

		/// <summary>
		/// Returns items in a specific group.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get items list by specified group</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/items/get-virtual-items-group"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="groupExternalId">Group external ID.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Defines localization of item's text fields.</param>
		/// <param name="currency">Defines currency of item's price.</param>
		public void GetGroupItems(string projectId, string groupExternalId, [NotNull] Action<StoreItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, [CanBeNull] string currency = null)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CATALOG_GET_ITEMS_IN_GROUP, projectId, groupExternalId)).Append(AdditionalUrlParams);
			urlBuilder.Append(GetLocaleUrlParam(locale));
			urlBuilder.Append(GetCurrencyUrlParam(currency));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), null, onSuccess, onError, Error.ItemsListErrors);
		}

		/// <summary>
		/// Returns groups list.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get items groups list</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/groups/get-item-groups"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Defines localization of item's text fields.</param>
		public void GetItemGroups(string projectId, [NotNull] Action<Groups> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CATALOG_GET_GROUPS, projectId)).Append(AdditionalUrlParams);
			urlBuilder.Append(GetLocaleUrlParam(locale));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), null, onSuccess, onError);
		}
	}
}