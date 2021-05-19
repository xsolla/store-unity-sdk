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
		public void PatchRequest<T, D>(string url, D jsonObject, List<WebRequestHeader> requestHeaders = null, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			StartCoroutine(PatchRequestCor<T, D>(url, jsonObject, requestHeaders, onComplete, onError, errorsToCheck));
		}

		IEnumerator PatchRequestCor<T, D>(string url, D jsonObject, List<WebRequestHeader> requestHeaders = null, Action<T> onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T : class
		{
			UnityWebRequest webRequest = new UnityWebRequest(url, "PATCH");
			webRequest.downloadHandler = new DownloadHandlerBuffer();
			AttachBodyToPatchRequest(webRequest, jsonObject);
			AttachHeadersToPatchRequest(webRequest, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private void AttachBodyToPatchRequest(UnityWebRequest webRequest, object jsonObject)
		{
			if (jsonObject != null)
			{
				string jsonData = JsonConvert.SerializeObject(jsonObject).Replace('\n', ' ');
				byte[] body = new UTF8Encoding().GetBytes(jsonData);
				webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(body);
			}
		}

		private void AttachHeadersToPatchRequest(UnityWebRequest webRequest, List<WebRequestHeader> requestHeaders, bool withContentType = true)
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

		private void AttachHeaders(UnityWebRequest webRequest, List<WebRequestHeader> requestHeaders)
		{
			if (requestHeaders != null)
				foreach (var header in requestHeaders)
					webRequest.SetRequestHeader(header.Name, header.Value);
		}
	}
}
