using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla
{
	public class XsollaPayStation : MonoSingleton<XsollaPayStation>
	{
		[SerializeField]
		string serverUrl;
		
		void OpenPurchaseUi(string xsollaToken)
		{
			Application.OpenURL("https://secure.xsolla.com/paystation3/?access_token=" + xsollaToken);
		}

		public void OpenPayStation()
		{
			StartCoroutine(PostRequest(serverUrl, OpenPurchaseUi, () => print("Error occured!")));
		}

		// This is temporary solution
		IEnumerator PostRequest(string url, Action<string> onComplete, Action onError)
		{
			var webRequest = UnityWebRequest.Post(url, new WWWForm());

#if UNITY_2018_1_OR_NEWER
			yield return webRequest.SendWebRequest();
#else
			yield return webRequest.Send();
#endif

			if (webRequest.isNetworkError)
			{
				onError();
			}
			else
			{
				onComplete(webRequest.downloadHandler.text);
			}
		}
	}
}
