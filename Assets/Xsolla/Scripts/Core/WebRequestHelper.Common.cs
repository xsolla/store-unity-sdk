using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoBehaviour
	{
		public void AddOptionalHeadersTo(UnityWebRequest request)
		{
			if (Application.platform != RuntimePlatform.WebGLPlayer) {
				request.SetRequestHeader("X-ENGINE", "UNITY");
				request.SetRequestHeader("X-ENGINE-V", Application.unityVersion.ToUpper());
				request.SetRequestHeader("X-SDK", "STORE");
				request.SetRequestHeader("X-SDK-V", Constants.StoreSdkVersion);
			}
		}

		public void AddContentTypeHeaderTo(UnityWebRequest request)
		{
			WebRequestHeader contentHeader = WebRequestHeader.ContentTypeHeader();
			request.SetRequestHeader(contentHeader.Name, contentHeader.Value);
		}

		private IEnumerator PerformWebRequest(UnityWebRequest webRequest, Action onComplete, Action<Error> onError, Dictionary<string, ErrorType> errorsToCheck)
		{
			yield return StartCoroutine(SendWebRequest(webRequest));

			ProcessRequest(webRequest, onComplete, onError, errorsToCheck);
		}

		private IEnumerator PerformWebRequest(UnityWebRequest webRequest, Action<string> onComplete, Action<Error> onError, Dictionary<string, ErrorType> errorsToCheck)
		{
			yield return StartCoroutine(SendWebRequest(webRequest));

			ProcessRequest(webRequest, onComplete, onError, errorsToCheck);
		}

		private IEnumerator PerformWebRequest<T>(UnityWebRequest webRequest, Action<T> onComplete, Action<Error> onError, Dictionary<string, ErrorType> errorsToCheck) where T: class
		{
			yield return StartCoroutine(SendWebRequest(webRequest));

			ProcessRequest(webRequest, onComplete, onError, errorsToCheck);
		}

		private IEnumerator SendWebRequest(UnityWebRequest webRequest)
		{
#if UNITY_2018_1_OR_NEWER
			yield return webRequest.SendWebRequest();
#else
			yield return webRequest.Send();
#endif
		}
	}
}

