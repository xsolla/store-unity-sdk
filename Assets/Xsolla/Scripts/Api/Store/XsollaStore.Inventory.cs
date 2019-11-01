using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		private const string URL_INVENTORY_GET_ITEMS = "https://store.xsolla.com/api/v1/project/{0}/user/inventory/items";
		private const string URL_INVENTORY_ITEM_CONSUME = "https://store.xsolla.com/api/v2/project/{0}/user/inventory/item/consume";

		public void GetInventoryItems(string projectId, [NotNull] Action<InventoryItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_INVENTORY_GET_ITEMS, projectId)).Append(AdditionalUrlParams);
			urlBuilder.Append(GetLocaleUrlParam(locale));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.ItemsListErrors);
		}

		public void ConsumeInventoryItem(string projectId, ConsumeItem item, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_INVENTORY_ITEM_CONSUME, projectId)).Append(AdditionalUrlParams);

			var headers = new List<WebRequestHeader>() { WebRequestHeader.AuthHeader(Token), WebRequestHeader.ContentTypeHeader() };
			WebRequestHelper.Instance.PostRequest(urlBuilder.ToString(), item, headers, onSuccess, onError, Error.ConsumeItemErrors);
		}
	}
}