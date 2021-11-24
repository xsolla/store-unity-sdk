using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoSingleton<WebRequestHelper>
	{
		public void DeleteRequest(SdkType sdkType, string url, WebRequestHeader requestHeader, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			var requestHeaders = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(DeleteRequestCor(sdkType, url, requestHeaders, onComplete, onError, errorsToCheck));
		}

		public void DeleteRequest(SdkType sdkType, string url, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			if (requestHeaders != null)
				requestHeaders = AppendAnalyticHeaders(sdkType, requestHeaders.ToArray());
			StartCoroutine(DeleteRequestCor(sdkType, url, requestHeaders, onComplete, onError, errorsToCheck));
		}

		IEnumerator DeleteRequestCor(SdkType sdkType, string url, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			url = AppendAnalyticsToUrl(sdkType, url);

			var webRequest = UnityWebRequest.Delete(url);
			webRequest.downloadHandler = new DownloadHandlerBuffer();
			AttachHeaders(webRequest, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}
	}
}

