using System;
using UnityEngine;

namespace Xsolla
{
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

		public void GetListOfItems(Action<XsollaStoreItems> onSuccess, Action<XsollaError> onError)
		{
			var url = string.Format("https://store.xsolla.com/api/v1/project/{0}/items/virtual_items?engine=unity&engine_v={1}&sdk=store&sdk_v={2}", 
				projectId, Application.unityVersion, Constants.SdkVersion);
			
			StartCoroutine(WebRequest.GetRequest(url,(status, response) =>
			{
				var error = CheckForErrors(status, response);
				if (error == null)
				{
					var items = new XsollaStoreItems();
					try
					{
						items = JsonUtility.FromJson<XsollaStoreItems>(response);
						
						if (onSuccess != null)
						{
							onSuccess.Invoke(items);
						}
					}
					catch (Exception)
					{
						print(response);
					}
				}
				else
				{
					if (onError != null)
					{
						onError.Invoke(error);
					}
				}
			}));
		}

		public void BuyItem(string id, Action<XsollaError> onError, string authToken = null, string currency = null)
		{
			if (string.IsNullOrEmpty(authToken))
				authToken = Token;
			
			WWWForm form = new WWWForm();
			
			if (!string.IsNullOrEmpty(currency))
				form.AddField("currency", currency);

			var url = string.Format("https://store.xsolla.com/api/v1/payment/item/{0}?engine=unity&engine_v={1}&sdk=store&sdk_v=0.1", id, Application.unityVersion);
			
			StartCoroutine(WebRequest.PostRequest(url, form, ("Authorization", "Bearer " + authToken),(status, response) =>
			{
				var error = CheckForErrors(status, response); 
				if (error == null)
				{
					try
					{
						string PS3token = JsonUtility.FromJson<XsollaToken>(response).token;
						Application.OpenURL("https://secure.xsolla.com/paystation2/?access_token=" + PS3token);
					}
					catch (Exception)
					{
						print(response);
					}
				}
				else
				{
					if (onError != null)
					{
						onError.Invoke(error);
					}
				}
			}));
		}

		XsollaError CheckForErrors(bool status, string message)
		{
			if (!status)
			{
				return new XsollaError(ErrorType.NetworkError, message);
			}

			try
			{
				var error = JsonUtility.FromJson<XsollaError>(message);
			
				if (error != null && error.statusCode != null)
				{
					return error;
				}
			}
			catch (Exception)
			{
				print(message);
			}

			return null;
		}
	}
}