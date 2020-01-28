using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoBehaviour
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
			try {
				error = CheckResponseForErrors(webRequest, errorsToCheck);
			}catch(Exception e) {
				Debug.LogWarning(e.Message);
				error = null;
			}
			if (error == null) {
				string data = webRequest.downloadHandler.text;
				if (data != null) {
					onComplete?.Invoke(data);
				} else {
					error = Error.UnknownError;
				}
			}
			if (error != null) {
				TriggerOnError(onError, error);
			}
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
			if (error == null) {
				T data = GetResponsePayload<T>(webRequest);
				if(data != null) {
					onComplete?.Invoke(data);
				} else {
					error = Error.UnknownError;
				}
			}
			if (error != null) {
				TriggerOnError(onError, error);
			}
		}

		T GetResponsePayload<T>(UnityWebRequest webRequest) where T: class
		{
			string responseData = webRequest.downloadHandler.text;
			return (responseData != null) ? ParseUtils.FromJson<T>(responseData) : null;
		}

		Error CheckResponseForErrors(UnityWebRequest webRequest, Dictionary<string, ErrorType> errorsToCheck)
		{
			if (webRequest.isNetworkError) {
				return Error.NetworkError;
			}
			return CheckResponsePayloadForErrors(webRequest, errorsToCheck);
		}

		Error CheckResponsePayloadForErrors(UnityWebRequest webRequest, Dictionary<string, ErrorType> errorsToCheck)
		{
			string responseData = webRequest.downloadHandler.text;
			Debug.Log(
				"URL: " + webRequest.url + Environment.NewLine +
				"RESPONSE: " + responseData
				);
			return (responseData != null) ? TryParseErrorMessage(responseData, errorsToCheck) : null;
		}

		Error TryParseErrorMessage(string json, Dictionary<string, ErrorType> errorsToCheck)
		{
			var error = ParseUtils.ParseError(json);
			if (error != null && !string.IsNullOrEmpty(error.statusCode)) {
				if (errorsToCheck != null && errorsToCheck.ContainsKey(error.statusCode)) {
					error.ErrorType = errorsToCheck[error.statusCode];
					return error;
				}

				if (Error.GeneralErrors.ContainsKey(error.statusCode)) {
					error.ErrorType = Error.GeneralErrors[error.statusCode];
					return error;
				}

				return Error.UnknownError;
			}

			return null;
		}

		void TriggerOnError(Action<Error> onError, Error error)
		{
			onError?.Invoke(error);
		}
	}
}

