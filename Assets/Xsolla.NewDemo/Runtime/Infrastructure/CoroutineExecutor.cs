using System.Collections;
using UnityEngine;

namespace Xsolla.Demo
{
	public class CoroutineExecutor : MonoBehaviour
	{
		private static CoroutineExecutor _instance;

		private static CoroutineExecutor Instance
		{
			get
			{
				if (!_instance)
				{
					var executorGameObject = new GameObject("CoroutineExecutor");
					_instance = executorGameObject.AddComponent<CoroutineExecutor>();
					DontDestroyOnLoad(executorGameObject);
				}

				return _instance;
			}
		}

		public static Coroutine Run(IEnumerator enumerator)
		{
			return Instance.StartCoroutine(enumerator);
		}

		public static void Stop(Coroutine coroutine)
		{
			if (coroutine != null)
				Instance.StopCoroutine(coroutine);
		}
	}
}