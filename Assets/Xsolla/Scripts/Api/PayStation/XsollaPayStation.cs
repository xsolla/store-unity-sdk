using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;
using Xsolla.Core;

namespace Xsolla.PayStation
{
	public class XsollaPayStation : MonoSingleton<XsollaPayStation>
	{
		public void RequestToken([NotNull] Action<string> onSuccess, [CanBeNull] Action<Error> onError)
		{
			StartCoroutine(PostRequest(XsollaSettings.PayStationTokenRequestUrl, onSuccess, onError));
		}

		// Temporary placed here since existing utility class can't handle simple non-json string responses properly
		IEnumerator PostRequest(string url, Action<string> onComplete, Action<Error> onError)
		{
			var webRequest = UnityWebRequest.Post(url, new WWWForm());

#if UNITY_2018_1_OR_NEWER
			yield return webRequest.SendWebRequest();
#else
			yield return webRequest.Send();
#endif

			if (webRequest.isNetworkError)
			{
				onError?.Invoke(Error.NetworkError);
			}
			else
			{
				onComplete?.Invoke(webRequest.downloadHandler.text);
			}
		}
	}
}