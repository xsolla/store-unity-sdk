using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla
{
	public static class WebRequest
	{
		public static IEnumerator PostRequest(string url, WWWForm form, (string, string) requestHeader,
			Action<bool, string> callback = null)
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
				callback?.Invoke(false, "");
			}
			else
			{
				string recievedMessage = request.downloadHandler.text;
				callback?.Invoke(true, recievedMessage);
			}
		}

		public static IEnumerator GetRequest(string uri, Action<bool, string> callback = null)
		{
			UnityWebRequest request = UnityWebRequest.Get(uri);

#if UNITY_2018_1_OR_NEWER
			yield return request.SendWebRequest();
#else
			yield return request.Send();
#endif

			if (request.isNetworkError)
			{
				callback?.Invoke(false, "");
			}
			else
			{
				callback?.Invoke(true, request.downloadHandler.text);
			}
		}
	}
}