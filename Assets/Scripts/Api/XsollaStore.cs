using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla
{
	public class XsollaStore : MonoSingleton<XsollaStore>
	{
		public event Action<StoreItems> OnSuccessGetListOfItems;

		//409 
		public event Action<ErrorDescription> OnInvalidProjectSettings;

		//422 
		public event Action<ErrorDescription> OnInvalidData;

		public event Action<ErrorDescription> OnIdentifiedError;
		public event Action OnNetworkError;

		public string Project_Id
		{
			get { return _project_id; }
			set { _project_id = value; }
		}

		public string Token
		{
			set { PlayerPrefs.SetString("Xsolla_Store_Token", value); }
			get { return PlayerPrefs.HasKey("Xsolla_Store_Token") ? PlayerPrefs.GetString("Xsolla_Store_Token") : ""; }
		}

		public string CurrencySymbol
		{
			get { return Region.GetCurrencySymbol(); }
		}

		[SerializeField] private string _project_id;

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
					//Debug.Log("Recieved message from GetListOfItems: " + message);
					if (!CheckForErrors(status, message, null))
					{
						StoreItems items = new StoreItems();
						try
						{
							message = message.Remove(0, 8).Insert(0, "{\"storeItems\"");
							items = JsonUtility.FromJson<StoreItems>(message);
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
					//Debug.Log("Recieved message from GetListOfItems: " + message);
					if (!CheckForErrors(status, message, null))
					{
						try
						{
							string PS3token = JsonUtility.FromJson<JsonToken>(message).token;
							Application.OpenURL("https://secure.xsolla.com/paystation2/?access_token=" + PS3token);
						}
						catch (Exception)
						{
						}
					}
				}));
		}

		private bool CheckForErrors(bool status, string message, Func<ErrorDescription, bool> checkError)
		{
			//if it is not a network error
			if (status)
			{
				//try to deserialize mistake
				ErrorDescription errorJson = DeserializeError(message);
				bool errorShowStatus = false;
				//if postRequest got an error
				if (errorJson != null && errorJson.http_status_code != null)
				{
					//check for general errors
					errorShowStatus = CheckGeneralErrors(errorJson);
					//if it is not a general error check for registration error
					if (!errorShowStatus && checkError != null)
						errorShowStatus = checkError(errorJson);
					//else if it is not a general and not a registration error generate indentified error
					if (!errorShowStatus && OnIdentifiedError != null)
						OnIdentifiedError.Invoke(errorJson);
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

		private bool CheckGeneralErrors(ErrorDescription errorDescription)
		{
			switch (errorDescription.http_status_code)
			{
				case "409":
					if (OnInvalidProjectSettings != null)
						OnInvalidProjectSettings.Invoke(errorDescription);
					break;
				case "404":
					if (OnInvalidProjectSettings != null)
						OnInvalidProjectSettings.Invoke(errorDescription);
					break;
				case "422":
					if (OnInvalidData != null)
						OnInvalidData.Invoke(errorDescription);
					break;
				default:
					return false;
			}

			return true;
		}

		private ErrorDescription DeserializeError(string recievedMessage)
		{
			ErrorDescription message = new ErrorDescription();

			try
			{
				message = JsonUtility.FromJson<ErrorDescription>(recievedMessage);
				message.extended_message = recievedMessage;
			}
			catch (Exception)
			{
			}

			return message;
		}



		private static class Region
		{
			private static readonly Dictionary<SystemLanguage, LanguageProperties> currencyCodes =
				new Dictionary<SystemLanguage, LanguageProperties>()
				{
					{SystemLanguage.Chinese, new LanguageProperties("元", "CNY")},
					{SystemLanguage.English, new LanguageProperties("$", "USD")},
					{SystemLanguage.French, new LanguageProperties("€", "EUR")},
					{SystemLanguage.German, new LanguageProperties("€", "EUR")},
					{SystemLanguage.Korean, new LanguageProperties("₩", "KRW")},
					{SystemLanguage.Portuguese, new LanguageProperties("R$", "BRL")},
					{SystemLanguage.Russian, new LanguageProperties("₽", "RUB")},
					{SystemLanguage.Spanish, new LanguageProperties("€", "EUR")},
					{SystemLanguage.Unknown, new LanguageProperties("$", "USD")}
				};

			public static string GetCurrencyCode()
			{
				if (currencyCodes.ContainsKey(Application.systemLanguage))
				{
					return currencyCodes[Application.systemLanguage].Code;
				}
				else
				{
					return currencyCodes[SystemLanguage.Unknown].Code;
				}
			}

			public static string GetCurrencySymbol()
			{
				if (currencyCodes.ContainsKey(Application.systemLanguage))
				{
					return currencyCodes[Application.systemLanguage].Symbol;
				}
				else
				{
					return currencyCodes[SystemLanguage.Unknown].Symbol;
				}
			}

			private class LanguageProperties
			{
				public string Symbol;
				public string Code;

				public LanguageProperties(string symbol, string code)
				{
					Symbol = symbol;
					Code = code;
				}
			}
		}

		[Serializable]
		public class ErrorDescription
		{
			public string http_status_code;
			public string message;
			public string request_id;
			public string extended_message;
		}

		[Serializable]
		public struct JsonToken
		{
			public string token;
		}

		[Serializable]
		public class StoreItems
		{
			public StoreItem[] storeItems;
		}

		[Serializable]
		public class StoreItem
		{
			public string sku;
			public string[] groups;
			public string name;
			public string type;
			public bool is_free;
			public string long_description;
			public string description;
			public string image_url;
			public ItemPrices[] prices;
		}

		[Serializable]
		public struct ItemPrices
		{
			public float amount;
			public string currency;
		}

		[Serializable]
		public struct StoreItemPrices
		{
			public float EUR;
			public float USD;
			public float AED;
			public float AFN;
			public float ALL;
			public float AMD;
			public float ARS;
			public float AUD;
			public float AZN;
			public float BAM;
			public float BBD;
			public float BGN;
			public float BHD;
			public float BND;
			public float BOB;
			public float BRL;
			public float BSD;
			public float BTC;
			public float BWP;
			public float BYN;
			public float BZD;
			public float CAD;
			public float CHF;
			public float CLP;
			public float CNY;
			public float COP;
			public float CRC;
			public float CZK;
			public float DKK;
			public float DOP;
			public float DZD;
			public float EGP;
			public float ETH;
			public float FJD;
			public float FKP;
			public float GBP;
			public float GEL;
			public float GHS;
			public float GIP;
			public float GTQ;
			public float GYD;
			public float HKD;
			public float HNL;
			public float HRK;
			public float HUF;
			public float IDR;
			public float ILS;
			public float INR;
			public float IQD;
			public float IRR;
			public float ISK;
			public float JMD;
			public float JOD;
			public float JPY;
			public float KES;
			public float KGS;
			public float KMF;
			public float KRW;
			public float KWD;
			public float KZT;
			public float LAK;
			public float LBP;
			public float LKR;
			public float LRD;
			public float LTC;
			public float MAD;
			public float MDL;
			public float MGO;
			public float MKD;
			public float MMK;
			public float MNT;
			public float MUR;
			public float MXN;
			public float MYR;
			public float MZN;
			public float NAD;
			public float NGN;
			public float NIO;
			public float NOK;
			public float NZD;
			public float OMR;
			public float PAB;
			public float PEN;
			public float PHP;
			public float PKR;
			public float PLN;
			public float PYG;
			public float QAR;
			public float RON;
			public float RSD;
			public float RUB;
			public float SAR;
			public float SDG;
			public float SEK;
			public float SGD;
			public float SOS;
			public float SRD;
			public float SVC;
			public float SYP;
			public float THB;
			public float TJS;
			public float TMT;
			public float TND;
			public float TRY;
			public float TTD;
			public float TWD;
			public float UAH;
			public float UGX;
			public float UYU;
			public float UZS;
			public float VEF;
			public float VES;
			public float VND;
			public float XCD;
			public float XOF;
			public float YER;
			public float ZAR;
			public float ZWD;
		}
	}
}