using System;
using System.Collections.Generic;
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

			StartCoroutine(WebRequest.GetRequest(url, (response) =>
			{
				var error = CheckForErrors(response, XsollaError.ItemsListErrors);
				if (error == null)
				{
					var items = ParseUtils.ParseStoreItems(response);
					if (items != null)
					{
						onSuccess(items);
					}
					else
					{
						if (onError != null)
						{
							onError(new XsollaError {ErrorType = ErrorType.UnknownError});
						}
					}
				}
				else
				{
					if (onError != null)
					{
						onError(error);
					}
				}
			}, onError));
		}

		public void BuyItem(string id, [CanBeNull] Action<XsollaError> onError, string authToken = null, string currency = null)
		{
			if (string.IsNullOrEmpty(authToken))
				authToken = Token;

			WWWForm form = new WWWForm();

			if (!string.IsNullOrEmpty(currency))
				form.AddField("currency", currency);

			var url = string.Format("https://store.xsolla.com/api/v1/payment/item/{0}?engine=unity&engine_v={1}&sdk=store&sdk_v=0.1", id, Application.unityVersion);

			var requestHeader = new WebRequestHeader {Name = "Authorization", Value = string.Format("Bearer {0}", authToken)};
			
			StartCoroutine(WebRequest.PostRequest(url, form, requestHeader,(response) =>
			{
				var error = CheckForErrors(response, XsollaError.BuyItemErrors);
				if (error == null)
				{
					var payStationToken = ParseUtils.ParseToken(response);
					if (payStationToken != null)
					{
						Application.OpenURL("https://secure.xsolla.com/paystation2/?access_token=" + payStationToken.token);
					}
					else
					{
						if (onError != null)
						{
							onError(new XsollaError {ErrorType = ErrorType.UnknownError});
						}
					}
				}
				else
				{
					if (onError != null)
					{
						onError(error);
					}
				}
			}, onError));
		}

		XsollaError CheckForErrors(string json, Dictionary<string, ErrorType> errorsToCheck)
		{
			var error = ParseUtils.ParseError(json);
			if (error != null && !string.IsNullOrEmpty(error.statusCode))
			{
				if (errorsToCheck != null && errorsToCheck.ContainsKey(error.statusCode))
				{
					error.ErrorType = errorsToCheck[error.statusCode];
					return error;
				}

				if (XsollaError.GeneralErrors.ContainsKey(error.statusCode))
				{
					error.ErrorType = XsollaError.GeneralErrors[error.statusCode];
					return error;
				}

				return new XsollaError {ErrorType = ErrorType.UnknownError};
			}

			return null;
		}
	}
}