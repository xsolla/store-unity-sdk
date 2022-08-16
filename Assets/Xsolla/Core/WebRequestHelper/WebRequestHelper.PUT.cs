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
		public void PutRequest(SdkType sdkType, string url, WebRequestHeader requestHeader, Action onComplete = null, Action<Error> onError = null, ErrorCheckType errorsToCheck = ErrorCheckType.CommonErrors)
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(PutRequestCor(sdkType, url, headers, onComplete, onError, errorsToCheck));
		}
		
		public void PutRequest<T>(SdkType sdkType, string url, T jsonObject, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, ErrorCheckType errorsToCheck = ErrorCheckType.CommonErrors) where T : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeaders);
			StartCoroutine(PutRequestCor(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}

		public void PutRequest<T>(SdkType sdkType, string url, T jsonObject, WebRequestHeader requestHeader, Action onComplete = null, Action<Error> onError = null, ErrorCheckType errorsToCheck = ErrorCheckType.CommonErrors) where T : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(PutRequestCor(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}
		
		public void PutRequest<T, D>(SdkType sdkType, string url, D jsonObject, WebRequestHeader requestHeader, Action<T> onComplete = null, Action<Error> onError = null, ErrorCheckType errorsToCheck = ErrorCheckType.CommonErrors) where D : class where T : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(PutRequestCor(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}
		
		private IEnumerator PutRequestCor(SdkType sdkType, string url, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, ErrorCheckType errorsToCheck = ErrorCheckType.CommonErrors)
		{
			url = AppendAnalyticsToUrl(sdkType, url);

			UnityWebRequest webRequest = new UnityWebRequest(url, "PUT") {
				downloadHandler = new DownloadHandlerBuffer()
			};
			
			AttachHeadersToPutRequest(webRequest, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private IEnumerator PutRequestCor<T>(SdkType sdkType, string url, T jsonObject, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, ErrorCheckType errorsToCheck = ErrorCheckType.CommonErrors) where T : class
		{
			url = AppendAnalyticsToUrl(sdkType, url);

			UnityWebRequest webRequest = new UnityWebRequest(url, "PUT") {
				downloadHandler = new DownloadHandlerBuffer()
			};
			
			AttachBodyToPutRequest(webRequest, jsonObject);
			AttachHeadersToPutRequest(webRequest, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}
		
		private IEnumerator PutRequestCor<T, D>(SdkType sdkType, string url, D jsonObject, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, ErrorCheckType errorsToCheck = ErrorCheckType.CommonErrors) where T : class where D : class
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

