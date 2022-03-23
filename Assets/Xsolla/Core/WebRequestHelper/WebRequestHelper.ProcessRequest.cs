using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoSingleton<WebRequestHelper>
	{
		/// <summary>
		/// Processing request and invoke no data callback on success
		/// </summary>
		/// <param name="webRequest"></param>
		/// <param name="onComplete"></param>
		/// <param name="onError"></param>
		/// <param name="errorsToCheck"></param>
		private void ProcessRequest(UnityWebRequest webRequest, Action onComplete, Action<Error> onError, ErrorCheckType errorsToCheck)
		{
			if (CheckNoErrors(webRequest, errorsToCheck, out var error))
				onComplete?.Invoke();
			else
				onError?.Invoke(error);
		}

		/// <summary>
		/// Processing request and invoke response code callback on success
		/// </summary>
		/// <param name="webRequest"></param>
		/// <param name="onComplete"></param>
		/// <param name="onError"></param>
		/// <param name="errorsToCheck"></param>
		private void ProcessRequest(UnityWebRequest webRequest, Action<int> onComplete, Action<Error> onError, ErrorCheckType errorsToCheck)
		{
			if (CheckNoErrors(webRequest, errorsToCheck, out var error))
				onComplete?.Invoke((int)webRequest.responseCode);
			else
				onError?.Invoke(error);
		}

		/// <summary>
		/// Processing request and invoke raw data (Action<string>) callback on success
		/// </summary>
		/// <param name="webRequest"></param>
		/// <param name="onComplete"></param>
		/// <param name="onError"></param>
		/// <param name="errorsToCheck"></param>
		private void ProcessRequest(UnityWebRequest webRequest, Action<string> onComplete, Action<Error> onError, ErrorCheckType errorsToCheck)
		{
			if (CheckNoErrors(webRequest, errorsToCheck, out var error))
			{
				string data = webRequest.downloadHandler.text;
				if (data != null)
					onComplete?.Invoke(data);
				else
					onError?.Invoke(Error.UnknownError);
			}
			else
				onError?.Invoke(error);
		}

		/// <summary>
		/// Processing request and invoke Texture2D (Action<Texture2D>) callback on success
		/// </summary>
		/// <param name="webRequest"></param>
		/// <param name="onComplete"></param>
		/// <param name="onError"></param>
		private void ProcessRequest(UnityWebRequest webRequest, Action<Texture2D> onComplete, Action<Error> onError)
		{
			if (CheckNoErrors(webRequest, ErrorCheckType.CommonErrors, out var error, log: false))
			{
				var texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
				if (texture != null)
					onComplete?.Invoke(texture);
				else
					onError?.Invoke(Error.UnknownError);
			}
			else
				onError?.Invoke(error);
		}

		/// <summary>
		/// Processing request and invoke Action<T> callback by success
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="webRequest"></param>
		/// <param name="onComplete"></param>
		/// <param name="onError"></param>
		/// <param name="errorsToCheck"></param>
		private void ProcessRequest<T>(UnityWebRequest webRequest, Action<T> onComplete, Action<Error> onError, ErrorCheckType errorsToCheck) where T : class
		{
			if (CheckNoErrors(webRequest, errorsToCheck, out var error))
			{
				var rawData = webRequest.downloadHandler.text;
				var data = (rawData != null) ? ParseUtils.FromJson<T>(rawData) : null;
				if (data != null)
					onComplete?.Invoke(data);
				else
					onError?.Invoke(Error.UnknownError);
			}
			else
				onError?.Invoke(error);
		}

		private bool CheckNoErrors(UnityWebRequest webRequest, ErrorCheckType errorsToCheck, out Error error, bool log = true)
		{
#if UNITY_2020_1_OR_NEWER
			if (webRequest.result == UnityWebRequest.Result.ConnectionError)
			{
				error = Error.NetworkError;
				return false;
			}
#else
			if (webRequest.isNetworkError)
			{
				error = Error.NetworkError;
				return false;
			}
#endif

			var url = webRequest.url;
			var data = webRequest.downloadHandler.text;

			if (log)
				Debug.Log($"URL: {url}{Environment.NewLine}RESPONSE: {data}");

			if (string.IsNullOrEmpty(data))
			{
				error = null;
				return true;
			}

			error = ParseUtils.ParseError(data);

			if (error != null && !string.IsNullOrEmpty(error.statusCode))
			{
				if (CodeToErrorType.TryGetSpecificType(error.statusCode, errorsToCheck, out var specificErrorType))
					error.ErrorType = specificErrorType;
				else if (CodeToErrorType.TryGetCommonType(error.statusCode, out var commonErrorType))
					error.ErrorType = commonErrorType;
			}

			return error == null;
		}
	}
}
