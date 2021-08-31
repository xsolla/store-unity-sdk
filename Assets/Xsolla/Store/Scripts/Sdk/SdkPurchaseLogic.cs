using System;
using System.Collections.Generic;
using System.Linq;
using Xsolla.Core;
using Xsolla.Store;

namespace Xsolla.Demo
{
	public class SdkPurchaseLogic : MonoSingleton<SdkPurchaseLogic>
	{
		public void PurchaseForRealMoney(CatalogItemModel item, RestrictedPaymentAllower restrictedPaymentAllower = null, Action<CatalogItemModel> onSuccess = null, Action<Error> onError = null)
		{
			XsollaStore.Instance.ItemPurchase(XsollaSettings.StoreProjectId, item.Sku,
			onSuccess: data =>
			{
				XsollaStore.Instance.OpenPurchaseUi(data,
				forcePlatformBrowser: false,
				onRestrictedPaymentMethod: methodId =>
				{
					HandleRestrictedPaymentMethod(data, methodId, restrictedPaymentAllower,
					onSuccess: () =>
					{
						onSuccess?.Invoke(item);
					},
					onError);
				});

				OrderTracking.Instance.AddOrderForTracking(XsollaSettings.StoreProjectId, data.order_id, () =>
				{
					onSuccess?.Invoke(item);
				},
				onError);
			},
			onError);
		}

		public void PurchaseForVirtualCurrency(CatalogItemModel item, Action<CatalogItemModel> onSuccess = null, Action<Error> onError = null)
		{
			XsollaStore.Instance.ItemPurchaseForVirtualCurrency(
				XsollaSettings.StoreProjectId,
				item.Sku,
				item.VirtualPrice?.Key,
				onSuccess: _ =>
				{
					onSuccess?.Invoke(item);
				},
				onError);
		}

		public void PurchaseCart(List<UserCartItem> items, RestrictedPaymentAllower restrictedPaymentAllower = null, Action<List<UserCartItem>> onSuccess = null, Action<Error> onError = null)
		{
			if (!items.Any())
			{
				var error = new Error(errorMessage: "Cart is empty");
				onError?.Invoke(error);
				return;
			}

			XsollaStore.Instance.GetCartItems(XsollaSettings.StoreProjectId, newCart =>
			{
				XsollaStore.Instance.ClearCart(XsollaSettings.StoreProjectId, newCart.cart_id, () =>
				{
					var cartItems = items.Select(i => new CartFillItem
					{
						sku = i.Sku,
						quantity = i.Quantity
					}).ToList();

					XsollaStore.Instance.FillCart(XsollaSettings.StoreProjectId, cartItems, () =>
					{
						XsollaStore.Instance.CartPurchase(XsollaSettings.StoreProjectId, newCart.cart_id, data =>
						{
							XsollaStore.Instance.OpenPurchaseUi(data, false, methodId =>
							{
								HandleRestrictedPaymentMethod(data, methodId, restrictedPaymentAllower,
								onSuccess: () =>
								{
									onSuccess?.Invoke(items);
								},
								onError);

							});

							OrderTracking.Instance.AddOrderForTracking(XsollaSettings.StoreProjectId, data.order_id,
							onSuccess: () =>
							{
								onSuccess?.Invoke(items);
							},
							onError);

						}, onError);
					}, onError);
				}, onError);
			}, onError);
		}

		private void HandleRestrictedPaymentMethod(PurchaseData data, int methodId, RestrictedPaymentAllower restrictedPaymentAllower, Action onSuccess, Action<Error> onError)
		{
#if UNITY_STANDALONE || UNITY_EDITOR
			OrderTracking.Instance.RemoveOrderFromTracking(data.order_id);

			Action<bool> onAllowed = allowed =>
			{
				BrowserHelper.Instance.CloseIfExists();

				if (allowed)
				{
					XsollaStore.Instance.OpenPurchaseUi(data, true);
					OrderTracking.Instance.AddOrderForTrackingUntilDone(XsollaSettings.StoreProjectId, data.order_id, onSuccess, onError);
				}
			};

			if (restrictedPaymentAllower != null)
			{
				restrictedPaymentAllower.OnAllowed += onAllowed;
				restrictedPaymentAllower.OnRestrictedPayment.Invoke(methodId);
			}
			else
			{
				onAllowed.Invoke(true);
			}
#endif
		}
	}
}
