using System;
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

		public void GetListOfItems([NotNull] Action<XsollaStoreItems> onSuccess, [CanBeNull] Action<XsollaError> onError)
		{
			if (onSuccess == null)
			{
				throw new ArgumentNullException("onSuccess");
			}

			var url = string.Format("https://store.xsolla.com/api/v1/project/{0}/items/virtual_items?engine=unity&engine_v={1}&sdk=store&sdk_v={2}", projectId, Application.unityVersion, Constants.SdkVersion);

			WebRequestHelper.Instance.GetRequest<XsollaStoreItems>(url, onSuccess, onError, XsollaError.ItemsListErrors);
		}

		public void BuyItem(string id, [CanBeNull] Action<XsollaError> onError, string authToken = null, string currency = null)
		{
			if (string.IsNullOrEmpty(authToken))
			{
				authToken = Token;
			}

			WWWForm form = new WWWForm();

			if (!string.IsNullOrEmpty(currency))
			{
				form.AddField("currency", currency);
			}

			var url = string.Format("https://store.xsolla.com/api/v1/payment/item/{0}?engine=unity&engine_v={1}&sdk=store&sdk_v=0.1", id, Application.unityVersion);

			var requestHeader = new WebRequestHeader {Name = "Authorization", Value = string.Format("Bearer {0}", authToken)};
			
			WebRequestHelper.Instance.PostRequest<XsollaToken>(url, form, requestHeader, xsollaToken =>
			{
				Application.OpenURL("https://secure.xsolla.com/paystation2/?access_token=" + xsollaToken.token);
			}, onError, XsollaError.BuyItemErrors);
		}
	}
}