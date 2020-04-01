using System;
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
		private const string URL_BUY_ITEM = "https://store.xsolla.com/api/v2/project/{0}/payment/item/{1}";
		private const string URL_BUY_ITEM_FOR_VC = "https://store.xsolla.com/api/v2/project/{0}/payment/item/{1}/virtual/{2}";
		private const string URL_BUY_CART = "https://store.xsolla.com/api/v2/project/{0}/payment/cart/{1}";

		private const string URL_ORDER_GET_STATUS = "https://store.xsolla.com/api/v2/project/{0}/order/{1}";
		private const string URL_PAYSTATION_UI = "https://secure.xsolla.com/paystation2/?access_token=";
		private const string URL_PAYSTATION_UI_IN_SANDBOX_MODE = "https://sandbox-secure.xsolla.com/paystation2/?access_token=";

		private List<WebRequestHeader> GetPaymentHeaders(Token token)
		{
			List<WebRequestHeader> headers = new List<WebRequestHeader> {
				WebRequestHeader.AuthHeader(token)
			};
			if (token.FromSteam) {
				string userId = token.GetSteamUserID();
				if (!string.IsNullOrEmpty(userId)) {
					headers.Add(WebRequestHeader.SteamPaymentHeader(userId));
				}
			}
			return headers;
		}

		public void BuyItem(string projectId, string itemId, [CanBeNull] Action<PurchaseData> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null)
		{
			TempPurchaseParams tempPurchaseParams = new TempPurchaseParams {
				sandbox = XsollaSettings.IsSandbox,
				settings = new TempPurchaseParams.Settings(XsollaSettings.PaystationTheme)
			};

			var urlBuilder = new StringBuilder(string.Format(URL_BUY_ITEM, projectId, itemId)).Append(AdditionalUrlParams);
			WebRequestHelper.Instance.PostRequest<PurchaseData, TempPurchaseParams>(urlBuilder.ToString(), tempPurchaseParams, GetPaymentHeaders(Token), onSuccess, onError, Error.BuyItemErrors);
		}

		public void BuyItem(string projectId, string itemId, string priceSku, [CanBeNull] Action<PurchaseData> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null)
		{
			TempPurchaseParams tempPurchaseParams = new TempPurchaseParams {
				sandbox = XsollaSettings.IsSandbox,
				settings = new TempPurchaseParams.Settings(XsollaSettings.PaystationTheme)
			};

			var urlBuilder = new StringBuilder(string.Format(URL_BUY_ITEM_FOR_VC, projectId, itemId, priceSku)).Append(AdditionalUrlParams);
			WebRequestHelper.Instance.PostRequest<PurchaseData, TempPurchaseParams>(urlBuilder.ToString(), tempPurchaseParams, GetPaymentHeaders(Token), onSuccess, onError, Error.BuyItemErrors);
		}

		public void BuyCart(string projectId, string cartId, [CanBeNull] Action<PurchaseData> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null)
		{
			TempPurchaseParams tempPurchaseParams = new TempPurchaseParams {
				sandbox = XsollaSettings.IsSandbox,
				settings = new TempPurchaseParams.Settings(XsollaSettings.PaystationTheme)
			};

			var urlBuilder = new StringBuilder(string.Format(URL_BUY_CART, projectId, cartId)).Append(AdditionalUrlParams);
			WebRequestHelper.Instance.PostRequest<PurchaseData, TempPurchaseParams>(urlBuilder.ToString(), tempPurchaseParams, GetPaymentHeaders(Token), onSuccess, onError, Error.BuyCartErrors);
		}

		public void OpenPurchaseUi(PurchaseData purchaseData)
		{
			string url = (XsollaSettings.IsSandbox) ? URL_PAYSTATION_UI_IN_SANDBOX_MODE : URL_PAYSTATION_UI;
			BrowserHelper.Instance.OpenPurchase(
				url, purchaseData.token,
				XsollaSettings.IsSandbox,
				XsollaSettings.InAppBrowserEnabled);
		}

		public void CheckOrderStatus(string projectId, int orderId, [NotNull] Action<OrderStatus> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_ORDER_GET_STATUS, projectId, orderId)).Append(AdditionalUrlParams);
			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.OrderStatusErrors);
		}
	}
}
