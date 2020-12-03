using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		private const string URL_BUY_ITEM = BASE_STORE_API_URL + "/payment/item/{1}";
		private const string URL_BUY_ITEM_FOR_VC = BASE_STORE_API_URL + "/payment/item/{1}/virtual/{2}";
		private const string URL_BUY_CURRENT_CART = BASE_STORE_API_URL + "/payment/cart";
		private const string URL_BUY_SPECIFIC_CART = BASE_STORE_API_URL + "/payment/cart/{1}";

		private const string URL_ORDER_GET_STATUS = BASE_STORE_API_URL + "/order/{1}";
		private const string URL_PAYSTATION_UI = "https://secure.xsolla.com/paystation2/?access_token=";
		private const string URL_PAYSTATION_UI_IN_SANDBOX_MODE = "https://sandbox-secure.xsolla.com/paystation2/?access_token=";

		/// <summary>
		/// Returns headers list such as <c>AuthHeader</c> and <c>SteamPaymentHeader</c>.
		/// </summary>
		/// <param name="token">Auth token taken from Xsolla Login.</param>
		/// <returns></returns>
		private List<WebRequestHeader> GetPaymentHeaders(Token token)
		{
			List<WebRequestHeader> headers = null;

			if (token.FromSteam)
			{
				string userId = token.GetSteamUserID();
				if (!string.IsNullOrEmpty(userId))
					headers = AppendAnalyticHeadersTo(WebRequestHeader.AuthHeader(token), WebRequestHeader.SteamPaymentHeader(userId));
			}

			if (headers == null)
				headers = AppendAnalyticHeadersTo(WebRequestHeader.AuthHeader(token));

			return headers;
		}

		/// <summary>
		/// Returns unique identifier of the created order and
		/// the Pay Station token for the purchase of the specified product.
		/// </summary>
		/// <remarks> Swagger method name:<c>Create order with specified item</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/payment/create-order-with-item"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="itemSku">Item SKU to purchase.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c>, and <c>currency</c>.</param>
		/// <seealso cref="OpenPurchaseUi"/>
		public void ItemPurchase(string projectId, string itemSku, [CanBeNull] Action<PurchaseData> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null)
		{
			TempPurchaseParams tempPurchaseParams = new TempPurchaseParams
			{
				sandbox = XsollaSettings.IsSandbox,
				settings = new TempPurchaseParams.Settings(XsollaSettings.PaystationTheme)
			};

			var urlBuilder = new StringBuilder(string.Format(URL_BUY_ITEM, projectId, itemSku)).Append(AnalyticUrlAddition);
			WebRequestHelper.Instance.PostRequest<PurchaseData, TempPurchaseParams>(urlBuilder.ToString(), tempPurchaseParams, GetPaymentHeaders(Token), onSuccess, onError, Error.BuyItemErrors);
		}

		/// <summary>
		/// Returns unique identifier of the created order and
		/// the Pay Station token for the purchase of the specified product by virtual currency.
		/// </summary>
		/// <remarks> Swagger method name:<c>Create order with specified item purchased by virtual currency</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/payment/create-order-with-item-for-virtual-currency"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="itemSku">Item SKU to purchase.</param>
		/// <param name="priceSku">Virtual currency SKU.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c> and <c>currency</c>.</param>
		/// <seealso cref="OpenPurchaseUi"/>
		public void ItemPurchaseForVirtualCurrency(
			string projectId,
			string itemSku,
			string priceSku,
			[CanBeNull] Action<PurchaseData> onSuccess,
			[CanBeNull] Action<Error> onError,
			PurchaseParams purchaseParams = null)
		{
			TempPurchaseParams tempPurchaseParams = new TempPurchaseParams
			{
				sandbox = XsollaSettings.IsSandbox,
				settings = new TempPurchaseParams.Settings(XsollaSettings.PaystationTheme)
			};

			var urlBuilder = new StringBuilder(string.Format(URL_BUY_ITEM_FOR_VC, projectId, itemSku, priceSku)).Append(AnalyticUrlAddition);
			urlBuilder.Append(GetPlatformUrlParam());

			WebRequestHelper.Instance.PostRequest<PurchaseData, TempPurchaseParams>(urlBuilder.ToString(), tempPurchaseParams, GetPaymentHeaders(Token), onSuccess, onError, Error.BuyItemErrors);
		}
		
		/// <summary>
		/// Returns the Paystation Token for the purchase of the items in the current cart.
		/// </summary>
		/// <remarks> Swagger method name:<c>Creates an order with all items from the current cart</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/payment/create-order/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c> and <c>currency</c>.</param>
		public void CartPurchase(string projectId, [CanBeNull] Action<PurchaseData> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null)
		{
			TempPurchaseParams tempPurchaseParams = new TempPurchaseParams
			{
				sandbox = XsollaSettings.IsSandbox,
				settings = new TempPurchaseParams.Settings(XsollaSettings.PaystationTheme)
			};

			var urlBuilder = new StringBuilder(string.Format(URL_BUY_CURRENT_CART, projectId)).Append(AnalyticUrlAddition);
			WebRequestHelper.Instance.PostRequest<PurchaseData, TempPurchaseParams>(urlBuilder.ToString(), tempPurchaseParams, GetPaymentHeaders(Token), onSuccess, onError, Error.BuyCartErrors);
		}

		/// <summary>
		/// Returns the Pay Station token for the purchase of the items in the specified cart.
		/// </summary>
		/// <remarks> Swagger method name:<c>Creates an order with all items from the particular cart</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/cart-payment/payment/create-order-by-cart-id/"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c> and <c>currency</c>.</param>
		public void CartPurchase(string projectId, string cartId, [CanBeNull] Action<PurchaseData> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null)
		{
			TempPurchaseParams tempPurchaseParams = new TempPurchaseParams
			{
				sandbox = XsollaSettings.IsSandbox,
				settings = new TempPurchaseParams.Settings(XsollaSettings.PaystationTheme)
			};

			var urlBuilder = new StringBuilder(string.Format(URL_BUY_SPECIFIC_CART, projectId, cartId)).Append(AnalyticUrlAddition);
			WebRequestHelper.Instance.PostRequest<PurchaseData, TempPurchaseParams>(urlBuilder.ToString(), tempPurchaseParams, GetPaymentHeaders(Token), onSuccess, onError, Error.BuyCartErrors);
		}

		/// <summary>
		/// Opens Pay Station in the browser with retrieved Pay Station token.
		/// </summary>
		/// <see cref="https://developers.xsolla.com/doc/pay-station"/>
		/// <param name="purchaseData">Contains Pay Station token for the purchase.</param>
		/// <seealso cref="BrowserHelper"/>
		public void OpenPurchaseUi(PurchaseData purchaseData)
		{
			string url = (XsollaSettings.IsSandbox) ? URL_PAYSTATION_UI_IN_SANDBOX_MODE : URL_PAYSTATION_UI;
			BrowserHelper.Instance.OpenPurchase(
				url, purchaseData.token,
				XsollaSettings.IsSandbox,
				XsollaSettings.InAppBrowserEnabled);
		}

		/// <summary>
		/// Returns status of the specified order.
		/// </summary>
		/// <remarks> Swagger method name:<c>Get order</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/store-api/order/get-order"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="orderId">Unique order identifier.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		/// <seealso cref="ItemPurchaseForVirtualCurrency"/>
		/// <seealso cref="CartPurchase"/>
		public void CheckOrderStatus(string projectId, int orderId, [NotNull] Action<OrderStatus> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_ORDER_GET_STATUS, projectId, orderId)).Append(AnalyticUrlAddition);
			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), AuthAndAnalyticHeaders, onSuccess, onError, Error.OrderStatusErrors);
		}

		/// <summary>
		/// Polls Store every 3 seconds to know when the payment is finished.
		/// </summary>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="orderId">Unique identifier of the created order.</param>
		/// <param name="onSuccess">Successful payment callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void ProcessOrder(string projectId, int orderId, Action onSuccess = null, Action<Error> onError = null)
		{
			StartCoroutine(CheckOrderStatus(projectId, orderId, onSuccess, onError));
		}

		IEnumerator CheckOrderStatus(string projectId, int orderId, Action onSuccess = null, Action<Error> onError = null)
		{
			// Wait for 3 seconds.
			yield return new WaitForSeconds(3.0f);
		
			CheckOrderStatus(projectId, orderId, status =>
			{
				if ((status.Status != OrderStatusType.Paid) && (status.Status != OrderStatusType.Done))
				{
					StartCoroutine(CheckOrderStatus(projectId, orderId, onSuccess, onError));
				}
				else
				{
					Debug.Log($"Order `{orderId}` was successfully processed!");
					onSuccess?.Invoke();
				}
			}, onError);
		}
	}
}
