using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public partial interface IStoreDemoImplementation
	{
		void GetCartItems([NotNull] Action<UserCartModel> onSuccess, [CanBeNull] Action<Error> onError = null);
		void UpdateCartItem(string sku, int quantity, [NotNull] Action onSuccess, [CanBeNull] Action<Error> onError = null);
		void RemoveCartItem(string sku, [NotNull] Action onSuccess, [CanBeNull] Action<Error> onError = null);
	}
}