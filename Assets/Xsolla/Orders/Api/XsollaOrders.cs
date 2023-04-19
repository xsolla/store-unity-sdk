using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xsolla.Core;

namespace Xsolla.Orders
{
	public class XsollaOrders : MonoSingleton<XsollaOrders>
	{
		private const string URL_ORDER_GET_STATUS = Constants.BASE_STORE_API_URL + "/order/{1}";
		private const string URL_PAYSTATION_UI = "https://secure.xsolla.com/paystation3/?access_token=";
		private const string URL_PAYSTATION_UI_IN_SANDBOX_MODE = "https://sandbox-secure.xsolla.com/paystation3/?access_token=";
		private const string URL_CREATE_PAYMENT_TOKEN = Constants.BASE_STORE_API_URL + "/payment";

		/// <summary>
		/// Opens Pay Station in the browser with a retrieved Pay Station token.
		/// </summary>
		/// More about the use cases:
		/// - [Cart purchase](https://developers.xsolla.com/sdk/unity/item-purchase/cart-purchase/)
		/// - [Purchase in one click](https://developers.xsolla.com/sdk/unity/item-purchase/one-click-purchase/)
		/// - [Ordering free items](https://developers.xsolla.com/sdk/unity/promo/free-items/#sdk_free_items_order_item_via_cart)
		///
		/// <param name="purchaseData">Contains Pay Station token for the purchase.</param>
		/// <param name="forcePlatformBrowser">Whether to force platform browser usage ignoring plug-in settings.</param>
		/// <param name="onRestrictedPaymentMethod">Restricted payment method was triggered in an built-in browser.</param>
		/// <param name="onBrowserClosed">Called after the browser was closed.</param>
		/// <seealso cref="Xsolla.Core.BrowserHelper"/>
		public void OpenPurchaseUi(PurchaseData purchaseData, bool forcePlatformBrowser = false, Action<int> onRestrictedPaymentMethod = null, Action<bool> onBrowserClosed = null)
		{
			string url = XsollaSettings.IsSandbox ? URL_PAYSTATION_UI_IN_SANDBOX_MODE : URL_PAYSTATION_UI;
			BrowserHelper.Instance.OpenPurchase(
				url,
				purchaseData.token,
				forcePlatformBrowser,
				onRestrictedPaymentMethod,
				onBrowserClosed);
		}

		/// <summary>
		/// Returns status of the specified order.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/track-order/).</remarks>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. </param>
		/// <param name="orderId">Unique order identifier.</param>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso cref="Xsolla.Catalog.XsollaCatalog.PurchaseItemForVirtualCurrency"/>
		public void CheckOrderStatus(string projectId, int orderId, [NotNull] Action<OrderStatus> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var url = string.Format(URL_ORDER_GET_STATUS, projectId, orderId);
			WebRequestHelper.Instance.GetRequest(SdkType.Store, url, WebRequestHeader.AuthHeader(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => CheckOrderStatus(projectId, orderId, onSuccess, onError)),
				ErrorCheckType.OrderStatusErrors);
		}

		/// <summary>
		/// Creates a new payment token.
		/// </summary>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. </param>
		/// <param name="amount">The total amount to be paid by the user.</param>
		/// <param name="currency">Default purchase currency. Three-letter code per [ISO 4217](https://en.wikipedia.org/wiki/ISO_4217) (USD by default).</param>
		/// <param name="description">Purchase description. Used to describe the purchase if there are no specific items.</param>
		/// <param name="locale">Interface language. <br/>
		/// The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).</param>
		/// <param name="externalID">Transaction's external ID.</param>
		/// <param name="paymentMethod">Payment method ID.</param>
		/// <param name="customParameters">Your custom parameters represented as a valid JSON set of key-value pairs.</param>
		/// <param name="onSuccess">Called after the successful item purchase.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
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

			WebRequestHelper.Instance.PostRequest<TokenEntity, CreatePaymentTokenRequest>(SdkType.Store, url, requestBody, PurchaseParamsGenerator.GetPaymentHeaders(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => CreatePaymentToken(projectId, amount, currency, description, locale, externalID, paymentMethod, customParameters, onSuccess, onError)),
				ErrorCheckType.BuyItemErrors);
		}

		/// <summary>
		/// Creates a new payment token.
		/// </summary>
		/// <param name="projectId">Project ID, can be found in Publisher Account next to the name of the project. </param>
		/// <param name="amount">The total amount to be paid by the user.</param>
		/// <param name="currency">Default purchase currency. Three-letter code per [ISO 4217](https://en.wikipedia.org/wiki/ISO_4217) (USD by deafault).</param>
		/// <param name="items">Used to describe a purchase if it includes a list of specific items.</param>
		/// <param name="locale">Interface language. <br/>
		/// The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).</param>
		/// <param name="externalID">Transaction's external ID.</param>
		/// <param name="paymentMethod">Payment method ID.</param>
		/// <param name="customParameters">Your custom parameters represented as a valid JSON set of key-value pairs.</param>
		/// <param name="onSuccess">Called after the successful item purchase.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
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

			WebRequestHelper.Instance.PostRequest<TokenEntity, CreatePaymentTokenRequest>(SdkType.Store, url, requestBody, PurchaseParamsGenerator.GetPaymentHeaders(Token.Instance), onSuccess,
				onError: error => TokenRefresh.Instance.CheckInvalidToken(error, onError, () => CreatePaymentToken(projectId, amount, currency, items, locale, externalID, paymentMethod, customParameters, onSuccess, onError)),
				ErrorCheckType.BuyItemErrors);
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
