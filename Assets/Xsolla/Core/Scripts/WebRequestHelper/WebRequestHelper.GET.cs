using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoSingleton<WebRequestHelper>
	{
		public void GetRequest<T>(string url, WebRequestHeader requestHeader = null, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			StartCoroutine(GetRequestCor<T>(url, requestHeader, onComplete, onError, errorsToCheck));
		}

		IEnumerator GetRequestCor<T>(string url, WebRequestHeader requestHeader = null, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			var webRequest = UnityWebRequest.Get(url);

			AddOptionalHeadersTo(webRequest);
			if (requestHeader != null)
			{
				webRequest.SetRequestHeader(requestHeader.Name, requestHeader.Value);
			}

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}
	}
}

