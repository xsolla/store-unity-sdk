using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoSingleton<WebRequestHelper>
	{
		public void PatchRequest<T, D>(SdkType sdkType, string url, D jsonObject, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, ErrorCheckType errorsToCheck = ErrorCheckType.CommonErrors) where T : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeaders);
			StartCoroutine(PatchRequestCor<T, D>(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}

		public void PatchRequest<T, D>(SdkType sdkType, string url, D jsonObject, WebRequestHeader requestHeader, Action<T> onComplete = null, Action<Error> onError = null, ErrorCheckType errorsToCheck = ErrorCheckType.CommonErrors) where T : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(PatchRequestCor<T, D>(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}

		public void PatchRequest<T, D>(SdkType sdkType, string url, D jsonObject, Action<T> onComplete = null, Action<Error> onError = null, ErrorCheckType errorsToCheck = ErrorCheckType.CommonErrors) where T : class
		{
			var headers = GetAnalyticHeaders(sdkType);
			StartCoroutine(PatchRequestCor<T, D>(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}

		IEnumerator PatchRequestCor<T, D>(SdkType sdkType, string url, D jsonObject, List<WebRequestHeader> requestHeaders = null, Action<T> onComplete = null, Action<Error> onError = null, ErrorCheckType errorsToCheck = ErrorCheckType.CommonErrors) where T : class
		{
			url = AppendAnalyticsToUrl(sdkType, url);

			UnityWebRequest webRequest = new UnityWebRequest(url, "PATCH");
			webRequest.downloadHandler = new DownloadHandlerBuffer();
			AttachBodyToRequest(webRequest, jsonObject);
			AttachHeadersToPatchRequest(webRequest, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private void AttachHeadersToPatchRequest(UnityWebRequest webRequest, List<WebRequestHeader> requestHeaders, bool withContentType = true)
		{
			if (withContentType)
			{
				if (requestHeaders != null)
					requestHeaders.Add(WebRequestHeader.ContentTypeHeader());
				else
					requestHeaders = new List<WebRequestHeader>() {WebRequestHeader.ContentTypeHeader()};
			}

			AttachHeadersToRequest(webRequest, requestHeaders);
		}
	}
}
