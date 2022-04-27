using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xsolla.Cart;
using Xsolla.Core;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		[Obsolete("Use XsollaCart instead")]
		public void GetCartItems(string projectId, [NotNull] Action<Cart.Cart> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, [CanBeNull] string currency = null)
			=> XsollaCart.Instance.GetCartItems(projectId, onSuccess, onError, locale, currency);

		[Obsolete("Use XsollaCart instead")]
		public void GetCartItems(string projectId, string cartId, [NotNull] Action<CartItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, [CanBeNull] string currency = null)
			=> XsollaCart.Instance.GetCartItems(projectId, cartId, onSuccess, onError, locale, currency);

		[Obsolete("Use XsollaCart instead")]
		public void FillCart(string projectId, List<CartFillItem> items, [NotNull] Action onSuccess, [CanBeNull] Action<Error> onError)
			=> XsollaCart.Instance.FillCart(projectId, items, onSuccess, onError);

		[Obsolete("Use XsollaCart instead")]
		public void FillCart(string projectId, string cartId, List<CartFillItem> items, [NotNull] Action onSuccess, [CanBeNull] Action<Error> onError)
			=> XsollaCart.Instance.FillCart(projectId, cartId, items, onSuccess, onError);

		[Obsolete("Use XsollaCart instead")]
		public void UpdateItemInCart(string projectId, string itemSku, int quantity, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
			=> XsollaCart.Instance.UpdateItemInCart(projectId, itemSku, quantity, onSuccess, onError);

		[Obsolete("Use XsollaCart instead")]
		public void UpdateItemInCart(string projectId, string cartId, string itemSku, int quantity, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
			=> XsollaCart.Instance.UpdateItemInCart(projectId, cartId, itemSku, quantity, onSuccess, onError);

		[Obsolete("Use XsollaCart instead")]
		public void ClearCart(string projectId, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
			=> XsollaCart.Instance.ClearCart(projectId, onSuccess, onError);

		[Obsolete("Use XsollaCart instead")]
		public void ClearCart(string projectId, string cartId, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
			=> XsollaCart.Instance.ClearCart(projectId, cartId, onSuccess, onError);

		[Obsolete("Use XsollaCart instead")]
		public void RemoveItemFromCart(string projectId, string itemSku, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
			=> XsollaCart.Instance.RemoveItemFromCart(projectId, itemSku, onSuccess, onError);

		[Obsolete("Use XsollaCart instead")]
		public void RemoveItemFromCart(string projectId, string cartId, string itemSku, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
			=> XsollaCart.Instance.RemoveItemFromCart(projectId, cartId, itemSku, onSuccess, onError);

		[Obsolete("Use XsollaCart instead")]
		public void RedeemPromocode(string projectId, string promocode, string cartId, [NotNull] Action<CartItems> onSuccess, [CanBeNull] Action<Error> onError)
			=> XsollaCart.Instance.RedeemPromocode(projectId, promocode, cartId, onSuccess, onError);

		[Obsolete("Use XsollaCart instead")]
		public void GetPromocodeReward(string projectId, string promocode, [NotNull] Action<PromocodeReward> onSuccess, [CanBeNull] Action<Error> onError)
			=> XsollaCart.Instance.GetPromocodeReward(projectId, promocode, onSuccess, onError);

		[Obsolete("Use XsollaCart instead")]
		public void RemovePromocodeFromCart(string projectId, string cartId, Action<RemovePromocodeFromCartResponse> onSuccess, Action<Error> onError = null)
			=> XsollaCart.Instance.RemovePromocodeFromCart(projectId, cartId, onSuccess, onError);
	}
}
