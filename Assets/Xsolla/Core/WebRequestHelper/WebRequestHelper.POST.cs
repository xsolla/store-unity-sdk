using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoSingleton<WebRequestHelper>
	{
		#region PostRequest<T,D>
		public void PostRequest<T, D>(SdkType sdkType, string url, D jsonObject, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
			where T : class
			where D : class
		{
			List<WebRequestHeader> headers = null;
			if (requestHeaders != null)
				headers = AppendAnalyticHeaders(sdkType, requestHeaders.ToArray());
			StartCoroutine(PostRequestCor<T>(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}

		public void PostRequest<T, D>(SdkType sdkType, string url, D jsonObject, [NotNull]WebRequestHeader requestHeader, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
			where T : class
			where D : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(PostRequestCor<T>(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}

		public void PostRequest<T, D>(SdkType sdkType, string url, D jsonObject, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
			where T : class
			where D : class
		{
			var headers = GetAnalyticHeaders(sdkType);
			StartCoroutine(PostRequestCor<T>(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}
		#endregion

		#region PostRequest<T>
		public void PostRequest<T>(SdkType sdkType, string url, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
			where T : class
		{
			List<WebRequestHeader> headers = null;
			if (requestHeaders != null)
				headers = AppendAnalyticHeaders(sdkType, requestHeaders.ToArray());
			StartCoroutine(PostRequestCor<T>(sdkType, url, null, headers, onComplete, onError, errorsToCheck));
		}

		public void PostRequest<T>(SdkType sdkType, string url, WebRequestHeader requestHeader, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
			where T : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(PostRequestCor<T>(sdkType, url, null, headers, onComplete, onError, errorsToCheck));
		}

		public void PostRequest<T>(SdkType sdkType, string url, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
			where T : class
		{
			var headers = GetAnalyticHeaders(sdkType);
			StartCoroutine(PostRequestCor<T>(sdkType, url, null, headers, onComplete, onError, errorsToCheck));
		}
		#endregion

		#region PostRequest<D> with jsonObject
		public void PostRequest<D>(SdkType sdkType, string url, D jsonObject, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
			where D : class
		{
			List<WebRequestHeader> headers = null;
			if (requestHeaders != null)
				headers = AppendAnalyticHeaders(sdkType, requestHeaders.ToArray());
			StartCoroutine(PostRequestCor(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}

		public void PostRequest<D>(SdkType sdkType, string url, D jsonObject, WebRequestHeader requestHeader, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
			where D : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(PostRequestCor(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}

		public void PostRequest<D>(SdkType sdkType, string url, D jsonObject = null, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
			where D : class
		{
			var headers = GetAnalyticHeaders(sdkType);
			StartCoroutine(PostRequestCor(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}
		#endregion

		#region PostRequest
		public void PostRequest(SdkType sdkType, string url, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			List<WebRequestHeader> headers = null;
			if (requestHeaders != null)
				headers = AppendAnalyticHeaders(sdkType, requestHeaders.ToArray());
			StartCoroutine(PostRequestCor(sdkType, url, null, headers, onComplete, onError, errorsToCheck));
		}

		public void PostRequest(SdkType sdkType, string url, WebRequestHeader requestHeader, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(PostRequestCor(sdkType, url, null, headers, onComplete, onError, errorsToCheck));
		}

		public void PostRequest(SdkType sdkType, string url, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			var headers = GetAnalyticHeaders(sdkType);
			StartCoroutine(PostRequestCor(sdkType, url, null, headers, onComplete, onError, errorsToCheck));
		}
		#endregion

		#region PostRquest<T> with WWWForm
		public void PostRequest<T>(SdkType sdkType, string url, WWWForm data, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
			where T : class
		{
			List<WebRequestHeader> headers = null;
			if (requestHeaders != null)
				headers = AppendAnalyticHeaders(sdkType, requestHeaders.ToArray());
			StartCoroutine(PostRequestCor<T>(sdkType, url, data, headers, onComplete, onError, errorsToCheck));
		}

		public void PostRequest<T>(SdkType sdkType, string url, WWWForm data, WebRequestHeader requestHeader, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
			where T : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(PostRequestCor<T>(sdkType, url, data, headers, onComplete, onError, errorsToCheck));
		}

		public void PostRequest<T>(SdkType sdkType, string url, WWWForm data, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
			where T : class
		{
			var headers = GetAnalyticHeaders(sdkType);
			StartCoroutine(PostRequestCor<T>(sdkType, url, data, headers, onComplete, onError, errorsToCheck));
		}
		#endregion

		#region PostUploadRequest
		public void PostUploadRequest<T>(SdkType sdkType, string url, string pathToFile, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
			where T: class
		{
			List<WebRequestHeader> headers = null;
			if (requestHeaders != null)
				headers = AppendAnalyticHeaders(sdkType, requestHeaders.ToArray());
			StartCoroutine(PostUploadRequestCor(sdkType, url, pathToFile, headers, onComplete, onError, errorsToCheck));
		}

		public void PostUploadRequest(SdkType sdkType, string url, byte[] fileData, List<WebRequestHeader> requestHeaders, Action<string> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			List<WebRequestHeader> headers = null;
			if (requestHeaders != null)
				headers = AppendAnalyticHeaders(sdkType, requestHeaders.ToArray());
			StartCoroutine(PostUploadRequestCor(sdkType, url, fileData, headers, onComplete, onError, errorsToCheck));
		}
		#endregion


		IEnumerator PostRequestCor(SdkType sdkType, string url, object jsonObject, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			url = AppendAnalyticsToUrl(sdkType, url);
			UnityWebRequest webRequest = PreparePostWebRequest(url, jsonObject, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		IEnumerator PostRequestCor<T>(SdkType sdkType, string url, object jsonObject, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			url = AppendAnalyticsToUrl(sdkType, url);
			UnityWebRequest webRequest = PreparePostWebRequest(url, jsonObject, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}
		
		IEnumerator PostUploadRequestCor<T>(SdkType sdkType, string url, string pathToFile, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			url = AppendAnalyticsToUrl(sdkType, url);
			UnityWebRequest webRequest = PreparePostUploadRequest(url, pathToFile, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		IEnumerator PostUploadRequestCor(SdkType sdkType, string url, byte[] fileData, List<WebRequestHeader> requestHeaders, Action<string> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			url = AppendAnalyticsToUrl(sdkType, url);
			UnityWebRequest webRequest = UnityWebRequest.Post(url, UnityWebRequest.kHttpVerbPOST);
			webRequest.timeout = 10;
			webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(fileData);
			AttachHeadersToPostRequest(webRequest, requestHeaders, withContentType: false);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}
		
		IEnumerator PostRequestCor<T>(SdkType sdkType, string url, WWWForm data, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			url = AppendAnalyticsToUrl(sdkType, url);
			UnityWebRequest webRequest = UnityWebRequest.Post(url, data);

			AttachHeadersToPostRequest(webRequest, requestHeaders, withContentType: false);

			yield return StartCoroutine(PerformWebRequest<T>(webRequest, onComplete, onError, errorsToCheck));
		}

		private UnityWebRequest PreparePostWebRequest(string url, object jsonObject, List<WebRequestHeader> requestHeaders)
		{
			UnityWebRequest webRequest = UnityWebRequest.Post(url, UnityWebRequest.kHttpVerbPOST);
			webRequest.timeout = 10;

			AttachBodyToPostRequest(webRequest, jsonObject);
			AttachHeadersToPostRequest(webRequest, requestHeaders);

			return webRequest;
		}
		
		private UnityWebRequest PreparePostUploadRequest(string url, string pathToFile, List<WebRequestHeader> requestHeaders)
		{
			var webRequest = UnityWebRequest.Post(url, UnityWebRequest.kHttpVerbPOST);
			webRequest.timeout = 10;

			AttachFileToUploadRequest(webRequest, pathToFile);
			AttachHeadersToPostRequest(webRequest, requestHeaders);

			return webRequest;
		}
		
		private void AttachFileToUploadRequest(UnityWebRequest webRequest, string pathToFile)
		{
			if (!string.IsNullOrEmpty(pathToFile)) {
				webRequest.uploadHandler = new UploadHandlerFile(pathToFile);
			}
		}

		private void AttachBodyToPostRequest(UnityWebRequest webRequest, object jsonObject)
		{
			if (jsonObject != null) {
				string jsonData = JsonConvert.SerializeObject(jsonObject).Replace('\n', ' ');
				byte[] body = new UTF8Encoding().GetBytes(jsonData);
				webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(body);
			}
		}

		private void AttachHeadersToPostRequest(UnityWebRequest webRequest, List<WebRequestHeader> requestHeaders, bool withContentType = true)
		{
			if (withContentType)
			{
				if (requestHeaders != null)
					requestHeaders.Add(WebRequestHeader.ContentTypeHeader());
				else
					requestHeaders = new List<WebRequestHeader>() { WebRequestHeader.ContentTypeHeader() };
			}

			AttachHeaders(webRequest, requestHeaders);
		}
	}
}

