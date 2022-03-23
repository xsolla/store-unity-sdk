using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xsolla.Core;
using Xsolla.Core.Browser;

namespace Xsolla.Orders
{
	public partial class XsollaOrders : MonoSingleton<XsollaOrders>
	{
		private const string URL_ORDER_GET_STATUS = Constants.BASE_STORE_API_URL + "/order/{1}";
		private const string URL_PAYSTATION_UI = "https://secure.xsolla.com/paystation3/?access_token=";
		private const string URL_PAYSTATION_UI_IN_SANDBOX_MODE = "https://sandbox-secure.xsolla.com/paystation3/?access_token=";

		private const string URL_CREATE_PAYMENT_TOKEN = Constants.BASE_STORE_API_URL + "/payment";


		/// <summary>
		/// Opens Pay Station in the browser with a retrieved Pay Station token.
		/// </summary>
		/// <see cref="https://developers.xsolla.com/doc/pay-station"/>
		/// <param name="purchaseData">Contains Pay Station token for the purchase.</param>
		/// <param name="forcePlatformBrowser">Flag indicating whether to force platform browser usage ignoring plug-in settings.</param>
		/// <param name="onRestrictedPaymentMethod">Restricted payment method was triggered in an in-app browser.</param>
		/// <seealso cref="BrowserHelper"/>
		public void OpenPurchaseUi(PurchaseData purchaseData, bool forcePlatformBrowser = false, Action<int> onRestrictedPaymentMethod = null)
		{
			string url = XsollaSettings.IsSandbox ? URL_PAYSTATION_UI_IN_SANDBOX_MODE : URL_PAYSTATION_UI;
			BrowserHelper.Instance.OpenPurchase(
				url,
				purchaseData.token,
				forcePlatformBrowser,
				onRestrictedPaymentMethod);
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
			var url = string.Format(URL_ORDER_GET_STATUS, projectId, orderId);
			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess, onError, ErrorCheckType.OrderStatusErrors);
		}

		/// <summary>
		/// Creates a new payment token.
		/// </summary>
		/// <remarks> Swagger method name:<c>Create payment token</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/commerce-api/cart-payment/payment/create-payment"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="amount">The total amount to be paid by the user.</param>
		/// <param name="currency">Default purchase currency. Three-letter code per ISO 4217.</param>
		/// <param name="description">Purchase description. Used to describe the purchase if there are no specific items.</param>
		/// <param name="locale">:Interface language. Two-letter lowercase language code.</param>
		/// <param name="externalID">Transaction's external ID.</param>
		/// <param name="paymentMethod">Payment method ID.</param>
		/// <param name="customParameters">Your custom parameters represented as a valid JSON set of key-value pairs.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void CreatePaymentToken(
			string projectId,
			float amount,
			string currency,
			string description,
			string locale = null,
			string externalID = null,
			int? paymentMethod = null,
			object customParameters = null,
			Action<TokenEntity> onSuccess = null,
			[CanBeNull] Action<Error> onError = null)
		{
			var url = string.Format(URL_CREATE_PAYMENT_TOKEN, projectId);

			var checkout = new CreatePaymentTokenRequest.Purchase.Checkout(amount, currency);
			var purchaseDescription = new CreatePaymentTokenRequest.Purchase.Description(description);
			var purchase = new CreatePaymentTokenRequest.Purchase(checkout, purchaseDescription);
			var settings = GeneratePaymentTokenSettings(currency, locale, externalID, paymentMethod);
			var requestBody = new CreatePaymentTokenRequest(purchase, settings, customParameters);

			WebRequestHelper.Instance.PostRequest<TokenEntity, CreatePaymentTokenRequest>(SdkType.Store, url, requestBody, PurchaseParamsGenerator.GetPaymentHeaders(Token.Instance), onSuccess, onError, ErrorCheckType.BuyItemErrors);
		}

		/// <summary>
		/// Creates a new payment token.
		/// </summary>
		/// <remarks> Swagger method name:<c>Create payment token</c>.</remarks>
		/// <see cref="https://developers.xsolla.com/commerce-api/cart-payment/payment/create-payment"/>
		/// <param name="projectId">Project ID from your Publisher Account.</param>
		/// <param name="amount">The total amount to be paid by the user.</param>
		/// <param name="currency">Default purchase currency. Three-letter code per ISO 4217.</param>
		/// <param name="items">Used to describe a purchase if it includes a list of specific items.</param>
		/// <param name="locale">:Interface language. Two-letter lowercase language code.</param>
		/// <param name="externalID">Transaction's external ID.</param>
		/// <param name="paymentMethod">Payment method ID.</param>
		/// <param name="customParameters">Your custom parameters represented as a valid JSON set of key-value pairs.</param>
		/// <param name="onSuccess">Successful operation callback.</param>
		/// <param name="onError">Failed operation callback.</param>
		public void CreatePaymentToken(
			string projectId,
			float amount,
			string currency,
			PaymentTokenItem[] items,
			string locale = null,
			string externalID = null,
			int? paymentMethod = null,
			object customParameters = null,
			Action<TokenEntity> onSuccess = null,
			[CanBeNull] Action<Error> onError = null)
		{
			var url = string.Format(URL_CREATE_PAYMENT_TOKEN, projectId);

			var checkout = new CreatePaymentTokenRequest.Purchase.Checkout(amount, currency);

			var purchaseItems = new List<CreatePaymentTokenRequest.Purchase.Item>(items.Length);
			foreach (var item in items)
			{
				var price = new CreatePaymentTokenRequest.Purchase.Item.Price(item.amount, item.amountBeforeDiscount);
				var purchaseItem = new CreatePaymentTokenRequest.Purchase.Item(item.name, price, item.imageUrl, item.description, item.quantity, item.isBouns);
				purchaseItems.Add(purchaseItem);
			}

			var purchase = new CreatePaymentTokenRequest.Purchase(checkout, purchaseItems.ToArray());
			var settings = GeneratePaymentTokenSettings(currency, locale, externalID, paymentMethod);
			var requestBody = new CreatePaymentTokenRequest(purchase, settings, customParameters);

			WebRequestHelper.Instance.PostRequest<TokenEntity, CreatePaymentTokenRequest>(SdkType.Store, url, requestBody, PurchaseParamsGenerator.GetPaymentHeaders(Token.Instance), onSuccess, onError, ErrorCheckType.BuyItemErrors);
		}

		private CreatePaymentTokenRequest.Settings GeneratePaymentTokenSettings(string currency, string locale, string externalID, int? paymentMethod)
		{
			var baseSettings = new TempPurchaseParams.Settings();
			baseSettings.ui = PayStationUISettings.GenerateSettings();
			baseSettings.redirect_policy = RedirectPolicySettings.GeneratePolicy();
			baseSettings.return_url = baseSettings.redirect_policy?.return_url;

			var settings = new CreatePaymentTokenRequest.Settings();
			settings.return_url = baseSettings.return_url;
			settings.ui = baseSettings.ui;
			settings.redirect_policy = baseSettings.redirect_policy;

			settings.currency = currency;
			settings.locale = locale;
			settings.sandbox = XsollaSettings.IsSandbox;
			settings.external_id = externalID;
			settings.payment_method = paymentMethod;

			return settings;
		}
	}
}
