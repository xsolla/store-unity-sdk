using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoSingleton<WebRequestHelper>
	{
		public void PutRequest<T>(SdkType sdkType, string url, T jsonObject, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeaders?.ToArray());
			StartCoroutine(PutRequestCor(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}

		public void PutRequest<T>(SdkType sdkType, string url, T jsonObject, WebRequestHeader requestHeader, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(PutRequestCor(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}

		IEnumerator PutRequestCor<T>(SdkType sdkType, string url, T jsonObject, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			url = AppendAnalyticsToUrl(sdkType, url);

			UnityWebRequest webRequest = new UnityWebRequest(url, "PUT") {
				downloadHandler = new DownloadHandlerBuffer()
			};
			
			AttachBodyToPutRequest(webRequest, jsonObject);
			AttachHeadersToPutRequest(webRequest, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private void AttachBodyToPutRequest(UnityWebRequest webRequest, object jsonObject)
		{
			if (jsonObject != null) {
				string jsonData = JsonConvert.SerializeObject(jsonObject).Replace('\n', ' ');
				byte[] body = new UTF8Encoding().GetBytes(jsonData);
				webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(body);
			}
		}

		private void AttachHeadersToPutRequest(UnityWebRequest webRequest, List<WebRequestHeader> requestHeaders)
		{
			if (requestHeaders != null)
				requestHeaders.Add(WebRequestHeader.ContentTypeHeader());
			else
				requestHeaders = new List<WebRequestHeader>() { WebRequestHeader.ContentTypeHeader() };

			AttachHeaders(webRequest, requestHeaders);
		}
	}
}

