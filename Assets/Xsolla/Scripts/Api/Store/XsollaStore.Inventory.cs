using System;
using System.Text;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		private const string URL_INVENTORY_GET_ITEMS = BASE_STORE_API_URL + "/user/inventory/items";
		private const string URL_INVENTORY_ITEM_CONSUME = BASE_STORE_API_URL + "/user/inventory/item/consume";

		/// <summary>
		/// Retrieves the user’s inventory.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get the current user's inventory</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/inventory-client/get-user-inventory"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Defines localization of item's text fields.</param>
		public void GetInventoryItems(string projectId, [NotNull] Action<InventoryItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_INVENTORY_GET_ITEMS, projectId)).Append(AnalyticUrlAddition);
			urlBuilder.Append(GetLocaleUrlParam(locale));
			urlBuilder.Append(GetPlatformUrlParam());

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), AuthAndAnalyticHeaders, onSuccess, onError, Error.ItemsListErrors);
		}

		/// <summary>
		/// Consume item from inventory.
		/// Please note: API support consume only one item now (quantity = 1).
		/// </summary>
		/// <remarks> Swagger method name:<c>Consume item</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/inventory-client/consume-item"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="item">Contains consume parameters.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void ConsumeInventoryItem(string projectId, ConsumeItem item, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_INVENTORY_ITEM_CONSUME, projectId)).Append(AnalyticUrlAddition);
			urlBuilder.Append(GetPlatformUrlParam());

			var headers = AppendAnalyticHeadersTo(WebRequestHeader.AuthHeader(Token), WebRequestHeader.ContentTypeHeader());

			WebRequestHelper.Instance.PostRequest(urlBuilder.ToString(), item, headers, onSuccess, onError, Error.ConsumeItemErrors);
		}
	}
}