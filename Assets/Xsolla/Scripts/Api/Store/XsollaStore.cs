using System;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Store
{
	[PublicAPI]
	public class XsollaStore : MonoSingleton<XsollaStore>
	{ 
		public string Token
		{
			set { PlayerPrefs.SetString(Constants.XsollaStoreToken, value); }
			get { return PlayerPrefs.GetString(Constants.XsollaStoreToken, string.Empty); }
		}
		
		string AdditionalUrlParams
		{
			get
			{
				return string.Format("?engine=unity&engine_v={0}&sdk=store&sdk_v={1}", Application.unityVersion, Constants.StoreSdkVersion);
			}
		}

		string GetLocaleUrlParam(string locale)
		{
			if (string.IsNullOrEmpty(locale))
			{
				return string.Empty;
			}
			
			return string.Format("&locale={0}", locale);
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
		
		public void OpenPurchaseUi(PurchaseData purchaseData)
		{
			if (XsollaSettings.IsSandbox)
			{
				if(Application.platform==RuntimePlatform.WebGLPlayer)
				{
					var str = string.Format("window.open(\"{0}\",\"_blank\")", "https://sandbox-secure.xsolla.com/paystation2/?access_token=" + purchaseData.token);
					Application.ExternalEval(str);
					return;
				}
				
				Application.OpenURL("https://sandbox-secure.xsolla.com/paystation2/?access_token=" + purchaseData.token);
			}
			else
			{
				if(Application.platform==RuntimePlatform.WebGLPlayer)
				{
					var str = string.Format("window.open(\"{0}\",\"_blank\")", "https://secure.xsolla.com/paystation2/?access_token=" + purchaseData.token);
					Application.ExternalEval(str);
					return;
				}
				
				Application.OpenURL("https://secure.xsolla.com/paystation2/?access_token=" + purchaseData.token);
			}
		}

		public void GetListOfItems(string projectId, [NotNull] Action<StoreItems> onSuccess, [CanBeNull] Action<Error> onError, string locale = null)
		{
			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/project/{0}/items/virtual_items", projectId)).Append(AdditionalUrlParams);
			urlBuilder.Append(GetLocaleUrlParam(locale));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), null, onSuccess, onError, Error.ItemsListErrors);
		}

		public void GetListOfItemGroups(string projectId, [NotNull] Action<Groups> onSuccess, [CanBeNull] Action<Error> onError, string locale = null)
		{
			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/project/{0}/items/groups", projectId)).Append(AdditionalUrlParams);
			urlBuilder.Append(GetLocaleUrlParam(locale));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), null, onSuccess, onError);
		}

		public void BuyItem(string projectId, string itemId, [CanBeNull] Action<PurchaseData> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null)
		{
			var form = RequestParams(purchaseParams);

			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/project/{0}/payment/item/{1}", projectId, itemId)).Append(AdditionalUrlParams);

			WebRequestHelper.Instance.PostRequest<PurchaseData>(urlBuilder.ToString(), form, WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.BuyItemErrors);
		}

		public void CreateNewCart(string projectId, [NotNull] Action<Cart> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/project/{0}/cart", projectId)).Append(AdditionalUrlParams);

			WebRequestHelper.Instance.PostRequest(urlBuilder.ToString(), null, WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.CreateCartErrors);
		}

		public void AddItemToCart(string projectId, string cartId, string itemSku, int quantity, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/project/{0}/cart/{1}/item/{2}", projectId, cartId, itemSku)).Append(AdditionalUrlParams);

			WebRequestHelper.Instance.PutRequest(urlBuilder.ToString(), JsonUtility.ToJson(new Quantity {quantity = quantity}), WebRequestHeader.AuthHeader(Token), WebRequestHeader.ContentTypeHeader(), onSuccess, onError, Error.AddToCartCartErrors);
		}

		public void ClearCart(string projectId, string cartId, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/project/{0}/cart/{1}/clear", projectId, cartId)).Append(AdditionalUrlParams);

			WebRequestHelper.Instance.PutRequest(urlBuilder.ToString(), string.Empty, WebRequestHeader.AuthHeader(Token), null, onSuccess, onError, Error.AddToCartCartErrors);
		}

		public void GetCartItems(string projectId, string cartId, [NotNull] Action<CartItems> onSuccess, [CanBeNull] Action<Error> onError, string locale = null)
		{
			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/project/{0}/cart/{1}", projectId, cartId)).Append(AdditionalUrlParams);
			urlBuilder.Append(GetLocaleUrlParam(locale));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.GetCartItemsErrors);
		}

		public void RemoveItemFromCart(string projectId, string cartId, string itemSku, [CanBeNull] Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/project/{0}/cart/{1}/item/{2}", projectId, cartId, itemSku)).Append(AdditionalUrlParams);

			WebRequestHelper.Instance.DeleteRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.DeleteFromCartErrors);
		}

		public void BuyCart(string projectId, string cartId,[CanBeNull] Action<PurchaseData> onSuccess, [CanBeNull] Action<Error> onError, PurchaseParams purchaseParams = null)
		{
			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/project/{0}/payment/cart/{1}", projectId, cartId)).Append(AdditionalUrlParams);

			WebRequestHelper.Instance.PostRequest<PurchaseData>(urlBuilder.ToString(), RequestParams(purchaseParams), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.BuyCartErrors);
		}

		public void CheckOrderStatus(string projectId, int orderId, [NotNull] Action<OrderStatus> onSuccess, [CanBeNull] Action<Error> onError)
		{
			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/project/{0}/order/{1}", projectId, orderId)).Append(AdditionalUrlParams);

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.OrderStatusErrors);
		}

		public void GetInventoryItems(string projectId, [NotNull] Action<InventoryItems> onSuccess, [CanBeNull] Action<Error> onError, string locale = null)
		{
			var urlBuilder = new StringBuilder(string.Format("https://store.xsolla.com/api/v1/project/{0}/user/inventory/items", projectId)).Append(AdditionalUrlParams);
			urlBuilder.Append(GetLocaleUrlParam(locale));

			WebRequestHelper.Instance.GetRequest(urlBuilder.ToString(), WebRequestHeader.AuthHeader(Token), onSuccess, onError, Error.ItemsListErrors);
		}
	}
}