using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla.Login
{
	static class WebRequests
	{
		public static IEnumerator PostRequest(string url, WWWForm form, Action<bool, string> callback = null)
		{
			UnityWebRequest request = UnityWebRequest.Post(url, form);

#if UNITY_2018_1_OR_NEWER
			yield return request.SendWebRequest();
#else
            yield return request.Send();
#endif
			if (request.isNetworkError && callback != null)
			{
				callback.Invoke(false, request.error);
			}
			else if (callback != null)
			{
				callback.Invoke(true, request.downloadHandler.text);
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
			if (request.isNetworkError && callback != null)
			{
				callback.Invoke(false, request.error);
			}
			else if (callback != null)
			{
				callback.Invoke(true, request.downloadHandler.text);
			}
		}
	}
}