using System.Collections;
using UnityEngine;

namespace Xsolla.Core
{
	internal class CoroutinesExecutor : MonoBehaviour
	{
		private static CoroutinesExecutor _instance;

		private static CoroutinesExecutor Instance
		{
			get
			{
				if (!_instance)
				{
					_instance = new GameObject("CoroutinesExecutor").AddComponent<CoroutinesExecutor>();
					DontDestroyOnLoad(_instance.gameObject);
				}

				return _instance;
			}
		}

		public static Coroutine Run(IEnumerator coroutine)
		{
			return Instance.StartCoroutine(coroutine);
		}

		public static void Stop(Coroutine coroutine)
		{
			if (coroutine != null)
				Instance.StopCoroutine(coroutine);
		}
	}
}