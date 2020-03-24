using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoSingleton<WebRequestHelper>
	{
		public void PutRequest<T>(string url, T jsonObject, WebRequestHeader requestHeader, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T: class
		{
			StartCoroutine(PutRequestCor(url, jsonObject, requestHeader, onComplete, onError, errorsToCheck));
		}

		IEnumerator PutRequestCor<T>(string url, T jsonObject, WebRequestHeader requestHeader, Action onComplete = null, Action<Error> onError = null, Dictionary<string, ErrorType> errorsToCheck = null) where T: class
		{
			UnityWebRequest webRequest = new UnityWebRequest(url, "PUT") {
				downloadHandler = new DownloadHandlerBuffer()
			};
			
			AttachBodyToPutRequest(webRequest, jsonObject);
			AttachHeadersToPutRequest(webRequest, requestHeader);

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

		private void AttachHeadersToPutRequest(UnityWebRequest webRequest, WebRequestHeader requestHeader)
		{
			AddOptionalHeadersTo(webRequest);
			AddContentTypeHeaderTo(webRequest);

			if (requestHeader != null) {
				webRequest.SetRequestHeader(requestHeader.Name, requestHeader.Value);
			}
		}
	}
}

