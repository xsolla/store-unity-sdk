using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoSingleton<WebRequestHelper>
	{
		public void GetRequest<T>(string url, List<WebRequestHeader> requestHeaders = null, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			StartCoroutine(GetRequestCor<T>(url, requestHeaders, onComplete, onError, errorsToCheck));
		}

		IEnumerator GetRequestCor<T>(string url, List<WebRequestHeader> requestHeaders = null, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			var webRequest = UnityWebRequest.Get(url);

			if (requestHeaders != null)
				foreach (var header in requestHeaders)
					webRequest.SetRequestHeader(header.Name, header.Value);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}
	}
}

