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
		/// Returns the current user’s balance of virtual currency. For each virtual currency, complete data is returned.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/player-inventory/display-inventory/).</remarks>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Called after virtual currency balance was successfully received.</param>
		/// <param name="platform">Publishing platform the user plays on.<br/>
		/// Can be `xsolla` (default), `playstation_network`, `xbox_live`, `pc_standalone`, `nintendo_shop`, `google_play`, `app_store_ios`, `android_standalone`, `ios_standalone`, `android_other`, `ios_other`, or `pc_other`.</param>
		public void GetVirtualCurrencyBalance(string projectId, [NotNull] Action<VirtualCurrencyBalances> onSuccess, [CanBeNull] Action<Error> onError, string platform = null)
		{
			var url = string.Format(URL_VIRTUAL_CURRENCY_BALANCE, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, platform: platform);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => GetVirtualCurrencyBalance(projectId, onSuccess, onError, platform)),
				ErrorCheckType.ItemsListErrors);
		}

		/// <summary>
		/// Returns a list of virtual items from the user’s inventory according to pagination settings. For each virtual item, complete data is returned.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/player-inventory/display-inventory/).</remarks>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Called after purchased virtual items were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Defines localization of item's text fields.<br/>
		/// The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).</param>
		/// <param name="platform">Publishing platform the user plays on.<br/>
		/// Can be `xsolla` (default), `playstation_network`, `xbox_live`, `pc_standalone`, `nintendo_shop`, `google_play`, `app_store_ios`, `android_standalone`, `ios_standalone`, `android_other`, `ios_other`, or `pc_other`.</param>
		public void GetInventoryItems(string projectId, [NotNull] Action<InventoryItems> onSuccess, [CanBeNull] Action<Error> onError, int limit = 50, int offset = 0, [CanBeNull] string locale = null, string platform = null)
		{
			var url = string.Format(URL_INVENTORY_GET_ITEMS, projectId);
			url = UrlParameterizer.ConcatUrlAndParams(url, limit: limit, offset: offset, locale: locale, platform: platform);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => GetInventoryItems(projectId, onSuccess, onError, limit, offset, locale, platform)),
				ErrorCheckType.ItemsListErrors);
		}

		/// <summary>
		/// Consumes an inventory item. Use for only for consumable virtual items.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/player-inventory/consume-item/).</remarks>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="item">Contains consume parameters.</param>
		/// <param name="onSuccess">Called after successful inventory item consumption.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="platform">Publishing platform the user plays on.<br/>
		/// Can be `xsolla` (default), `playstation_network`, `xbox_live`, `pc_standalone`, `nintendo_shop`, `google_play`, `app_store_ios`, `android_standalone`, `ios_standalone`, `android_other`, `ios_other`, or `pc_other`.</param>
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
		/// Returns a list of time-limited items from the user’s inventory. For each item, complete data is returned.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/player-inventory/display-inventory/).</remarks>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Called after list of user time limited items was successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="platform">Publishing platform the user plays on.<br/>
		/// Can be `xsolla` (default), `playstation_network`, `xbox_live`, `pc_standalone`, `nintendo_shop`, `google_play`, `app_store_ios`, `android_standalone`, `ios_standalone`, `android_other`, `ios_other`, or `pc_other`.</param>
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
