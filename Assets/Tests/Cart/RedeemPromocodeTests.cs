using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Cart;

namespace Xsolla.Tests.Cart
{
	public class RedeemPromocodeTests : CartTestsBase
	{
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
			yield return SetOldAccessToken();
			yield return RedeemPromocode();
		}

		private IEnumerator RedeemPromocode()
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaCart.RedeemPromocode(
				"NEWGAMER2020",
				CurrentCartId,
				cart =>
				{
					isComplete = true;
					Assert.NotNull(cart);
					Assert.NotNull(cart.items);
				},
				error =>
				{
					isComplete = true;
					Assert.Fail(error?.errorMessage);
				});

			yield return new WaitUntil(() => isComplete);
		}
	}
}