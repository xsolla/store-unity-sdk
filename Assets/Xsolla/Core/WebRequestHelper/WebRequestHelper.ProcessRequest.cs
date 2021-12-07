using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoSingleton<WebRequestHelper>
	{
		/// <summary>
		/// Processing request and invoke no data callback by success
		/// </summary>
		/// <param name="webRequest"></param>
		/// <param name="onComplete"></param>
		/// <param name="onError"></param>
		/// <param name="errorsToCheck"></param>
		void ProcessRequest(UnityWebRequest webRequest, Action onComplete, Action<Error> onError, Dictionary<string, ErrorType> errorsToCheck)
		{
			Error error = CheckResponseForErrors(webRequest, errorsToCheck);
			if (error == null)
				onComplete?.Invoke();
			else
				TriggerOnError(onError, error);
		}
		
		void ProcessRequest(UnityWebRequest webRequest, Action<int> onComplete, Action<Error> onError, Dictionary<string, ErrorType> errorsToCheck)
		{
			Error error = CheckResponseForErrors(webRequest, errorsToCheck);
			if (error == null)
				onComplete?.Invoke((int)webRequest.responseCode);
			else
				TriggerOnError(onError, error);
		}

		/// <summary>
		/// Processing request and invoke raw data (Action<string>) callback by success
		/// </summary>
		/// <param name="webRequest"></param>
		/// <param name="onComplete"></param>
		/// <param name="onError"></param>
		/// <param name="errorsToCheck"></param>
		void ProcessRequest(UnityWebRequest webRequest, Action<string> onComplete, Action<Error> onError, Dictionary<string, ErrorType> errorsToCheck)
		{
			Error error;
			try
			{
				error = CheckResponseForErrors(webRequest, errorsToCheck);
			}
			catch (Exception e)
			{
				Debug.LogWarning(e.Message);
				error = null;
			}
			if (error == null)
			{
				string data = webRequest.downloadHandler.text;
				if (data != null)
				{
					onComplete?.Invoke(data);
				}
				else
				{
					error = Error.UnknownError;
				}
			}
			TriggerOnError(onError, error);
		}

		/// <summary>
		/// Processing request and invoke Texture2D (Action<Texture2D>) callback by success
		/// </summary>
		/// <param name="webRequest"></param>
		/// <param name="onComplete"></param>
		/// <param name="onError"></param>
		void ProcessRequest(UnityWebRequest webRequest, Action<Texture2D> onComplete, Action<Error> onError)
		{
			Error error = null;
#if UNITY_2020_1_OR_NEWER
			if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
				error = Error.NetworkError;
#else
			if (webRequest.isNetworkError || webRequest.isHttpError)
				error = Error.NetworkError;
#endif

			if (error == null)
			{
				var texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
				if (texture != null)
				{
					onComplete?.Invoke(texture);
				}
				else
				{
					error = Error.UnknownError;
				}
			}
			TriggerOnError(onError, error);
		}

		/// <summary>
		/// Processing request and invoke Action<T> callback by success
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="webRequest"></param>
		/// <param name="onComplete"></param>
		/// <param name="onError"></param>
		/// <param name="errorsToCheck"></param>
		void ProcessRequest<T>(UnityWebRequest webRequest, Action<T> onComplete, Action<Error> onError, Dictionary<string, ErrorType> errorsToCheck) where T : class
		{
			Error error = CheckResponseForErrors(webRequest, errorsToCheck);
			if (error == null)
			{
				T data = GetResponsePayload<T>(webRequest.downloadHandler.text);
				if (data != null)
				{
					onComplete?.Invoke(data);
				}
				else
				{
					error = Error.UnknownError;
				}
			}
			TriggerOnError(onError, error);
		}

		T GetResponsePayload<T>(string data) where T : class
		{
			return (data != null) ? ParseUtils.FromJson<T>(data) : null;
		}

		Error CheckResponseForErrors(UnityWebRequest webRequest, Dictionary<string, ErrorType> errorsToCheck)
		{
#if UNITY_2020_1_OR_NEWER
			if (webRequest.result == UnityWebRequest.Result.ConnectionError)
				return Error.NetworkError;
#else
			if (webRequest.isNetworkError)
				return Error.NetworkError;
#endif

			return CheckResponsePayloadForErrors(webRequest.url, webRequest.downloadHandler.text, errorsToCheck);
		}

		Error CheckResponsePayloadForErrors(string url, string data, Dictionary<string, ErrorType> errorsToCheck)
		{
			Debug.Log(
				"URL: " + url + Environment.NewLine +
				"RESPONSE: " + (string.IsNullOrEmpty(data) ? string.Empty : data)
				);
			return !string.IsNullOrEmpty(data) ? TryParseErrorMessage(data, errorsToCheck) : null;
		}

		Error TryParseErrorMessage(string json, Dictionary<string, ErrorType> errorsToCheck)
		{
			var error = ParseUtils.ParseError(json);
			if (error != null && !string.IsNullOrEmpty(error.statusCode))
			{
				if (errorsToCheck != null && errorsToCheck.ContainsKey(error.statusCode))
					error.ErrorType = errorsToCheck[error.statusCode];
				else if (Error.GeneralErrors.ContainsKey(error.statusCode))
					error.ErrorType = Error.GeneralErrors[error.statusCode];

				return error;
			}

			return null;
		}

		static void TriggerOnError(Action<Error> onError, Error error)
		{
			if (error != null)
				onError?.Invoke(error);
		}
	}
}
