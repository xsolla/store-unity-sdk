using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Cart;

namespace Xsolla.Tests.Cart
{
	public class FillCartTests : CartTestsBase
	{
		[UnityTest]
		public IEnumerator FillCart_NoCartId_Success()
		{
			yield return FillCart(new List<CartFillItem> {new CartFillItem {sku = "lootbox_1", quantity = 1}});
		}

		[UnityTest]
		public IEnumerator FillCart_NoCartId_InvalidToken_Success()
		{
			yield return SetOldAccessToken();
			yield return FillCart(new List<CartFillItem> {new CartFillItem {sku = "lootbox_1", quantity = 1}});
		}

		[UnityTest]
		public IEnumerator FillCart_CartId_Success()
		{
			yield return PrepareCurrentCart();
			yield return FillCart(new List<CartFillItem> {new CartFillItem {sku = "lootbox_1", quantity = 1}}, CurrentCartId);
		}

		[UnityTest]
		public IEnumerator FillCart_CartId_InvalidToken_Success()
		{
			yield return PrepareCurrentCart();
			yield return SetOldAccessToken();
			yield return FillCart(new List<CartFillItem> {new CartFillItem {sku = "lootbox_1", quantity = 1}}, CurrentCartId);
		}

		public static IEnumerator FillCart(List<CartFillItem> fillItems, string cartId = null)
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaCart.FillCart(
				fillItems,
				() => isComplete = true,
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				},
				cartId);

			yield return new WaitUntil(() => isComplete);
		}
	}
}