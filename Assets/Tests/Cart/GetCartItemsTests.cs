using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Cart;

namespace Xsolla.Tests.Cart
{
	public class GetCartItemsTests : CartTestsBase
	{
		[UnityTest]
		public IEnumerator GetCartItems_DefaultValues_Success()
		{
			yield return GetCartItems();
		}

		[UnityTest]
		public IEnumerator GetCartItems_Parametrized_Success()
		{
			yield return GetCartItems();
		}

		[UnityTest]
		public IEnumerator GetCartItems_DefaultValues_InvalidToken_Success()
		{
			yield return SetOldAccessToken();
			yield return GetCartItems();
		}

		[UnityTest]
		public IEnumerator GetCartItems_DefaultValues_CartID_Success()
		{
			yield return PrepareCurrentCart();
			yield return GetCartItems(CurrentCartId);
		}

		[UnityTest]
		public IEnumerator GetCartItems_Parametrized_CartID_Success()
		{
			yield return PrepareCurrentCart();
			yield return GetCartItems(CurrentCartId, "en", "USD");
		}

		[UnityTest]
		public IEnumerator GetCartItems_DefaultValues_CartID_InvalidToken_Success()
		{
			yield return PrepareCurrentCart();
			yield return SetOldAccessToken();
			yield return GetCartItems(CurrentCartId);
		}

		private static IEnumerator GetCartItems(string cartId = null, string locale = null, string currency = null)
		{
			yield return CheckSession();

			var isComplete = false;
			XsollaCart.GetCartItems(
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
				},
				cartId,
				locale,
				currency);

			yield return new WaitUntil(() => isComplete);
		}
	}
}