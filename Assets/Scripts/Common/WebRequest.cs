using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla
{
	public static class WebRequest
	{
		public static IEnumerator PostRequest(string url, WWWForm form, (string, string) requestHeader, Action<bool, string> onComplete = null)
		{
			UnityWebRequest request = UnityWebRequest.Post(url, form);
			request.SetRequestHeader(requestHeader.Item1, requestHeader.Item2);

#if UNITY_2018_1_OR_NEWER
			yield return request.SendWebRequest();
#else
			yield return request.Send();
#endif

			if (request.isNetworkError)
			{
				onComplete?.Invoke(false, string.Empty);
			}
			else
			{
				onComplete?.Invoke(true, request.downloadHandler.text);
			}
		}

		public static IEnumerator GetRequest(string url, Action<bool, string> onComplete = null)
		{
			UnityWebRequest request = UnityWebRequest.Get(url);

#if UNITY_2018_1_OR_NEWER
			yield return request.SendWebRequest();
#else
			yield return request.Send();
#endif

			if (request.isNetworkError)
			{
				onComplete?.Invoke(false, string.Empty);
			}
			else
			{
				onComplete?.Invoke(true, request.downloadHandler.text);
			}
		}
	}
}