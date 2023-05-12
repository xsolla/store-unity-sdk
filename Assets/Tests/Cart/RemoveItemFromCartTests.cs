using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Cart;

namespace Xsolla.Tests.Cart
{
	public class RemoveItemFromCartTests : CartTestsBase
	{
		[UnityTest]
		public IEnumerator RemoveItemFromCart_NoCartId_Success()
		{
			yield return RemoveItemFromCart();
		}

		[UnityTest]
		public IEnumerator RemoveItemFromCart_NoCartId_InvalidToken_Success()
		{
			yield return SetOldAccessToken();
			yield return RemoveItemFromCart();
		}

		[UnityTest]
		public IEnumerator RemoveItemFromCart_CartId_Success()
		{
			yield return PrepareCurrentCart();
			yield return RemoveItemFromCart(CurrentCartId);
		}

		[UnityTest]
		public IEnumerator RemoveItemFromCart_CartId_InvalidToken_Success()
		{
			yield return PrepareCurrentCart();
			yield return SetOldAccessToken();
			yield return RemoveItemFromCart(CurrentCartId);
		}

		private static IEnumerator RemoveItemFromCart(string cartId = null)
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaCart.RemoveItemFromCart(
				"lootbox_1",
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