using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Cart;

namespace Xsolla.Tests.Cart
{
	public class CreateOrderWithFreeCartTests : CartTestsBase
	{
		[UnityTest]
		public IEnumerator CreateOrderWithFreeCart_NotFreeItem_Failure()
		{
			yield return SignInAsTestUser();
			yield return ClearCartTests.ClearCart();
			yield return FillCartTests.FillCart(new List<CartFillItem> {
				new CartFillItem {
					sku = "lootbox_1",
					quantity = 1
				}
			});
			yield return CreateOrderWithFreeCart(isSuccessExpected: false);
		}

		[UnityTest]
		public IEnumerator CreateOrderWithFreeCart_InvalidUser_Failure()
		{
			yield return SignIn();
			yield return ClearCartTests.ClearCart();
			yield return FillCartTests.FillCart(new List<CartFillItem> {
				new CartFillItem {
					sku = "Xsolla_free_item",
					quantity = 1
				}
			});
			yield return CreateOrderWithFreeCart(isSuccessExpected: false);
		}

		[UnityTest]
		public IEnumerator CreateOrderWithFreeParticularCart_NotFreeItem_Failure()
		{
			yield return SignInAsTestUser();
			yield return PrepareCurrentCart();
			yield return ClearCartTests.ClearCart(CurrentCartId);
			yield return FillCartTests.FillCart(new List<CartFillItem> {
				new CartFillItem {
					sku = "lootbox_1",
					quantity = 1
				}
			}, CurrentCartId);
			yield return CreateOrderWithFreeCart(CurrentCartId, false);
		}

		[UnityTest]
		public IEnumerator CreateOrderWithFreeParticularCart_InvalidUser_Failure()
		{
			yield return SignIn();
			yield return PrepareCurrentCart();
			yield return ClearCartTests.ClearCart(CurrentCartId);
			yield return FillCartTests.FillCart(new List<CartFillItem> {
				new CartFillItem {
					sku = "Xsolla_free_item",
					quantity = 1
				}
			}, CurrentCartId);
			yield return CreateOrderWithFreeCart(CurrentCartId, false);
		}

		private static IEnumerator CreateOrderWithFreeCart(string cartId = null, bool isSuccessExpected = true)
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaCart.CreateOrderWithFreeCart(
				orderId => {
					isComplete = true;
					if (isSuccessExpected)
					{
						Assert.NotNull(orderId);
						Assert.Greater(orderId.order_id, 0);
					}
					else
					{
						Assert.Fail("Failure expected");
					}
				},
				error => {
					isComplete = true;
					if (isSuccessExpected)
					{
						Assert.Fail(error?.errorMessage);
					}
					else
					{
						Assert.NotNull(error);
						Assert.NotNull(error.errorMessage);
					}
				},
				cartId);

			yield return new WaitUntil(() => isComplete);
		}
	}
}