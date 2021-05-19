using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
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

			yield return new WaitUntil(() => task.IsCompleted || task.IsCanceled || task.IsFaulted);
			if (!task.IsCompleted)
			{
				Debug.LogError(string.Format("PATCH task is {0}", (task.IsCanceled ? "canceled" : "faulted")));
				yield break;
			}
			string response = task.Result;

			Error error = CheckResponsePayloadForErrors(url, response, errorsToCheck);
			if (error == null)
			{
				T responseData = GetResponsePayload<T>(response);
				if(responseData != null)
				{
					if (onComplete != null)
						onComplete.Invoke(responseData);
				}
				else
				{
					error = Error.UnknownError;
				}
			}
			TriggerOnError(onError, error);
		}

		private async Task<string> PatchAsync<T>(string url, T data, List<WebRequestHeader> headers = null)
		{
			var method = new HttpMethod("PATCH");
			var jsonString = GetJsonDataAsStringForPatch<T>(data);
			HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

			var request = new HttpRequestMessage(method, url) {Content = content};

			if (headers != null && headers.Any())
			{
				headers.ForEach(header =>
				{
					try
					{
						request.Headers.TryAddWithoutValidation(header.Name, header.Value);
					}
					catch (Exception ex)
					{
						Debug.LogWarning(string.Format("Could not assign header name: {0} value: {1}, attempt resulted in error: {2}", header.Name, header.Value, ex.Message));
					}
				});
			}

			var client = new HttpClient();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			var response = new HttpResponseMessage();
			try
			{
				response = await client.SendAsync(request);
			}
			catch(Exception e)
			{
				Debug.LogError(string.Format("ERROR: {0}", e));
			}

			return await response.Content.ReadAsStringAsync();
		}


		//Be aware - this method is suitable only for JSON-classes that have string-only fields
		private string GetJsonDataAsStringForPatch<T>(T jsonObject)
		{
			var type = typeof(T);
			var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
			var builder = new StringBuilder();
			builder.Append("{");

			if (fields != null && fields.Length > 0)
			{
				foreach (var field in fields)
				{
					var key = field.Name;
					var value = field.GetValue(jsonObject) as string;

					if (value != null)
						builder.Append(string.Format("\"{0}\":\"{1}\"", key, value)).Append(",");
				}

				builder.Remove(builder.Length - 1, 1);
			}

			builder.Append("}");

			return builder.ToString();
		}
	}
}

