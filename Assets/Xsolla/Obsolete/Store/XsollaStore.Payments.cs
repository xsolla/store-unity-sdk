using System;
using JetBrains.Annotations;
using Xsolla.Cart;
using Xsolla.Catalog;
using Xsolla.Core;
using Xsolla.Orders;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		[Obsolete("Use XsollaCatalog instead")]
		public void ItemPurchase(string projectId, string itemSku, [CanBeNull] Action<PurchaseData> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null)
			=> XsollaCatalog.Instance.ItemPurchase(projectId, itemSku, onSuccess, onError, purchaseParams);

		[Obsolete("Use XsollaCatalog instead")]
		public void ItemPurchaseForVirtualCurrency(
			string projectId,
			string itemSku,
			string priceSku,
			[CanBeNull] Action<PurchaseData> onSuccess,
			[CanBeNull] Action<Error> onError,
			PurchaseParams purchaseParams = null)
			=> XsollaCatalog.Instance.ItemPurchaseForVirtualCurrency(projectId, itemSku, priceSku, onSuccess, onError, purchaseParams);

		[Obsolete("Use XsollaCart instead")]
		public void CartPurchase(string projectId, [CanBeNull] Action<PurchaseData> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null)
			=> XsollaCart.Instance.CartPurchase(projectId, onSuccess, onError, purchaseParams);

		[Obsolete("Use XsollaCart instead")]
		public void CartPurchase(string projectId, string cartId, [CanBeNull] Action<PurchaseData> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null)
			=> XsollaCart.Instance.CartPurchase(projectId, cartId, onSuccess, onError, purchaseParams);

		[Obsolete("Use XsollaOrders instead")]
		public void OpenPurchaseUi(PurchaseData purchaseData, bool forcePlatformBrowser = false, Action<int> onRestrictedPaymentMethod = null)
			=> XsollaOrders.Instance.OpenPurchaseUi(purchaseData, forcePlatformBrowser, onRestrictedPaymentMethod);

		[Obsolete("Use XsollaOrders instead")]
		public void CheckOrderStatus(string projectId, int orderId, [NotNull] Action<OrderStatus> onSuccess, [CanBeNull] Action<Error> onError)
			=> XsollaOrders.Instance.CheckOrderStatus(projectId, orderId, onSuccess, onError);

		[Obsolete("Use XsollaOrders instead")]
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
			=> XsollaOrders.Instance.CreatePaymentToken(projectId, amount, currency, description, locale, externalID, paymentMethod, customParameters, onSuccess, onError);

		[Obsolete("Use XsollaOrders instead")]
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
			=> XsollaOrders.Instance.CreatePaymentToken(projectId, amount, currency, items, locale, externalID, paymentMethod, customParameters, onSuccess, onError);
	}
}
