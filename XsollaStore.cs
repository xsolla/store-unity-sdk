using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla
{
    public class XsollaStore : MonoBehaviour
    {
        #region Events 
        public event Action<string> OnSuccesfullGetToken;
        public event Action<string> OnCantParseToken;
        public event Action<StoreItems> OnSuccessGetListOfItems;
        public event Action<GroupItems> OnSuccessGetListOfGroups;
        public event Action<GroupItemInformation> OnSuccessGetGroupInformation;
        public event Action<ItemInformation> OnSuccessGetItemInformation;
        #region General
        //409 
        public event Action<ErrorDescription> OnInvalidProjectSettings;
        //422 
        public event Action<ErrorDescription> OnInvalidData;
        #endregion

        public event Action<ErrorDescription> OnIdentifiedError;
        public event Action OnNetworkError;
        #endregion

        public string Merchant_Id
        {
            get
            {
                return _merchant_id;
            }
            set
            {
                _merchant_id = value;
            }
        }

        public string Api_Key
        {
            get
            {
                return _api_key;
            }
            set
            {
                _api_key = value;
            }
        }
        public string Project_Id
        {
            get
            {
                return _project_id;
            }
            set
            {
                _project_id = value;
            }
        }
        public string Token
        {
            get
            {
                return PlayerPrefs.HasKey("Xsolla_Store_Token") ? PlayerPrefs.GetString("Xsolla_Store_Token") : "";
            }
        }

        [SerializeField]
        private string _project_id;
        [SerializeField]
        private string _merchant_id;
        [SerializeField]
        private string _api_key;

        public static XsollaStore Instance = null;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance == this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }
        /// <summary>
        /// Get item information by item id.
        /// </summary>
        public void GetItemInformation(string item_id)
        {
            StartCoroutine(GetRequest("https://api.xsolla.com/merchant/v2/projects/" + _project_id + "/virtual_items/items/" + item_id,
                            (status, message) =>
                            {
                                Debug.Log("Recieved message from GetItemInformation: " + message);
                                if (!CheckForErrors(status, message, null))
                                {
                                    ItemInformation item = new ItemInformation();
                                    try
                                    {
                                        item = JsonUtility.FromJson<ItemInformation>(message);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                    if (OnSuccessGetItemInformation != null)
                                        OnSuccessGetItemInformation.Invoke(item);
                                }
                            }));
        }
        /// <summary>
        /// Get group information by group id.
        /// </summary>
        public void GetGroupInformation(string group_id)
        {
            StartCoroutine(GetRequest("https://api.xsolla.com/merchant/v2/projects/" + _project_id + "/virtual_items/groups/" + group_id,
                            (status, message) =>
                            {
                                Debug.Log("Recieved message from GetGroupInformation: " + message);
                                if (!CheckForErrors(status, message, null))
                                {
                                    GroupItemInformation item = new GroupItemInformation();
                                    try
                                    {
                                        item = JsonUtility.FromJson<GroupItemInformation>(message);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                    if (OnSuccessGetGroupInformation != null)
                                        OnSuccessGetGroupInformation.Invoke(item);
                                }
                            }));
        }

        /// <summary>
        /// Get list of groups.
        /// </summary>
        public void GetListOfGroups()
        {
            StartCoroutine(GetRequest("https://api.xsolla.com/merchant/v2/projects/" + _project_id + "/virtual_items/groups",
                            (status, message) =>
                            {
                                Debug.Log("Recieved message from GetListOfGroups: " + message);
                                if (!CheckForErrors(status, message, null))
                                {
                                    GroupItems items = new GroupItems();
                                    try
                                    {
                                        message = message.Insert(message.Length, "}").Insert(0, "{\"groupItems\":");
                                        items = JsonUtility.FromJson<GroupItems>(message);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                    if (OnSuccessGetListOfGroups != null)
                                        OnSuccessGetListOfGroups.Invoke(items);
                                }
                            }));
        }

        /// <summary>
        /// Get list of items.
        /// </summary>
        public void GetListOfItems()
        {
            StartCoroutine(GetRequest("https://api.xsolla.com/merchant/v2/projects/" + _project_id + "/virtual_items/items",
                            (status, message) =>
                            {
                                Debug.Log("Recieved message from GetListOfItems: " + message);
                                if (!CheckForErrors(status, message, null))
                                {
                                    StoreItems items = new StoreItems();
                                    try
                                    {
                                        message = message.Insert(message.Length, "}").Insert(0, "{\"storeItems\":");
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
        /// You can create a token with arbitrary user parameters.
        /// You send these parameters when obtaining the token, and we send them back to you after a successful payment.
        /// A token is valid for 24 hours.
        /// </summary>
        public void GetToken(TokenInformation tokenInformation)
        {
            if (_project_id == "" || _merchant_id == "" || _api_key == "")
            {
                Debug.Log("INVALID SETTINGS");
                if (OnInvalidProjectSettings != null)
                    OnInvalidProjectSettings.Invoke(new ErrorDescription());
                return;
            }

            // V1 ???????
            StartCoroutine(GetTokenRequest("https://api.xsolla.com/merchant/v2/merchants/" + _merchant_id + "/token",
                                            JsonUtility.ToJson(tokenInformation),
                                            (status, message) =>
                                            {
                                                Debug.Log("Recieved message from GetToken: " + message);
                                                if (!CheckForErrors(status, message, null))
                                                {
                                                    try
                                                    {
                                                    //try to parse token
                                                    string token = JsonUtility.FromJson<JsonToken>(message).token;
                                                        PlayerPrefs.SetString("Xsolla_Store_Token", token);
                                                        if (OnSuccesfullGetToken != null)
                                                            OnSuccesfullGetToken.Invoke(token);
                                                    }
                                                    catch (Exception)
                                                    {
                                                    //if exception while parse token
                                                    if (OnCantParseToken != null)
                                                            OnCantParseToken.Invoke(message);
                                                    }
                                                }
                                            }));
        }

        #region Exceptions

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
            catch (Exception) { }
            return message;
        }
        #endregion

        #region WebRequest
        private IEnumerator PostRequest(string url, WWWForm form, Action<bool, string> callback = null)
        {
            UnityWebRequest request = UnityWebRequest.Post(url, form);

#if UNITY_2018_1_OR_NEWER
            request.SendWebRequest();
#else
            request.Send();
#endif

            while (!request.isDone)
            {
                //wait 
                yield return new WaitForEndOfFrame();
            }
            if (request.isNetworkError)
            {
                callback?.Invoke(false, "");
            }
            else
            {
                string recievedMessage = request.downloadHandler.text;
                callback?.Invoke(true, recievedMessage);
            }
        }

        IEnumerator GetTokenRequest(string url, string bodyJsonString, Action<bool, string> callback = null)
        {
            var request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(_merchant_id + ":" + _api_key)));
            request.SetRequestHeader("Content-Type", "application/json");

#if UNITY_2018_1_OR_NEWER
            request.SendWebRequest();
#else
            request.Send();
#endif

            while (!request.isDone)
            {
                //wait 
                yield return new WaitForEndOfFrame();
            }
            if (request.isNetworkError)
            {
                callback?.Invoke(false, "");
            }
            else
            {
                string recievedMessage = request.downloadHandler.text;
                callback?.Invoke(true, recievedMessage);
            }
        }

        private IEnumerator GetRequest(string uri, Action<bool, string> callback = null)
        {
            UnityWebRequest request = UnityWebRequest.Get(uri);
            request.SetRequestHeader("authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(_merchant_id + ":" + _api_key)));

#if UNITY_2018_1_OR_NEWER
            request.SendWebRequest();
#else
            request.Send();
#endif

            while (!request.isDone)
            {
                //wait 
                yield return new WaitForEndOfFrame();
            }
            if (request.isNetworkError)
            {
                callback?.Invoke(false, "");
            }
            else
            {
                callback?.Invoke(true, request.downloadHandler.text);
            }
        }
        #endregion

        #region JSONclasses
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
        public class GroupItems
        {
            public GroupItem[] groupItems;
        }
        [Serializable]
        public class ItemInformation
        {
            public string advertisement_type;
            public string default_currency;
            public bool deleted;
            public Languages description;
            public bool enabled;
            public string expiration;
            public string[] groups;
            public string id;
            public string image_url;
            public string item_code;
            public string item_type;
            public string[] keywords;
            public Languages long_description;
            public Languages name;
            public bool permanent;
            public StoreItemPrices prices;
            public string purchase_limit;
            public string[] secondary_market;
            public string sku;
            //public string[] user_attribute_conditions;
            public string virtual_currency_price;
        }
        [Serializable]
        public class GroupItemInformation
        {
            public int code;
            public Languages description;
            public Languages name;
            public string id;
            public string parent_id;
            public bool enabled;
        }
        [Serializable]
        public class Languages
        {
            public string en;
            public string ru;
        }
        [Serializable]
        public class GroupItem
        {
            public string id;
            public string parent_id;
            public string localized_name;
            public string has_groups;
            public string has_virtual_items;
            public int virtual_items_count;
            public bool enabled;
            public string code;
        }
        [Serializable]
        public class StoreItems
        {
            public StoreItem[] storeItems;
        }
        [Serializable]
        public class StoreItem
        {
            public string id;
            public string sku;
            public string localized_name;
            public StoreItemPrices prices;
            public string virtual_currency_price;
            public string default_currency;
            public bool enabled;
            public bool permanent;
            public string[] groups;
            public string advertisement_type;
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


        #region TokenInformation
        [Serializable]
        public struct TokenInformation
        {
            /// <summary>
            /// User details.
            /// </summary>
            public User user;
            //public Settings settings;
            public Purchase purchase;
        }
        [Serializable]
        public struct User
        {
            /// <summary>
            /// User ID. Required.
            /// </summary>
            public UserId id;
            public UserName name;
            public UserEmail email;
            public UserPhone phone;
            public UserCountry country;
        }
        [Serializable]
        public struct UserId
        {
            /// <summary>
            /// User ID. Required.
            /// </summary>
            public string value;
        }
        [Serializable]
        public struct UserName
        {
            /// <summary>
            /// User screen name
            /// </summary>
            public string value;
        }
        [Serializable]
        public struct UserEmail
        {
            /// <summary>
            /// User email. Must be valid according to the RFC 822 protocol.
            /// </summary>
            public string value;
        }
        [Serializable]
        public struct UserPhone
        {
            /// <summary>
            /// User phone number
            /// </summary>
            public string value;
        }
        [Serializable]
        public struct UserCountry
        {
            /// <summary>
            /// Two-letter uppercase country code per ISO 3166-1 alpha-2.
            /// </summary>
            public string value;
            /// <summary>
            /// Whether or not user can change the country on payment UI. 'false' by default.
            /// </summary>
            public bool allow_modify;
        }
        //[Serializable]
        //public struct Settings
        //{
        //    /// <summary>
        //    /// Game’s Xsolla ID. Can be found in Publisher Account. Required.
        //    /// </summary>
        //    public int project_id;
        //    /// <summary>
        //    /// Interface language. Two-letter lowercase language code per ISO 639-1.
        //    /// </summary>
        //    public string language;
        //    /// <summary>
        //    /// Preferred payment currency. Three-letter currency code per ISO 4217.
        //    /// </summary>
        //    public string currency;
        //    /// <summary>
        //    /// Set to 'sandbox' to test out the payment process. In this case, use https://sandbox-secure.xsolla.com to access the test payment UI.
        //    /// </summary>
        //    public string mode;
        //    /// <summary>
        //    /// Payment method ID.
        //    /// </summary>
        //    //public int payment_method;
        //}
        [Serializable]
        public struct Purchase
        {
            /// <summary>
            /// Object containing virtual currency details.
            /// </summary>
            //public VirtualCurrency virtual_currency;
            /// <summary>
            /// Object with data about the virtual items in purchase.
            /// </summary>
            //public VirtualItems virtual_items;
            /// <summary>
            /// Subscription data.
            /// </summary>
            //public Subscription subscription;
            /// <summary>
            ///Gift details.
            /// </summary>
            //public Gift gift;
        }
        //[Serializable]
        //public struct VirtualCurrency
        //{
        //    /// <summary>
        //    /// Purchase amount in the virtual currency.
        //    /// </summary>
        //    public float quantity;
        //    /// <summary>
        //    /// Currency of the virtual currency package to use in all calculations.
        //    /// </summary>
        //    public string currency;
        //}
        //[Serializable]
        //public struct VirtualItems
        //{
        //    /// <summary>
        //    /// Item data.
        //    /// </summary>
        //    public VirtualItems_Items[] items;
        //    /// <summary>
        //    /// Currency of the ordered items to use in all calculations.
        //    /// </summary>
        //    public string currency;
        //    /// <summary>
        //    /// Currency of the ordered items to use in all calculations.
        //    /// </summary>
        //    public string[] available_groups;
        //}
        //[Serializable]
        //public struct VirtualItems_Items
        //{
        //    /// <summary>
        //    /// Item ID.
        //    /// </summary>
        //    public string sku;
        //    /// <summary>
        //    /// Item quantity.
        //    /// </summary>
        //    public int amount;
        //}
        //[Serializable]
        //public struct Subscription
        //{
        //    /// <summary>
        //    /// Plan ID.
        //    /// </summary>
        //    public string plan_id;
        //    /// <summary>
        //    /// The type of operation applied to the user’s subscription plan.
        //    /// To change the subscription plan, pass the ‘change_plan’ value.
        //    /// You need to specify the new plan ID in the purchase.subscription.plan_id parameter.
        //    /// </summary>
        //    public string operation;
        //    /// <summary>
        //    /// Product ID.
        //    /// </summary>
        //    public string product_id;
        //    /// <summary>
        //    /// Currency of the subscription plan to use in all calculations.
        //    /// </summary>
        //    public string currency;
        //    /// <summary>
        //    /// Trial period in days.
        //    /// </summary>
        //    public int trial_days;
        //}
        //[Serializable]
        //public struct Gift
        //{
        //    /// <summary>
        //    /// Giver ID.
        //    /// </summary>
        //    public string giver_id;
        //    /// <summary>
        //    /// Message from the giver.
        //    /// </summary>
        //    public string message;
        //    /// <summary>
        //    /// Whether to hide the giver identity from the recipient. 'true' by default.
        //    /// </summary>
        //    public bool hide_giver_from_receiver;
        //    /// <summary>
        //    /// Array with data on friends.
        //    /// </summary>
        //    public GiftFriend friends;
        //}
        //[Serializable]
        //public struct GiftFriend
        //{
        //    /// <summary>
        //    /// Gift recipient ID.
        //    /// </summary>
        //    public string id;
        //    /// <summary>
        //    /// Gift recipient nickname.
        //    /// </summary>
        //    public string name;
        //    /// <summary>
        //    /// Gift recipient email.
        //    /// </summary>
        //    public bool email;
        //}
        #endregion


        #endregion
    }
}