using System;
using UnityEngine;

namespace Xsolla.Core.Android
{
	public class AndroidHelper : IDisposable
	{
		private AndroidJavaClass _unityPlayer;
		private AndroidJavaObject _activity;
		private AndroidJavaObject _context;

	#if !UNITY_ANDROID
		public AndroidHelper ()
		{
			Debug.LogError("AndroidHelper.Ctor: This class is not supposed to work outside of Android context");
		}
	#endif

		public AndroidJavaClass UnityPlayer
		{
			get
			{
				if(_unityPlayer == null)
				{
					_unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				}

				return _unityPlayer;
			}
		}

		public AndroidJavaObject CurrentActivity
		{
			get
			{
				if(_activity == null)
				{
					var unityPlayer = UnityPlayer;
					_activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
				}

				return _activity;
			}
		}

		public AndroidJavaObject ApplicationContext
		{
			get
			{
				if (_context == null)
				{
					var activity = CurrentActivity;
					_context = activity.Call<AndroidJavaObject>("getApplicationContext");
				}

				return _context;
			}
		}

		public void Dispose()
		{
			if (_context != null)
				_context.Dispose();
			if (_activity != null)
				_activity.Dispose();
			if (_unityPlayer != null)
				_unityPlayer.Dispose();
		}
	}
}
