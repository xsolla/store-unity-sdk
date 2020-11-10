using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoSingleton<WebRequestHelper>
	{
		public void DeleteRequest(string url, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			StartCoroutine(DeleteRequestCor(url, requestHeaders, onComplete, onError, errorsToCheck));
		}

		IEnumerator DeleteRequestCor(string url, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			var webRequest = UnityWebRequest.Delete(url);
			webRequest.downloadHandler = new DownloadHandlerBuffer();

			AttachHeadersToDeleteRequest(webRequest, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private void AttachHeadersToDeleteRequest(UnityWebRequest webRequest, List<WebRequestHeader> requestHeaders)
		{
			if (requestHeaders != null)
				foreach (var header in requestHeaders)
					webRequest.SetRequestHeader(header.Name, header.Value);
		}
	}
}

