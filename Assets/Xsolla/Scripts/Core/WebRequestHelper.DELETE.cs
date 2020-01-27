using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoBehaviour
	{
		public void DeleteRequest(string url, WebRequestHeader requestHeader, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			StartCoroutine(DeleteRequestCor(url, requestHeader, onComplete, onError, errorsToCheck));
		}
		
		IEnumerator DeleteRequestCor(string url, WebRequestHeader requestHeader, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			var webRequest = UnityWebRequest.Delete(url);
			webRequest.downloadHandler = new DownloadHandlerBuffer();

			AttachHeadersToDeleteRequest(webRequest, requestHeader);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private void AttachHeadersToDeleteRequest(UnityWebRequest webRequest, WebRequestHeader requestHeader)
		{
			AddOptionalHeadersTo(webRequest);

			if (requestHeader != null) {
				webRequest.SetRequestHeader(requestHeader.Name, requestHeader.Value);
			}
		}
	}
}

