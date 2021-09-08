using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		private const string URL_INVENTORY_GET_ITEMS = BASE_STORE_API_URL + "/user/inventory/items?limit={1}&offset={2}";
		private const string URL_INVENTORY_ITEM_CONSUME = BASE_STORE_API_URL + "/user/inventory/item/consume";
		private const string URL_INVENTORY_REDEEM_COUPON = BASE_STORE_API_URL + "/coupon/redeem";
		private const string URL_INVENTORY_GET_COUPON_REWARDS = BASE_STORE_API_URL + "/coupon/code/{1}/rewards";

		/// <summary>
		/// Retrieves the user’s inventory.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get the current user's inventory</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/inventory-client/get-user-inventory"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="locale">Defines localization of item's text fields.</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		public void GetInventoryItems(string projectId, [NotNull] Action<InventoryItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, int limit = 50, int offset = 0)
		{
			var url = string.Format(URL_INVENTORY_GET_ITEMS, projectId, limit, offset);
			var localeParam = GetLocaleUrlParam(locale);
			var platformParam = GetPlatformUrlParam();
			url = ConcatUrlAndParams(url, localeParam, platformParam);

			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError, Error.ItemsListErrors);
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
		public void ConsumeInventoryItem(string projectId, ConsumeItem item, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_INVENTORY_ITEM_CONSUME, projectId);
			var platformParam = GetPlatformUrlParam();
			url = ConcatUrlAndParams(url, platformParam);

			var headers = new List<WebRequestHeader>() { WebRequestHeader.AuthHeader(Token.Instance), WebRequestHeader.ContentTypeHeader() };

			WebRequestHelper.Instance.PostRequest(SdkType.Store, url, item, headers, onSuccess, onError, Error.ConsumeItemErrors);
		}

		/// <summary>
		/// Redeems a coupon code.
		/// </summary>
		/// <remarks> Swagger method name:<c>Redeem coupon code</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/promotions/coupons/redeem-coupon/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="couponCode">Unique coupon code. Contains letters and numbers.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void RedeemCouponCode(string projectId, CouponCode couponCode, [NotNull] Action<CouponRedeemedItems> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_INVENTORY_REDEEM_COUPON, projectId);
			var platformParam = GetPlatformUrlParam();
			url = ConcatUrlAndParams(url, platformParam);

			var headers = new List<WebRequestHeader>() { WebRequestHeader.AuthHeader(Token.Instance), WebRequestHeader.ContentTypeHeader() };
			WebRequestHelper.Instance.PostRequest(SdkType.Store, url, couponCode, headers, onSuccess, onError, Error.CouponErrors);
		}

		/// <summary>
		/// Gets coupons rewards by its code. Can be used to allow users to choose one of many items as a bonus.
		/// The usual case is choosing a DRM if the coupon contains a game as a bonus (type=unit).
		/// </summary>
		/// <remarks> Swagger method name:<c>Get coupon rewards</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/promotions/coupons/get-coupon-rewards-by-code/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="couponCode">Unique case sensitive code. Contains letters and numbers.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void GetCouponRewards(string projectId, string couponCode, [NotNull] Action<CouponReward> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null)
		{
			var url = string.Format(URL_INVENTORY_GET_COUPON_REWARDS, projectId, couponCode);
			var localeParam = GetLocaleUrlParam(locale);
			var platformParam = GetPlatformUrlParam();
			url = ConcatUrlAndParams(url, localeParam, platformParam);
			
			var headers = new List<WebRequestHeader>() { WebRequestHeader.AuthHeader(Token.Instance), WebRequestHeader.ContentTypeHeader() };
			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, headers, onSuccess, onError, Error.CouponErrors);
		}
	}
}
