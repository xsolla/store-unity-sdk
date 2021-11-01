using System;
using System.Collections.Concurrent;

namespace Xsolla.Core
{
	public class OnMainThreadExecutor : MonoSingleton<OnMainThreadExecutor>
	{
		private readonly ConcurrentQueue<Action> RunOnMainThread = new ConcurrentQueue<Action>();

		private void Update()
		{
			if (RunOnMainThread.IsEmpty)
				return;

			while (RunOnMainThread.TryDequeue(out var action))
			{
				action?.Invoke();
			}
		}

		public void Enqueue(Action action)
		{
			Instance.RunOnMainThread.Enqueue(action);
		}
	}
}