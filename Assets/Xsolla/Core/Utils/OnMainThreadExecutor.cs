using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Xsolla.Core
{
	public class OnMainThreadExecutor : MonoBehaviour
	{
		private readonly ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();

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