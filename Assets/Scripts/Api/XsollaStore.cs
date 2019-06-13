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
		
		void OpenPurchaseUi(XsollaToken xsollaToken)
		{
			Application.OpenURL("https://secure.xsolla.com/paystation2/?access_token=" + xsollaToken.token);
		}

		public void GetListOfItems([NotNull] Action<XsollaStoreItems> onSuccess, [CanBeNull] Action<XsollaError> onError)
		{
			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/project/{0}/items/virtual_items", projectId)).Append(AdditionalUrlParams);

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), null, onSuccess, onError, XsollaError.ItemsListErrors);
		}

		public void BuyItem(string id, [CanBeNull] Action<XsollaError> onError, PurchaseParams purchaseParams = null)
		{
			var form = RequestParams(purchaseParams);

			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/payment/item/{0}", id)).Append(AdditionalUrlParams);

			WebRequestHelper.Instance.PostRequest<XsollaToken>(urlBuilder.ToString(), form, WebRequestHeader.AuthHeader(Token), OpenPurchaseUi, onError, XsollaError.BuyItemErrors);
		}

		public void CreateNewCart([NotNull] Action<XsollaCart> onSuccess, [CanBeNull] Action<XsollaError> onError)
		{
			var urlBuilder = new StringBuilder("https://store.xsolla.com/api/v1/cart").Append(AdditionalUrlParams);

			WebRequestHelper.Instance.PostRequest(urlBuilder.ToString(), null, WebRequestHeader.AuthHeader(Token), onSuccess, onError, XsollaError.CreateCartErrors);
		}

		public void AddItemToCart(XsollaCart cart, XsollaStoreItem item, XsollaQuantity quantity, [CanBeNull] Action onSuccess, [CanBeNull] Action<XsollaError> onError)
		{
			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/cart/{0}/item/{1}", cart.id, item.sku)).Append(AdditionalUrlParams);
			
			WebRequestHelper.Instance.PutRequest(urlBuilder.ToString(), JsonUtility.ToJson(quantity), WebRequestHeader.AuthHeader(Token), WebRequestHeader.ContentTypeHeader(), onSuccess, onError, XsollaError.AddToCartCartErrors);
		}

		public void ClearCart(XsollaCart cart, [CanBeNull] Action onSuccess, [CanBeNull] Action<XsollaError> onError)
		{
			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/cart/{0}/clear", cart.id)).Append(AdditionalUrlParams);

			WebRequestHelper.Instance.PutRequest(urlBuilder.ToString(), string.Empty, WebRequestHeader.AuthHeader(Token), null, onSuccess, onError, XsollaError.AddToCartCartErrors);
		}

		public void GetCartItems(XsollaCart cart, [NotNull] Action<XsollaStoreItems> onSuccess, [CanBeNull] Action<XsollaError> onError)
		{
			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/cart/{0}", cart.id)).Append(AdditionalUrlParams);
			
			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, XsollaError.GetCartItemsErrors);
		}

		public void RemoveItemFromCart(XsollaCart cart, XsollaStoreItem item, [CanBeNull] Action onSuccess, [CanBeNull] Action<XsollaError> onError)
		{
			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/cart/{0}/item/{1}", cart.id, item.sku)).Append(AdditionalUrlParams);
			
			WebRequestHelper.Instance.DeleteRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, XsollaError.DeleteFromCartErrors);
		}

		public void BuyCart(XsollaCart cart, [CanBeNull] Action<XsollaError> onError, PurchaseParams purchaseParams = null)
		{
			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/payment/cart/{0}", cart.id)).Append(AdditionalUrlParams);

			WebRequestHelper.Instance.PostRequest<XsollaToken>(urlBuilder.ToString(), RequestParams(purchaseParams), WebRequestHeader.AuthHeader(Token), OpenPurchaseUi, onError, XsollaError.BuyItemErrors);
		}
	}
}