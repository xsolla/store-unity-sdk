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
		public void PutRequest<T>(string url, T jsonObject, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			StartCoroutine(PutRequestCor(url, jsonObject, requestHeaders, onComplete, onError, errorsToCheck));
		}

		IEnumerator PutRequestCor<T>(string url, T jsonObject, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
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

			foreach (var header in requestHeaders)
				webRequest.SetRequestHeader(header.Name, header.Value);
		}
	}
}

