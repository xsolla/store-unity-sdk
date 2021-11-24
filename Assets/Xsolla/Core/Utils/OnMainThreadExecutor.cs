using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Core
{
	public class OnMainThreadExecutor : MonoBehaviour
	{
		private static Queue<Action> _queue = new Queue<Action>();

		private void Update()
		{
			if (_queue.Count == 0)
				return;
			
			var temp = _queue;
			_queue = null;
			var action = temp.Dequeue();

			if (action != null)
				action.Invoke();

			_queue = temp;
		}

		public void Enqueue(Action action)
		{
			while (_queue == null)
			{
				//Wait
			}

			_queue.Enqueue(action);
		}
	}
}
