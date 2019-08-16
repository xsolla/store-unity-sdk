using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Xsolla.Core;

namespace Xsolla.Login
{
	static class WebRequests
	{
		public static IEnumerator PostRequest(string url, WWWForm form, Action<string> onComplete = null, Action<ErrorDescription> onError = null, Dictionary<string, Error> errorsToCheck = null)
		{
			UnityWebRequest request = UnityWebRequest.Post(url, form);
			AddOptionalHeaders(request);

#if UNITY_2018_1_OR_NEWER
			yield return request.SendWebRequest();
#else
            yield return request.Send();
#endif
			if (request.isNetworkError)
			{
				TriggerOnError(onError, ErrorDescription.NetworkError);
			}
			else
			{
				var response = request.downloadHandler.text;
				Debug.Log(response);
				var error = CheckForErrors(response, errorsToCheck);
				if (error == null)
				{
					if (onComplete != null)
					{
						onComplete.Invoke(request.downloadHandler.text);
					}
				}
				else
				{
					TriggerOnError(onError, error);
				}
			}
		}
		public static IEnumerator PostRequest(string url, string jsonData, Action<string> onComplete = null, Action<ErrorDescription> onError = null, Dictionary<string, Error> errorsToCheck = null)
		{
			var request = new UnityWebRequest(url, "POST");
			AddOptionalHeaders(request);
	        
			request.downloadHandler = new DownloadHandlerBuffer();
			
			if (!string.IsNullOrEmpty(jsonData))
			{
				request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
			}
	        
			request.SetRequestHeader(WebRequestHeader.ContentTypeHeader().Name, WebRequestHeader.ContentTypeHeader().Value);

#if UNITY_2018_1_OR_NEWER
			yield return request.SendWebRequest();
#else
            yield return request.Send();
#endif
			if (request.isNetworkError)
			{
				TriggerOnError(onError, ErrorDescription.NetworkError);
			}
			else
			{
				var response = request.downloadHandler.text;
				Debug.Log(response);
				var error = CheckForErrors(response, errorsToCheck);
				if (error == null)
				{
					if (onComplete != null)
					{
						onComplete.Invoke(request.downloadHandler.text);
					}
				}
				else
				{
					TriggerOnError(onError, error);
				}
			}
		}

		public static IEnumerator PostRequest(string url, WWWForm form, Action onComplete = null, Action<ErrorDescription> onError = null, Dictionary<string, Error> errorsToCheck = null)
		{
			UnityWebRequest request = UnityWebRequest.Post(url, form);
			AddOptionalHeaders(request);

#if UNITY_2018_1_OR_NEWER
			yield return request.SendWebRequest();
#else
            yield return request.Send();
#endif
			if (request.isNetworkError)
			{
				TriggerOnError(onError, ErrorDescription.NetworkError);
			}
			else
			{
				var response = request.downloadHandler.text;
				Debug.Log(response);
				var error = CheckForErrors(response, errorsToCheck);
				if (error == null)
				{
					if (onComplete != null)
					{
						onComplete.Invoke();
					}
				}
				else
				{
					TriggerOnError(onError, error);
				}
			}
		}
		
		public static IEnumerator PostRequest(string url, string jsonData, Action onComplete = null, Action<ErrorDescription> onError = null, Dictionary<string, Error> errorsToCheck = null)
		{
			var request = new UnityWebRequest(url, "POST");
			AddOptionalHeaders(request);
	        
			request.downloadHandler = new DownloadHandlerBuffer();
			
			if (!string.IsNullOrEmpty(jsonData))
			{
				request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
			}
	        
			request.SetRequestHeader(WebRequestHeader.ContentTypeHeader().Name, WebRequestHeader.ContentTypeHeader().Value);

#if UNITY_2018_1_OR_NEWER
			yield return request.SendWebRequest();
#else
            yield return request.Send();
#endif
			if (request.isNetworkError)
			{
				TriggerOnError(onError, ErrorDescription.NetworkError);
			}
			else
			{
				var response = request.downloadHandler.text;
				Debug.Log(response);
				var error = CheckForErrors(response, errorsToCheck);
				if (error == null)
				{
					if (onComplete != null)
					{
						onComplete.Invoke();
					}
				}
				else
				{
					TriggerOnError(onError, error);
				}
			}
		}
		
		public static ErrorDescription CheckForErrors(string json, Dictionary<string, Error> errorsToCheck)
		{
			if (string.IsNullOrEmpty(json))
			{
				return null;
			}
			
			var error = ParseUtils.FromJson<MessageJson>(json).error;
			
			if (error != null && !string.IsNullOrEmpty(error.code))
			{
				if (errorsToCheck != null && errorsToCheck.ContainsKey(error.code))
				{
					error.error = errorsToCheck[error.code];
					return error;
				}

				if (ErrorDescription.GeneralErrors.ContainsKey(error.code))
				{
					error.error = ErrorDescription.GeneralErrors[error.code];
					return error;
				}

				return ErrorDescription.UnknownError;
			}

			return null;
		}
		
		static void TriggerOnError(Action<ErrorDescription> onError, ErrorDescription error)
		{
			if (onError != null)
			{
				onError(error);
			}
		}
		
		public static void AddOptionalHeaders(UnityWebRequest request)
		{
			request.SetRequestHeader("X-ENGINE", "UNITY");
			request.SetRequestHeader("X-ENGINE_V", Application.unityVersion.ToUpper());
			request.SetRequestHeader("X-SDK", "LOGIN");
			request.SetRequestHeader("X-SDK_V", Constants.LoginSdkVersion);
		}
	}
}