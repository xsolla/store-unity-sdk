using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Inventory
{
	public class XsollaInventory : MonoSingleton<XsollaInventory>
	{
		private const string URL_VIRTUAL_CURRENCY_BALANCE = Constants.BASE_STORE_API_URL + "/user/virtual_currency_balance";
		private const string URL_INVENTORY_GET_ITEMS = Constants.BASE_STORE_API_URL + "/user/inventory/items";
		private const string URL_INVENTORY_ITEM_CONSUME = Constants.BASE_STORE_API_URL + "/user/inventory/item/consume";
		private const string URL_GET_TIME_LIMITED_ITEMS = Constants.BASE_STORE_API_URL + "/user/time_limited_items";

		/// <summary>
		/// Returns balance for all virtual currencies.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get the current user's virtual balance</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/inventory-client/get-user-virtual-balance"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="platform">Publishing platform the user plays on.</param>
		public void GetVirtualCurrencyBalance(string projectId, [NotNull] Action<VirtualCurrencyBalances> onSuccess, [CanBeNull] Action<Error> onError, string platform = null)
		{
			var url = string.Format(URL_VIRTUAL_CURRENCY_BALANCE, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, platform: platform);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => GetVirtualCurrencyBalance(projectId, onSuccess, onError, platform)),
				ErrorCheckType.ItemsListErrors);
		}

		/// <summary>
		/// Retrieves the user’s inventory.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get the current user's inventory</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/inventory-client/get-user-inventory"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Defines localization of item's text fields.</param>
		/// <param name="platform">Publishing platform the user plays on.</param>
		public void GetInventoryItems(string projectId, [NotNull] Action<InventoryItems> onSuccess, [CanBeNull] Action<Error> onError, int limit = 50, int offset = 0, [CanBeNull] string locale = null, string platform = null)
		{
			var url = string.Format(URL_INVENTORY_GET_ITEMS, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, limit: limit, offset: offset, locale: locale, platform: platform);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => GetInventoryItems(projectId, onSuccess, onError, limit, offset, locale, platform)),
				ErrorCheckType.ItemsListErrors);
		}

		/// <summary>
		/// Consumes item from inventory.
		/// </summary>
		/// <remarks> Swagger method name:<c>Consume item</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/inventory-client/consume-item"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="item">Contains consume parameters.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="platform">Publishing platform the user plays on.</param>
		public void ConsumeInventoryItem(string projectId, ConsumeItem item, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError, string platform = null)
		{
			var url = string.Format(URL_INVENTORY_ITEM_CONSUME, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, platform: platform);

			var headers = new List<WebRequestHeader>() { WebRequestHeader.AuthHeader(Token.Instance), WebRequestHeader.ContentTypeHeader() };

			WebRequestHelper.Instance.PostRequest(SdkType.Store, url, item, headers, onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => ConsumeInventoryItem(projectId, item, onSuccess, onError, platform)),
				ErrorCheckType.ConsumeItemErrors);
		}

		/// <summary>
		/// Retrieves the current user’s time limited items.
		/// </summary>
		/// <remarks> Swagger method name:<c>Retrieves the current user’s time limited items.</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/inventory-client/get-user-subscriptions"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="platform">Publishing platform the user plays on.</param>
		public void GetTimeLimitedItems(string projectId, [NotNull] Action<TimeLimitedItems> onSuccess, [CanBeNull] Action<Error> onError, string platform = null)
		{
			var url = string.Format(URL_GET_TIME_LIMITED_ITEMS, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, platform: platform);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => GetTimeLimitedItems(projectId, onSuccess, onError, platform)),
				ErrorCheckType.ItemsListErrors);
		}
	}
}
