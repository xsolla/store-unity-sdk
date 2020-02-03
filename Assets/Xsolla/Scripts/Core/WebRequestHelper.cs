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
		static WebRequestHelper _instance;
		public static WebRequestHelper Instance {
			get {
				if (_instance == null) {
					_instance = GetInstance();
				}
				return _instance;
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		internal static WebRequestHelper GetInstance()
		{
			WebRequestHelper instance = _instance;
			if (ReferenceEquals(_instance, null)) {
				var instances = FindObjectsOfType<WebRequestHelper>();

				switch ((uint)instances.Length) {
					case 0: {
							var singleton = new GameObject {
								hideFlags = HideFlags.HideAndDontSave,
								name = typeof(WebRequestHelper).ToString()
							};
							instance = singleton.AddComponent<WebRequestHelper>();

							DontDestroyOnLoad(singleton);

							Debug.Log(
								"[Singleton] An _instance of " + typeof(WebRequestHelper) +
								" is needed in the scene, so '" + singleton.name +
								"' was created with DontDestroyOnLoad.");
							break;
						}
					case 1: {
							WebRequestHelper founded = instances.ToList().First();
							Debug.Log("[Singleton] Using _instance already created: " + founded.gameObject.name);
							instance = founded;
							break;
						}
					default: {
							Debug.LogError(
								typeof(WebRequestHelper) + " search error: Something went really wrong " +
								" - there should never be more than 1 " + typeof(WebRequestHelper) +
								" Reopening the scene might fix it.");
							break;
						}
				}
			} else {
				Debug.LogWarning(
					"Something went wrong. We try create " + typeof(WebRequestHelper) +
					", but reference to singleton is not null."
					);
			}
			return instance;
		}

		// Prevent accidental WebRequestHelper instantiation
		WebRequestHelper()
		{
		}
	}
}

