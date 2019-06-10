using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla
{
	public class WebRequestHelper : MonoBehaviour
	{
		static WebRequestHelper _instance;
		
		public static WebRequestHelper Instance
		{
			get
			{
				if (_instance == null)
				{
					Init();
				}

				return _instance;
			}
		}
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		internal static void Init()
		{
			if (ReferenceEquals(_instance, null))
			{
				var instances = FindObjectsOfType<WebRequestHelper>();

				if (instances.Length > 1)
				{
					Debug.LogError(typeof(WebRequestHelper) + " Something went really wrong " +
									" - there should never be more than 1 " + typeof(WebRequestHelper) +
									" Reopening the scene might fix it.");
				}
				else if (instances.Length == 0)
				{
					var singleton = new GameObject {hideFlags = HideFlags.HideAndDontSave};
					_instance = singleton.AddComponent<WebRequestHelper>();
					singleton.name = typeof(WebRequestHelper).ToString();

					DontDestroyOnLoad(singleton);

					Debug.Log("[Singleton] An _instance of " + typeof(WebRequestHelper) +
								" is needed in the scene, so '" + singleton.name +
								"' was created with DontDestroyOnLoad.");
				}
				else
				{
					Debug.Log("[Singleton] Using _instance already created: " + _instance.gameObject.name);
				}
			}
		}

		// Prevent accidental WebRequestHelper instantiation
		WebRequestHelper()
		{
		}
		
		public static IEnumerator PostRequest(string url, WWWForm form, WebRequestHeader requestHeader, Action<string> onComplete = null, Action<XsollaError> onError = null)
		{
			var webRequest = UnityWebRequest.Post(url, form);
			
			webRequest.SetRequestHeader(requestHeader.Name, requestHeader.Value);

#if UNITY_2018_1_OR_NEWER
			yield return webRequest.SendWebRequest();
#else
			yield return webRequest.Send();
#endif

			if (webRequest.isNetworkError)
			{
				if (onError != null)
				{
					onError(new XsollaError {ErrorType = ErrorType.NetworkError});
				}
			}
			else
			{
				if (onComplete != null)
				{
					onComplete(webRequest.downloadHandler.text);
				}
			}
		}
		
		public static IEnumerator GetRequest(string url, Action<string> onComplete = null, Action<XsollaError> onError = null)
		{
			var webRequest = UnityWebRequest.Get(url);

#if UNITY_2018_1_OR_NEWER
			yield return webRequest.SendWebRequest();
#else
			yield return webRequest.Send();
#endif

			if (webRequest.isNetworkError)
			{
				if (onError != null)
				{
					onError(new XsollaError {ErrorType = ErrorType.NetworkError});
				}
			}
			else
			{
				if (onComplete != null)
				{
					onComplete(webRequest.downloadHandler.text);
				}
			}
		}
	}
}

