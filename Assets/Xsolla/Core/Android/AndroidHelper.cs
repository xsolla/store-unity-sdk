using System;
using UnityEngine;

namespace Xsolla.Core.Android
{
	public class AndroidHelper : IDisposable
	{
		private AndroidJavaClass _unityPlayer;
		private AndroidJavaObject _activity;
		private AndroidJavaObject _context;

		private static OnMainThreadExecutor _onMainThreadExecutorInstance;

		public AndroidHelper()
		{
			if (!_onMainThreadExecutorInstance)
				_onMainThreadExecutorInstance = new GameObject("Android Main Thread Executor").AddComponent<OnMainThreadExecutor>();
		}

		public OnMainThreadExecutor OnMainThreadExecutor => _onMainThreadExecutorInstance;

		private AndroidJavaClass UnityPlayer
		{
			get
			{
				if (_unityPlayer == null)
					_unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

				return _unityPlayer;
			}
		}

		public AndroidJavaObject CurrentActivity
		{
			get
			{
				if (_activity == null)
					_activity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

				return _activity;
			}
		}

		public AndroidJavaObject ApplicationContext
		{
			get
			{
				if (_context == null)
					_context = CurrentActivity.Call<AndroidJavaObject>("getApplicationContext");

				return _context;
			}
		}

		public void Dispose()
		{
			_context?.Dispose();
			_activity?.Dispose();
			_unityPlayer?.Dispose();
		}
	}
}