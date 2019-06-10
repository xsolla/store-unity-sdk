using System;
using UnityEngine;

namespace Xsolla
{
	public static class WebRequest
	{
		public static void PostRequest(string url, WWWForm form, WebRequestHeader requestHeader, Action<string> onComplete = null, Action<XsollaError> onError = null)
		{
			WebRequestHelper.Instance.StartCoroutine(WebRequestHelper.PostRequest(url, form, requestHeader, onComplete, onError));
		}

		public static void GetRequest(string url, Action<string> onComplete = null, Action<XsollaError> onError = null)
		{
			WebRequestHelper.Instance.StartCoroutine(WebRequestHelper.GetRequest(url, onComplete, onError));
		}
	}
}