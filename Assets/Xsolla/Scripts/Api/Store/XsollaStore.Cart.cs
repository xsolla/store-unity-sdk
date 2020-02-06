using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Store
{
	public partial class XsollaStore : MonoSingleton<XsollaStore>
	{
		private const string URL_CART_CREATE_NEW = "https://store.xsolla.com/api/v2/project/{0}/cart";
		private const string URL_CART_ITEM_ADD = "https://store.xsolla.com/api/v2/project/{0}/cart/{1}/item/{2}";
		private const string URL_CART_ITEM_REMOVE = "https://store.xsolla.com/api/v2/project/{0}/cart/{1}/item/{2}";
		private const string URL_CART_GET_ITEMS = "https://store.xsolla.com/api/v2/project/{0}/cart/{1}";
		private const string URL_CART_CLEAR = "https://store.xsolla.com/api/v2/project/{0}/cart/{1}/clear";		

		public void CreateNewCart(string projectId, [NotNull] Action<Cart> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CART_CREATE_NEW, projectId)).Append(AdditionalUrlParams);

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.CreateCartErrors);
		}

		public void AddItemToCart(string projectId, string cartId, string itemSku, int quantity, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CART_ITEM_ADD, projectId, cartId, itemSku)).Append(AdditionalUrlParams);

			Quantity jsonObject = new Quantity { quantity = quantity };

			WebRequestHelper.Instance.PutRequest<Quantity>(urlBuilder.ToString(), jsonObject, WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.AddToCartCartErrors);
		}

		public void ClearCart(string projectId, string cartId, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CART_CLEAR, projectId, cartId)).Append(AdditionalUrlParams);

			WebRequestHelper.Instance.PutRequest<Quantity>(urlBuilder.ToString(), null, WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.AddToCartCartErrors);
		}

		public void GetCartItems(string projectId, string cartId, [NotNull] Action<CartItems> onSuccess, [CanBeNull] Action<Error> onError, [CanBeNull] string locale = null, [CanBeNull] string currency = null)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CART_GET_ITEMS, projectId, cartId)).Append(AdditionalUrlParams);
			urlBuilder.Append(GetLocaleUrlParam(locale));
			urlBuilder.Append(GetCurrencyUrlParam(currency));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.GetCartItemsErrors);
		}

		public void RemoveItemFromCart(string projectId, string cartId, string itemSku, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format(URL_CART_ITEM_REMOVE, projectId, cartId, itemSku)).Append(AdditionalUrlParams);

			WebRequestHelper.Instance.DeleteRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.DeleteFromCartErrors);
		}
	}
}