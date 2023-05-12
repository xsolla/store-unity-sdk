using System.Collections;
using NUnit.Framework;
using UnityEngine;
using Xsolla.Cart;

namespace Xsolla.Tests.Cart
{
	public class CartTestsBase : TestBase
	{
		[OneTimeSetUp]
		[OneTimeTearDown]
		public void Clear()
		{
			ClearEnv();
			CurrentCartId = null;
		}

		protected string CurrentCartId { get; private set; }

		protected IEnumerator PrepareCurrentCart()
		{
			yield return CheckSession();

			if (!string.IsNullOrEmpty(CurrentCartId))
				yield break;

			var isComplete = false;
			XsollaCart.GetCartItems(
				cart =>
				{
					isComplete = true;
					CurrentCartId = cart.cart_id;
					Assert.NotNull(cart);
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