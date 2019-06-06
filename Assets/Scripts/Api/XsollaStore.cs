using System;
using UnityEngine;

namespace Xsolla
{
	public class XsollaStore : MonoSingleton<XsollaStore>
	{
		public event Action<XsollaStoreItems> OnSuccessGetListOfItems;

		//409 
		public event Action<XsollaError> OnInvalidProjectSettings;

		//422 
		public event Action<XsollaError> OnInvalidData;

		public event Action<XsollaError> OnIdentifiedError;
		public event Action OnNetworkError;

		public string Project_Id
		{
			get { return _project_id; }
			set { _project_id = value; }
		}

		public string Token
		{
			set { PlayerPrefs.SetString(Constants.XsollaStoreToken, value); }
			get { return PlayerPrefs.HasKey(Constants.XsollaStoreToken) ? PlayerPrefs.GetString(Constants.XsollaStoreToken) : string.Empty; }
		}

		[SerializeField] string _project_id;

		/// <summary>
		/// Get list of items.
		/// </summary>
		public void GetListOfItems()
		{
			StartCoroutine(WebRequest.GetRequest(
				"https://store.xsolla.com/api/v1/project/" + _project_id +
				"/items/virtual_items?engine=unity&engine_v=" + Application.unityVersion + "&sdk=store&sdk_v=0.1",
				(status, message) =>
				{
					if (!CheckForErrors(status, message, null))
					{
						XsollaStoreItems items = new XsollaStoreItems();
						try
						{
							message = message.Remove(0, 8).Insert(0, "{\"storeItems\"");
							items = JsonUtility.FromJson<XsollaStoreItems>(message);
						}
						catch (Exception)
						{
						}

						if (OnSuccessGetListOfItems != null)
							OnSuccessGetListOfItems.Invoke(items);
					}
				}));
		}

		/// <summary>
		/// Open PayStation.
		/// </summary>
		public void BuyItem(string id, string authorizationJWTtoken = "", string currency = "")
		{
			if (authorizationJWTtoken == "")
				authorizationJWTtoken = Token;
			WWWForm form = new WWWForm();
			if (currency != "")
				form.AddField("currency", currency);

			StartCoroutine(WebRequest.PostRequest(
				"https://store.xsolla.com/api/v1/payment/item/" + id + "?engine=unity&engine_v=" +
				Application.unityVersion + "&sdk=store&sdk_v=0.1", form,
				("Authorization", "Bearer " + authorizationJWTtoken),
				(status, message) =>
				{
					if (!CheckForErrors(status, message, null))
					{
						try
						{
							string PS3token = JsonUtility.FromJson<XsollaToken>(message).token;
							Application.OpenURL("https://secure.xsolla.com/paystation2/?access_token=" + PS3token);
						}
						catch (Exception)
						{
						}
					}
				}));
		}

		bool CheckForErrors(bool status, string message, Func<XsollaError, bool> checkError)
		{
			//if it is not a network error
			if (status)
			{
				//try to deserialize mistake
				XsollaError error = DeserializeError(message);
				//if postRequest got an error
				if (error != null && error.http_status_code != null)
				{
					//check for general errors
					bool errorShowStatus = CheckGeneralErrors(error);
					//if it is not a general error check for registration error
					if (!errorShowStatus && checkError != null)
						errorShowStatus = checkError(error);
					//else if it is not a general and not a registration error generate indentified error
					if (!errorShowStatus && OnIdentifiedError != null)
						OnIdentifiedError.Invoke(error);
					return true;
				}

				//else if success
				return false;
			}
			else
			{
				if (OnNetworkError != null)
					OnNetworkError.Invoke();
				return true;
			}
		}

		bool CheckGeneralErrors(XsollaError error)
		{
			switch (error.http_status_code)
			{
				case "409":
					if (OnInvalidProjectSettings != null)
						OnInvalidProjectSettings.Invoke(error);
					break;
				case "404":
					if (OnInvalidProjectSettings != null)
						OnInvalidProjectSettings.Invoke(error);
					break;
				case "422":
					if (OnInvalidData != null)
						OnInvalidData.Invoke(error);
					break;
				default:
					return false;
			}

			return true;
		}

		XsollaError DeserializeError(string recievedMessage)
		{
			XsollaError message = new XsollaError();

			try
			{
				message = JsonUtility.FromJson<XsollaError>(recievedMessage);
				message.extended_message = recievedMessage;
			}
			catch (Exception)
			{
			}

			return message;
		}
	}
}