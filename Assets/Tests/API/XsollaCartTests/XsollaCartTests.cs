using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Cart;
using Xsolla.Core;

namespace Xsolla.Tests
{
	public class XsollaCartTests
	{
		private Cart.Cart _currentCart;

		[OneTimeSetUp]
		public void Clear() => TestHelper.Clear();

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			TestHelper.Clear();
			_currentCart = null;
		}

		private IEnumerator PrepareCurrentCart([CallerMemberName]string testName = null)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			if (_currentCart != null)
				yield break;

			bool busy = true;
			Error cartGetError = null;

			Action<Cart.Cart> onSuccess = cart =>
			{
				_currentCart = cart;
				busy = false;
			};

			Action<Error> onError = error =>
			{
				cartGetError = error;
				busy = false;
			};

			XsollaCart.Instance.GetCartItems(
				projectId: XsollaSettings.StoreProjectId,
				onSuccess: onSuccess,
				onError: onError);

			yield return new WaitUntil(() => !busy);

			if (cartGetError != null)
				TestHelper.Fail(cartGetError, $"PrepareCurrentCart for {testName}");
		}

		[UnityTest]
		public IEnumerator GetCartItems_DefaultValues_Success()
		{
			yield return GetCartItems(defaultValues: true);
		}

		[UnityTest]
		public IEnumerator GetCartItems_Parametrized_Success()
		{
			yield return GetCartItems(defaultValues: false);
		}

		[UnityTest]
		public IEnumerator GetCartItems_DefaultValues_InvalidToken_Success()
		{
			yield return TestSignInHelper.Instance.SetOldToken();
			yield return GetCartItems(defaultValues: true);
		}

		[UnityTest]
		public IEnumerator GetCartItems_DefaultValues_CartID_Success()
		{
			yield return PrepareCurrentCart();
			yield return GetCartItems(defaultValues: true, cartID: _currentCart.cart_id);
		}

		[UnityTest]
		public IEnumerator GetCartItems_Parametrized_CartID_Success()
		{
			yield return PrepareCurrentCart();
			yield return GetCartItems(defaultValues: false, cartID: _currentCart.cart_id);
		}

		[UnityTest]
		public IEnumerator GetCartItems_DefaultValues_CartID_InvalidToken_Success()
		{
			yield return PrepareCurrentCart();
			yield return TestSignInHelper.Instance.SetOldToken();
			yield return GetCartItems(defaultValues: true, cartID: _currentCart.cart_id);
		}

		private IEnumerator GetCartItems([CallerMemberName]string testName = null, string cartID = null, bool defaultValues = true)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action<CartItems> onSuccess = items =>
			{
				if (items == null)
				{
					errorMessage = "CONTAINER IS NULL";
					success = false;
					return;
				}

				if (items.items == null)
				{
					errorMessage = "NESTED CONTAINER IS NULL";
					success = false;
					return;
				}

				success = true;
			};

			Action<Error> onError = error =>
			{
				errorMessage = error?.errorMessage ?? "ERROR IS NULL";
				success = false;
			};

			if (cartID == null && defaultValues)
			{
				XsollaCart.Instance.GetCartItems(
					projectId: XsollaSettings.StoreProjectId,
					onSuccess: onSuccess,
					onError: onError);
			}
			else if (cartID == null && !defaultValues)
			{
				XsollaCart.Instance.GetCartItems(
					projectId: XsollaSettings.StoreProjectId,
					onSuccess: onSuccess,
					onError: onError,
					locale: "de",
					currency: "USD");
			}
			else if (cartID != null && defaultValues)
			{
				XsollaCart.Instance.GetCartItems(
					projectId: XsollaSettings.StoreProjectId,
					cartId: cartID,
					onSuccess: onSuccess,
					onError: onError);
			}
			else/*if (cartID != null && !defaultValues)*/
			{
				XsollaCart.Instance.GetCartItems(
					projectId: XsollaSettings.StoreProjectId,
					cartId: cartID,
					onSuccess: onSuccess,
					onError: onError,
					locale: "de",
					currency: "USD");
			}

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}

		[UnityTest]
		public IEnumerator FillCart_NoCartId_Success()
		{
			yield return FillCart(fillItems: new List<CartFillItem> {new CartFillItem {sku = "lootbox_1", quantity = 1}});
		}

		[UnityTest]
		public IEnumerator FillCart_NoCartId_InvalidToken_Success()
		{
			yield return TestSignInHelper.Instance.SetOldToken();
			yield return FillCart(fillItems: new List<CartFillItem> {new CartFillItem {sku = "lootbox_1", quantity = 1}});
		}

		[UnityTest]
		public IEnumerator FillCart_CartId_Success()
		{
			yield return PrepareCurrentCart();
			yield return FillCart(fillItems: new List<CartFillItem> {new CartFillItem {sku = "lootbox_1", quantity = 1}}, cartID: _currentCart.cart_id);
		}

		[UnityTest]
		public IEnumerator FillCart_CartId_InvalidToken_Success()
		{
			yield return PrepareCurrentCart();
			yield return TestSignInHelper.Instance.SetOldToken();
			yield return FillCart(fillItems: new List<CartFillItem> {new CartFillItem {sku = "lootbox_1", quantity = 1}}, cartID: _currentCart.cart_id);
		}

		private IEnumerator FillCart([CallerMemberName]string testName = null, List<CartFillItem> fillItems = null, string cartID = null)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action onSuccess = () =>
			{
				success = true;
			};

			Action<Error> onError = error =>
			{
				errorMessage = error?.errorMessage ?? "ERROR IS NULL";
				success = false;
			};

			if (cartID == null)
			{
				XsollaCart.Instance.FillCart(
					projectId: XsollaSettings.StoreProjectId,
					items: fillItems,
					onSuccess: onSuccess,
					onError: onError);
			}
			else
			{
				XsollaCart.Instance.FillCart(
					projectId: XsollaSettings.StoreProjectId,
					items: fillItems,
					cartId: cartID,
					onSuccess: onSuccess,
					onError: onError);
			}

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}

		[UnityTest]
		public IEnumerator UpdateItemInCart_NoCartId_Success()
		{
			yield return UpdateItemInCart();
		}

		[UnityTest]
		public IEnumerator UpdateItemInCart_NoCartId_InvalidToken_Success()
		{
			yield return TestSignInHelper.Instance.SetOldToken();
			yield return UpdateItemInCart();
		}

		[UnityTest]
		public IEnumerator UpdateItemInCart_CartId_Success()
		{
			yield return PrepareCurrentCart();
			yield return UpdateItemInCart(cartID: _currentCart.cart_id);
		}

		[UnityTest]
		public IEnumerator UpdateItemInCart_CartId_InvalidToken_Success()
		{
			yield return PrepareCurrentCart();
			yield return TestSignInHelper.Instance.SetOldToken();
			yield return UpdateItemInCart(cartID: _currentCart.cart_id);
		}

		private IEnumerator UpdateItemInCart([CallerMemberName]string testName = null, string cartID = null)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action onSuccess = () =>
			{
				success = true;
			};

			Action<Error> onError = error =>
			{
				errorMessage = error?.errorMessage ?? "ERROR IS NULL";
				success = false;
			};

			if (cartID == null)
			{
				XsollaCart.Instance.UpdateItemInCart(
					projectId: XsollaSettings.StoreProjectId,
					itemSku: "lootbox_1",
					quantity: 1,
					onSuccess: onSuccess,
					onError: onError);
			}
			else
			{
				XsollaCart.Instance.UpdateItemInCart(
					projectId: XsollaSettings.StoreProjectId,
					itemSku: "lootbox_1",
					quantity: 1,
					cartId: cartID,
					onSuccess: onSuccess,
					onError: onError);
			}

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}

		[UnityTest]
		public IEnumerator RemoveItemFromCart_NoCartId_Success()
		{
			yield return RemoveItemFromCart();
		}

		[UnityTest]
		public IEnumerator RemoveItemFromCart_NoCartId_InvalidToken_Success()
		{
			yield return TestSignInHelper.Instance.SetOldToken();
			yield return RemoveItemFromCart();
		}

		[UnityTest]
		public IEnumerator RemoveItemFromCart_CartId_Success()
		{
			yield return PrepareCurrentCart();
			yield return RemoveItemFromCart(cartID: _currentCart.cart_id);
		}

		[UnityTest]
		public IEnumerator RemoveItemFromCart_CartId_InvalidToken_Success()
		{
			yield return PrepareCurrentCart();
			yield return TestSignInHelper.Instance.SetOldToken();
			yield return RemoveItemFromCart(cartID: _currentCart.cart_id);
		}

		private IEnumerator RemoveItemFromCart([CallerMemberName]string testName = null, string cartID = null)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action onSuccess = () =>
			{
				success = true;
			};

			Action<Error> onError = error =>
			{
				errorMessage = error?.errorMessage ?? "ERROR IS NULL";
				success = false;
			};

			if (cartID == null)
			{
				XsollaCart.Instance.RemoveItemFromCart(
					projectId: XsollaSettings.StoreProjectId,
					itemSku: "lootbox_1",
					onSuccess: onSuccess,
					onError: onError);
			}
			else
			{
				XsollaCart.Instance.RemoveItemFromCart(
					projectId: XsollaSettings.StoreProjectId,
					itemSku: "lootbox_1",
					cartId: cartID,
					onSuccess: onSuccess,
					onError: onError);
			}

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}

		[UnityTest]
		public IEnumerator ClearCart_NoCartId_Success()
		{
			yield return ClearCart();
		}

		[UnityTest]
		public IEnumerator ClearCart_NoCartId_InvalidToken_Success()
		{
			yield return TestSignInHelper.Instance.SetOldToken();
			yield return ClearCart();
		}

		[UnityTest]
		public IEnumerator ClearCart_CartId_Success()
		{
			yield return PrepareCurrentCart();
			yield return ClearCart(cartID: _currentCart.cart_id);
		}

		[UnityTest]
		public IEnumerator ClearCart_CartId_InvalidToken_Success()
		{
			yield return PrepareCurrentCart();
			yield return TestSignInHelper.Instance.SetOldToken();
			yield return ClearCart(cartID: _currentCart.cart_id);
		}

		private IEnumerator ClearCart([CallerMemberName]string testName = null, string cartID = null)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action onSuccess = () =>
			{
				success = true;
			};

			Action<Error> onError = error =>
			{
				errorMessage = error?.errorMessage ?? "ERROR IS NULL";
				success = false;
			};

			if (cartID == null)
			{
				XsollaCart.Instance.ClearCart(
					projectId: XsollaSettings.StoreProjectId,
					onSuccess: onSuccess,
					onError: onError);
			}
			else
			{
				XsollaCart.Instance.ClearCart(
					projectId: XsollaSettings.StoreProjectId,
					cartId: cartID,
					onSuccess: onSuccess,
					onError: onError);
			}

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}

		[UnityTest]
		public IEnumerator RedeemPromocode_Success()
		{
			yield return PrepareCurrentCart();
			yield return RedeemPromocode();
		}

		[UnityTest]
		public IEnumerator RedeemPromocode_InvalidToken_Success()
		{
			yield return PrepareCurrentCart();
			yield return TestSignInHelper.Instance.SetOldToken();
			yield return RedeemPromocode();
		}

		private IEnumerator RedeemPromocode([CallerMemberName]string testName = null)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action<CartItems> onSuccess = _ =>
			{
				success = true;
			};

			Action<Error> onError = error =>
			{
				errorMessage = error?.errorMessage ?? "ERROR IS NULL";
				success = false;
			};

			XsollaCart.Instance.RedeemPromocode(
				projectId: XsollaSettings.StoreProjectId,
				promocode: "NEWGAMER2020",
				cartId: _currentCart.cart_id,
				onSuccess: onSuccess,
				onError: onError);

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}

		// [UnityTest]
		// public IEnumerator GetPromocodeReward_Success()
		// {
		// 	yield return GetPromocodeReward();
		// }

		// [UnityTest]
		// public IEnumerator GetPromocodeReward_InvalidToken_Success()
		// {
		// 	yield return TestSignInHelper.Instance.SetOldToken();
		// 	yield return GetPromocodeReward();
		// }

		private IEnumerator GetPromocodeReward([CallerMemberName]string testName = null)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action<PromocodeReward> onSuccess = _ =>
			{
				success = true;
			};

			Action<Error> onError = error =>
			{
				errorMessage = error?.errorMessage ?? "ERROR IS NULL";
				success = false;
			};

			XsollaCart.Instance.GetPromocodeReward(
				projectId: XsollaSettings.StoreProjectId,
				promocode: "NEWGAMER2020",
				onSuccess: onSuccess,
				onError: onError);

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}

		// [UnityTest]
		// public IEnumerator RemovePromocodeFromCart_Success()
		// {
		// 	yield return PrepareCurrentCart();
		// 	yield return RemovePromocodeFromCart();
		// }

		// [UnityTest]
		// public IEnumerator RemovePromocodeFromCart_InvalidToken_Success()
		// {
		// 	yield return PrepareCurrentCart();
		// 	yield return TestSignInHelper.Instance.SetOldToken();
		// 	yield return RemovePromocodeFromCart();
		// }

		private IEnumerator RemovePromocodeFromCart([CallerMemberName]string testName = null)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action<RemovePromocodeFromCartResponse> onSuccess = _ =>
			{
				success = true;
			};

			Action<Error> onError = error =>
			{
				errorMessage = error?.errorMessage ?? "ERROR IS NULL";
				success = false;
			};

			XsollaCart.Instance.RemovePromocodeFromCart(
				projectId: XsollaSettings.StoreProjectId,
				cartId: _currentCart.cart_id,
				onSuccess: onSuccess,
				onError: onError);

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}

		[UnityTest]
		public IEnumerator PurchaseCart_NoCartId_Success()
		{
			yield return PurchaseCart();
		}

		[UnityTest]
		public IEnumerator PurchaseCart_CartId_Success()
		{
			yield return PrepareCurrentCart();
			yield return PurchaseCart(cartID: _currentCart.cart_id);
		}

		private IEnumerator PurchaseCart([CallerMemberName]string testName = null, string cartID = null, PurchaseParams purchaseParams = null, Dictionary<string, string> customHeaders = null)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action<PurchaseData> onSuccess = _ =>
			{
				success = true;
			};

			Action<Error> onError = error =>
			{
				errorMessage = error?.errorMessage ?? "ERROR IS NULL";
				success = false;
			};

			if (cartID == null)
			{
				XsollaCart.Instance.PurchaseCart(
					projectId: XsollaSettings.StoreProjectId,
					onSuccess: onSuccess,
					onError: onError,
					purchaseParams,
					customHeaders);
			}
			else
			{
				XsollaCart.Instance.PurchaseCart(
					projectId: XsollaSettings.StoreProjectId,
					cartId: _currentCart.cart_id,
					onSuccess: onSuccess,
					onError: onError,
					purchaseParams,
					customHeaders);
			}

			yield return new WaitUntil(() => success.HasValue);

			if (success.Value)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}
		
		[UnityTest]
		public IEnumerator CreateOrderWithFreeCart_Success()
		{
			yield return TestSignInHelper.Instance.SignInAsTestUser();
			yield return ClearCart();
			yield return FillCart(fillItems: new List<CartFillItem> {new CartFillItem {sku = "Xsolla_free_item", quantity = 1}});
			yield return CreateOrderWithFreeCart();
		}
		
		[UnityTest]
		public IEnumerator CreateOrderWithFreeCart_NotFreeItem_Failure()
		{
			yield return TestSignInHelper.Instance.SignInAsTestUser();
			yield return ClearCart();
			yield return FillCart(fillItems: new List<CartFillItem> {new CartFillItem {sku = "lootbox_1", quantity = 1}});
			yield return CreateOrderWithFreeCart(isSuccessExpected: false);
		}
		
		[UnityTest]
		public IEnumerator CreateOrderWithFreeCart_InvalidUser_Failure()
		{
			yield return TestSignInHelper.Instance.SignIn();
			yield return ClearCart();
			yield return FillCart(fillItems: new List<CartFillItem> {new CartFillItem {sku = "Xsolla_free_item", quantity = 1}});
			yield return CreateOrderWithFreeCart(isSuccessExpected: false);
		}
		
		[UnityTest]
		public IEnumerator CreateOrderWithFreeParticularCart_Success()
		{
			yield return TestSignInHelper.Instance.SignInAsTestUser();
			yield return PrepareCurrentCart();
			yield return ClearCart(cartID: _currentCart.cart_id);
			yield return FillCart(fillItems: new List<CartFillItem> {new CartFillItem {sku = "Xsolla_free_item", quantity = 1}}, cartID: _currentCart.cart_id);
			yield return CreateOrderWithFreeCart(cartID: _currentCart.cart_id);
		}
		
		[UnityTest]
		public IEnumerator CreateOrderWithFreeParticularCart_NotFreeItem_Failure()
		{
			yield return TestSignInHelper.Instance.SignInAsTestUser();
			yield return PrepareCurrentCart();
			yield return ClearCart(cartID: _currentCart.cart_id);
			yield return FillCart(fillItems: new List<CartFillItem> {new CartFillItem {sku = "lootbox_1", quantity = 1}}, cartID: _currentCart.cart_id);
			yield return CreateOrderWithFreeCart(cartID: _currentCart.cart_id, isSuccessExpected: false);
		}
		
		[UnityTest]
		public IEnumerator CreateOrderWithFreeParticularCart_InvalidUser_Failure()
		{
			yield return TestSignInHelper.Instance.SignIn();
			yield return PrepareCurrentCart();
			yield return ClearCart(cartID: _currentCart.cart_id);
			yield return FillCart(fillItems: new List<CartFillItem> {new CartFillItem {sku = "Xsolla_free_item", quantity = 1}}, cartID: _currentCart.cart_id);
			yield return CreateOrderWithFreeCart(cartID: _currentCart.cart_id, isSuccessExpected: false);
		}

		private IEnumerator CreateOrderWithFreeCart([CallerMemberName]string testName = null, string cartID = null, bool isSuccessExpected = true)
		{
			yield return TestSignInHelper.Instance.CheckSession();

			bool? success = default;
			string errorMessage = default;

			Action<int> onSuccess = orderId =>
			{
				success = true;
			};

			Action<Error> onError = error =>
			{
				errorMessage = error?.errorMessage ?? "ERROR IS NULL";
				success = false;
			};

			if (cartID == null)
			{
				XsollaCart.Instance.CreateOrderWithFreeCart(
					XsollaSettings.StoreProjectId,
					onSuccess,
					onError
				);
			}
			else
			{
				XsollaCart.Instance.CreateOrderWithParticularFreeCart(
					XsollaSettings.StoreProjectId,
					cartID,
					onSuccess,
					onError
				);
			}
			
			yield return new WaitUntil(() => success.HasValue);
			
			if (success.Value == isSuccessExpected)
				TestHelper.Pass(testName);
			else
				TestHelper.Fail(errorMessage, testName);
		}
	}
}
