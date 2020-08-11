using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoSingleton<WebRequestHelper>
	{
		public void PostRequest<T, D>(string url, D jsonObject, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
			where T : class
			where D : class
		{
			StartCoroutine(PostRequestCor<T>(url, jsonObject, requestHeaders, onComplete, onError, errorsToCheck));
		}

		public void PostRequest<T, D>(string url, D jsonObject, [NotNull]WebRequestHeader requestHeader, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
			where T : class
			where D : class
		{
			List<WebRequestHeader> headers = (requestHeader != null)
				? new List<WebRequestHeader> { requestHeader }
				: null;
			PostRequest<T, D>(url, jsonObject, headers, onComplete, onError, errorsToCheck);
		}

		public void PostRequest<T, D>(string url, D jsonObject, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
			where T : class
			where D : class
		{
			PostRequest<T, D>(url, jsonObject, new List<WebRequestHeader>(), onComplete, onError, errorsToCheck);
		}

		public void PostRequest<T>(string url, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
			where T : class
		{
			StartCoroutine(PostRequestCor(url, null, requestHeaders, onComplete, onError, errorsToCheck));
		}

		public void PostRequest<T>(string url, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
			where T : class
		{
			PostRequest(url, null, onComplete, onError, errorsToCheck);
		}

		public void PostRequest<D>(string url, D jsonObject, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
			where D : class
		{
			StartCoroutine(PostRequestCor(url, jsonObject, requestHeaders, onComplete, onError, errorsToCheck));
		}

		public void PostRequest<D>(string url, D jsonObject = null, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
			where D : class
		{
			PostRequest(url, jsonObject, new List<WebRequestHeader>(), onComplete, onError, errorsToCheck);
		}

		public void PostRequest(string url, List<WebRequestHeader> requestHeaders, Action<string> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			StartCoroutine(PostRequestCor(url, null, requestHeaders, onComplete, onError, errorsToCheck));
		}

		public void PostRequest(string url, Action<string> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			PostRequest(url, null, onComplete, onError, errorsToCheck);
		}

		public void PostRequest(string url, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			StartCoroutine(PostRequestCor(url, null, requestHeaders, onComplete, onError, errorsToCheck));
		}

		public void PostRequest(string url, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			PostRequest(url, null, onComplete, onError, errorsToCheck);
		}
		
		public void PostUploadRequest<T>(string url, string pathToFile, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
			where T: class
		{
			StartCoroutine(PostUploadRequestCor(url, pathToFile, requestHeaders, onComplete, onError, errorsToCheck));
		}

		IEnumerator PostRequestCor(string url, object jsonObject, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			UnityWebRequest webRequest = PreparePostWebRequest(url, jsonObject, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		IEnumerator PostRequestCor(string url, object jsonObject, List<WebRequestHeader> requestHeaders, Action<string> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null)
		{
			UnityWebRequest webRequest = PreparePostWebRequest(url, jsonObject, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		IEnumerator PostRequestCor<T>(string url, object jsonObject, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			UnityWebRequest webRequest = PreparePostWebRequest(url, jsonObject, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}
		
		IEnumerator PostUploadRequestCor<T>(string url, string pathToFile, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			UnityWebRequest webRequest = PreparePostUploadRequest(url, pathToFile, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
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

		private void AttachHeadersToPostRequest(UnityWebRequest webRequest, List<WebRequestHeader> requestHeaders)
		{
			AddContentTypeHeaderTo(webRequest);
			AddOptionalHeadersTo(webRequest);

			if (requestHeaders != null) {
				foreach (WebRequestHeader header in requestHeaders) {
					webRequest.SetRequestHeader(header.Name, header.Value);
				}
			}
		}
	}
}

