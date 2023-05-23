using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Cart;

namespace Xsolla.Tests.Cart
{
	public class UpdateItemInCartTests : CartTestsBase
	{
		[UnityTest]
		public IEnumerator UpdateItemInCart_NoCartId_Success()
		{
			yield return UpdateItemInCart();
		}

		[UnityTest]
		public IEnumerator UpdateItemInCart_NoCartId_InvalidToken_Success()
		{
			yield return SetOldAccessToken();
			yield return UpdateItemInCart();
		}

		[UnityTest]
		public IEnumerator UpdateItemInCart_CartId_Success()
		{
			yield return PrepareCurrentCart();
			yield return UpdateItemInCart(CurrentCartId);
		}

		[UnityTest]
		public IEnumerator UpdateItemInCart_CartId_InvalidToken_Success()
		{
			yield return PrepareCurrentCart();
			yield return SetOldAccessToken();
			yield return UpdateItemInCart(CurrentCartId);
		}

		private static IEnumerator UpdateItemInCart(string cartId = null)
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaCart.UpdateItemInCart(
				"lootbox_1",
				1,
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