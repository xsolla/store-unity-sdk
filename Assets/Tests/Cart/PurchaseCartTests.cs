using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Cart;

namespace Xsolla.Tests.Cart
{
	public class PurchaseCartTests : CartTestsBase
	{
		[UnityTest]
		public IEnumerator PurchaseCart_NoCartId_Success()
		{
			yield return PurchaseCart();
		}

		[UnityTest]
		public IEnumerator PurchaseCart_CartId_Success()
		{
			yield return PrepareCurrentCart();
			yield return PurchaseCart(CurrentCartId);
		}

		private IEnumerator PurchaseCart(string cartId = null)
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaCart.CreateOrder(
				_ => isComplete = true,
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