using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla
{
	public static class WebRequest
	{
		public static IEnumerator PostRequest(string url, WWWForm form, WebRequestHeader requestHeader, Action<string> onComplete = null, Action<XsollaError> onError = null)
		{
			var webRequest = UnityWebRequest.Post(url, form);
			
			webRequest.SetRequestHeader(requestHeader.Name, requestHeader.Value);

#if UNITY_2018_1_OR_NEWER
			yield return webRequest.SendWebRequest();
#else
			yield return webRequest.Send();
#endif

			if (webRequest.isNetworkError)
			{
				if (onError != null)
				{
					onError(new XsollaError {ErrorType = ErrorType.NetworkError});
				}
			}
			else
			{
				if (onComplete != null)
				{
					onComplete(webRequest.downloadHandler.text);
				}
			}
		}

		public static IEnumerator GetRequest(string url, Action<string> onComplete = null, Action<XsollaError> onError = null)
		{
			var webRequest = UnityWebRequest.Get(url);

#if UNITY_2018_1_OR_NEWER
			yield return webRequest.SendWebRequest();
#else
			yield return webRequest.Send();
#endif

			if (webRequest.isNetworkError)
			{
				if (onError != null)
				{
					onError(new XsollaError {ErrorType = ErrorType.NetworkError});
				}
			}
			else
			{
				if (onComplete != null)
				{
					onComplete(webRequest.downloadHandler.text);
				}
			}
		}
	}
}