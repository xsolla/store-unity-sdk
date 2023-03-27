using System;
using System.Collections.Generic;
using System.Linq;
using Xsolla.Cart;
using Xsolla.Catalog;
using Xsolla.Core;
using Xsolla.Orders;

namespace Xsolla.Demo
{
	public class SdkPurchaseLogic : MonoSingleton<SdkPurchaseLogic>
	{
		public event Action<CatalogItemModel> PurchaseForFreeEvent;
		public event Action<CatalogItemModel> PurchaseForRealMoneyEvent;
		public event Action<CatalogItemModel> PurchaseForVirtualCurrencyEvent;
		public event Action<List<UserCartItem>> PurchaseCartEvent;

		public void PurchaseFreeItem(CatalogItemModel item, Action<CatalogItemModel> onSuccess = null, Action<Error> onError = null)
		{
			XsollaCatalog.Instance.CreateOrderWithSpecifiedFreeItem(
				XsollaSettings.StoreProjectId,
				item.Sku,
				onSuccess: i =>
				{
					onSuccess?.Invoke(item);
					PurchaseForFreeEvent?.Invoke(item);
				},
				onError);
		}
		
		public void PurchaseForRealMoney(CatalogItemModel item, RestrictedPaymentAllower restrictedPaymentAllower = null, Action<CatalogItemModel> onSuccess = null, Action<Error> onError = null)
		{
			XsollaCatalog.Instance.PurchaseItem(XsollaSettings.StoreProjectId, item.Sku,
			onSuccess: data =>
			{
				XsollaOrders.Instance.OpenPurchaseUi(data,
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
					PurchaseForRealMoneyEvent?.Invoke(item);
					
					if (BrowserHelper.Instance.InAppBrowser?.IsOpened ?? false)
						BrowserHelper.Instance.Close();
				},
				onError);
			},
			onError, customHeaders: GenerateCustomHeaders());
		}

		public void PurchaseForVirtualCurrency(CatalogItemModel item, Action<CatalogItemModel> onSuccess = null, Action<Error> onError = null)
		{
			XsollaCatalog.Instance.PurchaseItemForVirtualCurrency(
				XsollaSettings.StoreProjectId,
				item.Sku,
				item.VirtualPrice?.Key,
				data =>
				{
					OrderTracking.Instance.AddVirtualCurrencyOrderForTracking(
						XsollaSettings.StoreProjectId,
						data.order_id,
						() =>
						{
							onSuccess?.Invoke(item);
							PurchaseForVirtualCurrencyEvent?.Invoke(item);
						},
						onError
					);
				},
				onError, customHeaders: GenerateCustomHeaders());
		}

		public void PurchaseCart(List<UserCartItem> items, RestrictedPaymentAllower restrictedPaymentAllower = null, Action<List<UserCartItem>> onSuccess = null, Action<Error> onError = null)
		{
			if (!items.Any())
			{
				var error = new Error(errorMessage: "Cart is empty");
				onError?.Invoke(error);
				return;
			}

			XsollaCart.Instance.GetCartItems(XsollaSettings.StoreProjectId, newCart =>
			{
				XsollaCart.Instance.ClearCart(XsollaSettings.StoreProjectId, newCart.cart_id, () =>
				{
					var cartItems = items.Select(i => new CartFillItem
					{
						sku = i.Sku,
						quantity = i.Quantity
					}).ToList();

					XsollaCart.Instance.FillCart(XsollaSettings.StoreProjectId, cartItems, () =>
					{
						XsollaCart.Instance.PurchaseCart(XsollaSettings.StoreProjectId, newCart.cart_id, data =>
						{
							XsollaOrders.Instance.OpenPurchaseUi(data, false, methodId =>
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
								PurchaseCartEvent?.Invoke(items);
								
								if (BrowserHelper.Instance.InAppBrowser?.IsOpened ?? false)
									BrowserHelper.Instance.Close();
							},
							onError);

						}, onError, customHeaders: GenerateCustomHeaders());
					}, onError);
				}, onError);
			}, onError);
		}
		
		public void PurchaseFreeCart(List<UserCartItem> items, Action<List<UserCartItem>> onSuccess = null, Action<Error> onError = null)
		{
			if (!items.Any())
			{
				var error = new Error(errorMessage: "Cart is empty");
				onError?.Invoke(error);
				return;
			}

			XsollaCart.Instance.GetCartItems(XsollaSettings.StoreProjectId, newCart =>
			{
				XsollaCart.Instance.ClearCart(XsollaSettings.StoreProjectId, newCart.cart_id, () =>
				{
					var cartItems = items.Select(i => new CartFillItem
					{
						sku = i.Sku,
						quantity = i.Quantity
					}).ToList();

					XsollaCart.Instance.FillCart(XsollaSettings.StoreProjectId, cartItems, () =>
					{
						XsollaCart.Instance.CreateOrderWithFreeCart(XsollaSettings.StoreProjectId, orderId => onSuccess?.Invoke(items), onError);
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
				BrowserHelper.Instance.InAppBrowser?.Close();

				if (allowed)
				{
					XsollaOrders.Instance.OpenPurchaseUi(data, forcePlatformBrowser:true);
					OrderTracking.Instance.AddOrderForShortPollingTracking(XsollaSettings.StoreProjectId, data.order_id, onSuccess, onError);
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

		private Dictionary<string, string> GenerateCustomHeaders()
		{
#if UNITY_STANDALONE || UNITY_EDITOR
			if (DemoSettings.UseSteamAuth && DemoSettings.PaymentsFlow == PaymentsFlow.SteamGateway)
				return new Dictionary<string, string>{{"x-steam-userid", Token.Instance.GetSteamUserID()}};
#endif
			return null;
		}
	}
}
