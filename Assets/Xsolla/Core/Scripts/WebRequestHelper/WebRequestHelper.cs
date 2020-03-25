using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoSingleton<WebRequestHelper>
	{
		private IEnumerator PerformWebRequest(UnityWebRequest webRequest, Action onComplete, Action<Error> onError, Dictionary<string, ErrorType> errorsToCheck)
		{
			yield return StartCoroutine(SendWebRequest(webRequest));

			ProcessRequest(webRequest, onComplete, onError, errorsToCheck);
		}

		private IEnumerator PerformWebRequest(UnityWebRequest webRequest, Action<string> onComplete, Action<Error> onError, Dictionary<string, ErrorType> errorsToCheck)
		{
			yield return StartCoroutine(SendWebRequest(webRequest));

			ProcessRequest(webRequest, onComplete, onError, errorsToCheck);
		}

		private IEnumerator PerformWebRequest<T>(UnityWebRequest webRequest, Action<T> onComplete, Action<Error> onError, Dictionary<string, ErrorType> errorsToCheck) where T : class
		{
			yield return StartCoroutine(SendWebRequest(webRequest));

			ProcessRequest(webRequest, onComplete, onError, errorsToCheck);
		}

		private IEnumerator SendWebRequest(UnityWebRequest webRequest)
		{
#if UNITY_2018_1_OR_NEWER
			yield return webRequest.SendWebRequest();
#else
			yield return webRequest.Send();
#endif
		}
	}
}

