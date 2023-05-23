using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Cart;

namespace Xsolla.Tests.Cart
{
	public class ClearCartTests : CartTestsBase
	{
		[UnityTest]
		public IEnumerator ClearCart_NoCartId_Success()
		{
			yield return ClearCart();
		}

		[UnityTest]
		public IEnumerator ClearCart_NoCartId_InvalidToken_Success()
		{
			yield return SetOldAccessToken();
			yield return ClearCart();
		}

		[UnityTest]
		public IEnumerator ClearCart_CartId_Success()
		{
			yield return PrepareCurrentCart();
			yield return ClearCart(CurrentCartId);
		}

		[UnityTest]
		public IEnumerator ClearCart_CartId_InvalidToken_Success()
		{
			yield return PrepareCurrentCart();
			yield return SetOldAccessToken();
			yield return ClearCart(CurrentCartId);
		}

		public static IEnumerator ClearCart(string cartId = null)
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaCart.ClearCart(
				() => { isComplete = true; },
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