using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoSingleton<WebRequestHelper>
	{
		private List<UnityWebRequest> _requests = new List<UnityWebRequest>();

		public bool IsBusy()
		{
			return _requests.Count > 0;
		}
		
		public void StopAll()
		{
			_requests.ForEach(r => r.Dispose());
			_requests.Clear();
		}
		
		protected override void OnDestroy()
		{
			StopAll();
			StopAllCoroutines();
			base.OnDestroy();
		}

		private IEnumerator InternalPerformWebRequest(UnityWebRequest webRequest, Action requestProccesAction)
		{
			webRequest.disposeDownloadHandlerOnDispose = true;
			webRequest.disposeUploadHandlerOnDispose = true;
			webRequest.disposeCertificateHandlerOnDispose = true;
			_requests.Add(webRequest);
			
			yield return StartCoroutine(SendWebRequest(webRequest));
			requestProccesAction?.Invoke();
			
			_requests.Remove(webRequest);
		}

		private IEnumerator PerformWebRequest(UnityWebRequest webRequest, Action onComplete, Action<Error> onError, Dictionary<string, ErrorType> errorsToCheck)
		{
			yield return InternalPerformWebRequest(webRequest,
				() => ProcessRequest(webRequest, onComplete, onError, errorsToCheck));
		}
		
		private IEnumerator PerformWebRequest(UnityWebRequest webRequest, Action<int> onComplete, Action<Error> onError, Dictionary<string, ErrorType> errorsToCheck)
		{
			yield return InternalPerformWebRequest(webRequest,
				() => ProcessRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private IEnumerator PerformWebRequest(UnityWebRequest webRequest, Action<string> onComplete, Action<Error> onError, Dictionary<string, ErrorType> errorsToCheck)
		{
			yield return InternalPerformWebRequest(webRequest,
				() => ProcessRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private IEnumerator PerformWebRequest<T>(UnityWebRequest webRequest, Action<T> onComplete, Action<Error> onError, Dictionary<string, ErrorType> errorsToCheck) where T : class
		{
			yield return InternalPerformWebRequest(webRequest,
				() => ProcessRequest(webRequest, onComplete, onError, errorsToCheck));
		}
		
		private IEnumerator PerformWebRequest(UnityWebRequest webRequest, Action<Texture2D> onComplete, Action<Error> onError)
		{
			yield return InternalPerformWebRequest(webRequest,
				() => ProcessRequest(webRequest, onComplete, onError));
		}

		private IEnumerator SendWebRequest(UnityWebRequest webRequest)
		{
#if UNITY_2018_1_OR_NEWER
			yield return webRequest.SendWebRequest();
#else
			yield return webRequest.Send();
#endif
		}

		private void AttachHeaders(UnityWebRequest webRequest, List<WebRequestHeader> requestHeaders)
		{
			if (requestHeaders != null)
				foreach (var header in requestHeaders)
					webRequest.SetRequestHeader(header.Name, header.Value);
		}
	}
}

