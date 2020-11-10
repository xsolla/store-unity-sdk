using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

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
			if (requestHeaders != null)
				requestHeaders.Add(WebRequestHeader.ContentTypeHeader());
			else
				requestHeaders = new List<WebRequestHeader>() { WebRequestHeader.ContentTypeHeader() };

			var task = PatchAsync(url, jsonObject, requestHeaders);
			task.Start();
			yield return new WaitWhile(() => task.IsCompleted || task.IsCanceled || task.IsFaulted);
			if (!task.IsCompleted)
			{
				Debug.LogError($"PATCH task is {(task.IsCanceled ? "canceled" : "faulted")}");
				yield break;
			}
			HttpResponseMessage response = task.Result;
			Error error = CheckResponsePayloadForErrors(url, response.Content.ToString(), errorsToCheck);
			if (error == null)
			{
				T responseData = GetResponsePayload<T>(response.Content.ToString());
				if(responseData != null) {
					onComplete?.Invoke(responseData);
				} else {
					error = Error.UnknownError;
				}
			}
			TriggerOnError(onError, error);
		}

		private async Task<HttpResponseMessage> PatchAsync<T>(string url, T data, List<WebRequestHeader> headers = null)
		{
			var client = new HttpClient();
			var method = new HttpMethod("PATCH");
			var request = new HttpRequestMessage(new HttpMethod("PATCH"), url) {
				Content = new ByteArrayContent(GetJsonData(data))
			};
			if (headers != null && headers.Any())
				headers.ForEach(h => request.Headers.Add(h.Name, h.Value));
			var response = new HttpResponseMessage();
			try {
				response = await client.SendAsync(request);
			} catch(TaskCanceledException e) {
				Debug.LogError($"ERROR: {e}");
			}
			return response;
		}

		private byte[] GetJsonData<T>(T jsonObject)
		{
			if (jsonObject == null) return new byte[0];
			var jsonData = JsonConvert.SerializeObject(jsonObject).Replace('\n', ' ');
			return new UTF8Encoding().GetBytes(jsonData);
		}
	}
}

