using System;
using UnityEngine;

namespace Xsolla.Core
{
	public partial class XsollaWebCallbacks : MonoBehaviour
	{
		public static event Action<object> MessageReceived;

		public void PublishMessage(object message)
		{
			MessageReceived?.Invoke(message);
		}

		public static XsollaWebCallbacks Instance { get; private set; }

		public static void CreateInstance()
		{
			if (!Instance)
			{
				var go = new GameObject("XsollaWebCallbacks");
				Instance = go.AddComponent<XsollaWebCallbacks>();
				DontDestroyOnLoad(go);
			}
		}
	}
}