using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Xsolla.Core
{
	internal class MainThreadExecutor : MonoBehaviour
	{
		private readonly ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}

		private void Update()
		{
			if (actions.IsEmpty)
				return;

			while (actions.TryDequeue(out var action))
			{
				action?.Invoke();
			}
		}

		public void Enqueue(Action action)
		{
			actions.Enqueue(action);
		}
	}
}