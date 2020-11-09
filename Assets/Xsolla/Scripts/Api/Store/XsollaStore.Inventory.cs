using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		private const string URL_INVENTORY_GET_ITEMS = BASE_STORE_API_URL + "/user/inventory/items";
		private const string URL_INVENTORY_ITEM_CONSUME = BASE_STORE_API_URL + "/user/inventory/item/consume";
		private const string URL_INVENTORY_REDEEM_COUPON = BASE_STORE_API_URL + "/coupon/redeem";
		private const string URL_INVENTORY_GET_COUPON_REWARDS = BASE_STORE_API_URL + "/coupon/code/{1}/rewards";

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
			var urlBuilder = new StringBuilder(string.Format(URL_INVENTORY_GET_ITEMS, projectId)).Append(AdditionalUrlParams);
			urlBuilder.Append(GetLocaleUrlParam(locale));
			urlBuilder.Append(GetPlatformUrlParam());

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.ItemsListErrors);
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
			var urlBuilder = new StringBuilder(string.Format(URL_INVENTORY_ITEM_CONSUME, projectId)).Append(AdditionalUrlParams);
			urlBuilder.Append(GetPlatformUrlParam());

			var headers = new List<WebRequestHeader>() { WebRequestHeader.AuthHeader(Token), WebRequestHeader.ContentTypeHeader() };
			WebRequestHelper.Instance.PostRequest(urlBuilder.ToString(), item, headers, onSuccess, onError, Error.ConsumeItemErrors);
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
		public void RedeemCouponCode(string projectId, CouponCode couponCode, [CanBeNull] Action<CouponRedeemedItems> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_INVENTORY_REDEEM_COUPON, projectId)).Append(AdditionalUrlParams);
			urlBuilder.Append(GetPlatformUrlParam());

			var headers = new List<WebRequestHeader>() { WebRequestHeader.AuthHeader(Token), WebRequestHeader.ContentTypeHeader() };
			WebRequestHelper.Instance.PostRequest(urlBuilder.ToString(), couponCode, headers, onSuccess, onError, Error.CouponErrors);
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
			var urlBuilder = new StringBuilder(string.Format(URL_INVENTORY_GET_COUPON_REWARDS, projectId, couponCode)).Append(AdditionalUrlParams);
			urlBuilder.Append(GetLocaleUrlParam(locale));
			urlBuilder.Append(GetPlatformUrlParam());

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.CouponErrors);
		}
	}
}