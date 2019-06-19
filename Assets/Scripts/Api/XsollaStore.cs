using System;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace Xsolla
{
	[PublicAPI]
	public class XsollaStore : MonoSingleton<XsollaStore>
	{
		[SerializeField]
		string projectId;

		public string ProjectId
		{
			get { return projectId; }
			set { projectId = value; }
		}

		public string Token
		{
			set { PlayerPrefs.SetString(Constants.XsollaStoreToken, value); }
			get { return PlayerPrefs.GetString(Constants.XsollaStoreToken, string.Empty); }
		}
		
		string AdditionalUrlParams
		{
			get
			{
				return string.Format("?engine=unity&engine_v={0}&sdk=store&sdk_v={1}", Application.unityVersion, Constants.SdkVersion);
			}
		}
		
		WWWForm RequestParams(PurchaseParams purchaseParams)
		{
			var form = new WWWForm();

			if (purchaseParams == null)
			{
				return form;
			}

			if (!string.IsNullOrEmpty(purchaseParams.currency))
			{
				form.AddField("currency", purchaseParams.currency);
			}
			if (!string.IsNullOrEmpty(purchaseParams.country))
			{
				form.AddField("country", purchaseParams.country);
			}
			if (!string.IsNullOrEmpty(purchaseParams.locale))
			{
				form.AddField("locale", purchaseParams.locale);
			}

			return form;
		}
		
		void OpenPurchaseUi(Token xsollaToken)
		{
			Application.OpenURL("https://secure.xsolla.com/paystation2/?access_token=" + xsollaToken.token);
		}

		public void GetListOfItems([NotNull] Action<StoreItems> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/project/{0}/items/virtual_items", projectId)).Append(AdditionalUrlParams);

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), null, onSuccess, onError, Error.ItemsListErrors);
		}

		public void BuyItem(string id, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null)
		{
			var form = RequestParams(purchaseParams);

			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/payment/item/{0}", id)).Append(AdditionalUrlParams);

			WebRequestHelper.Instance.PostRequest<Token>(urlBuilder.ToString(), form, WebRequestHeader.AuthHeader(Token), OpenPurchaseUi, onError, Error.BuyItemErrors);
		}

		public void CreateNewCart([NotNull] Action<Cart> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder("https://store.xsolla.com/api/v1/cart").Append(AdditionalUrlParams);

			WebRequestHelper.Instance.PostRequest(urlBuilder.ToString(), null, WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.CreateCartErrors);
		}

		public void AddItemToCart(Cart cart, string itemSku, Quantity quantity, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/cart/{0}/item/{1}", cart.id, itemSku)).Append(AdditionalUrlParams);
			
			WebRequestHelper.Instance.PutRequest(urlBuilder.ToString(), JsonUtility.ToJson(quantity), WebRequestHeader.AuthHeader(Token), WebRequestHeader.ContentTypeHeader(), onSuccess, onError, Error.AddToCartCartErrors);
		}

		public void ClearCart(Cart cart, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/cart/{0}/clear", cart.id)).Append(AdditionalUrlParams);

			WebRequestHelper.Instance.PutRequest(urlBuilder.ToString(), string.Empty, WebRequestHeader.AuthHeader(Token), null, onSuccess, onError, Error.AddToCartCartErrors);
		}

		public void GetCartItems(Cart cart, [NotNull] Action<CartItems> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/cart/{0}", cart.id)).Append(AdditionalUrlParams);
			
			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.GetCartItemsErrors);
		}

		public void RemoveItemFromCart(Cart cart, string itemSku, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/cart/{0}/item/{1}", cart.id, itemSku)).Append(AdditionalUrlParams);
			
			WebRequestHelper.Instance.DeleteRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.DeleteFromCartErrors);
		}

		public void BuyCart(Cart cart, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null)
		{
			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/payment/cart/{0}", cart.id)).Append(AdditionalUrlParams);

			WebRequestHelper.Instance.PostRequest<Token>(urlBuilder.ToString(), RequestParams(purchaseParams), WebRequestHeader.AuthHeader(Token), OpenPurchaseUi, onError, Error.BuyItemErrors);
		}
	}
}