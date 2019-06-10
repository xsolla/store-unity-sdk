using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla
{
	public class WebRequestHelper : MonoBehaviour
	{
		static WebRequestHelper _instance;
		
		public static WebRequestHelper Instance
		{
			get
			{
				if (_instance == null)
				{
					Init();
				}

				return _instance;
			}
		}
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		internal static void Init()
		{
			if (ReferenceEquals(_instance, null))
			{
				var instances = FindObjectsOfType<WebRequestHelper>();

				if (instances.Length > 1)
				{
					Debug.LogError(typeof(WebRequestHelper) + " Something went really wrong " +
									" - there should never be more than 1 " + typeof(WebRequestHelper) +
									" Reopening the scene might fix it.");
				}
				else if (instances.Length == 0)
				{
					var singleton = new GameObject {hideFlags = HideFlags.HideAndDontSave};
					_instance = singleton.AddComponent<WebRequestHelper>();
					singleton.name = typeof(WebRequestHelper).ToString();

					DontDestroyOnLoad(singleton);

					Debug.Log("[Singleton] An _instance of " + typeof(WebRequestHelper) +
								" is needed in the scene, so '" + singleton.name +
								"' was created with DontDestroyOnLoad.");
				}
				else
				{
					Debug.Log("[Singleton] Using _instance already created: " + _instance.gameObject.name);
				}
			}
		}

		// Prevent accidental WebRequestHelper instantiation
		WebRequestHelper()
		{
		}
		
		public void PostRequest<T>(string url, WWWForm form, WebRequestHeader requestHeader, Action<T> onComplete = null, Action<XsollaError> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			StartCoroutine(PostRequestCor<T>(url, form, requestHeader, onComplete, onError, errorsToCheck));
		}

		public void GetRequest<T>(string url, Action<T> onComplete = null, Action<XsollaError> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			StartCoroutine(GetRequestCor<T>(url, onComplete, onError, errorsToCheck));
		}

		IEnumerator PostRequestCor<T>(string url, WWWForm form, WebRequestHeader requestHeader, Action<T> onComplete = null, Action<XsollaError> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			var webRequest = UnityWebRequest.Post(url, form);
			
			webRequest.SetRequestHeader(requestHeader.Name, requestHeader.Value);

#if UNITY_2018_1_OR_NEWER
			yield return webRequest.SendWebRequest();
#else
			yield return webRequest.Send();
#endif

			ProcessRequest(webRequest, onComplete, onError, errorsToCheck);
		}

		IEnumerator GetRequestCor<T>(string url, Action<T> onComplete = null, Action<XsollaError> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			var webRequest = UnityWebRequest.Get(url);

#if UNITY_2018_1_OR_NEWER
			yield return webRequest.SendWebRequest();
#else
			yield return webRequest.Send();
#endif

			ProcessRequest(webRequest, onComplete, onError, errorsToCheck);
		}

		void ProcessRequest<T>(UnityWebRequest webRequest, Action<T> onComplete, Action<XsollaError> onError, Dictionary<string, ErrorType> errorsToCheck) where T : class
		{
			if (webRequest.isNetworkError)
			{
				if (onError != null)
				{
					onError(new XsollaError {ErrorType = ErrorType.NetworkError});
				}
			}
			else
			{
				var response = webRequest.downloadHandler.text;

				var error = CheckForErrors(response, errorsToCheck);
				if (error == null)
				{
					var data = ParseUtils.FromJson<T>(response);
					if (data != null)
					{
						if (onComplete != null)
						{
							onComplete(data);
						}
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
			}
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

