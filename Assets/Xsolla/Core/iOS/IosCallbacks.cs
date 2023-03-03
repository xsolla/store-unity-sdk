#if UNITY_IOS
using System;
using AOT;

namespace Xsolla.Core
{
	public static class IosCallbacks
	{
		public delegate void ActionVoidCallbackDelegate(IntPtr actionPtr);

		public delegate void ActionStringCallbackDelegate(IntPtr actionPtr, string msg);

		[MonoPInvokeCallback(typeof(ActionVoidCallbackDelegate))]
		public static void ActionVoidCallback(IntPtr actionPtr)
		{
			if (UnityEngine.Debug.isDebugBuild)
			{
				Debug.Log("ActionVoidCallback");
			}

			if (actionPtr != IntPtr.Zero)
			{
				var action = actionPtr.Cast<Action>();
				action();
			}
		}

		[MonoPInvokeCallback(typeof(ActionStringCallbackDelegate))]
		public static void ActionStringCallback(IntPtr actionPtr, string msg)
		{
			if (UnityEngine.Debug.isDebugBuild)
			{
				Debug.Log("ActionStringCallback");
			}

			if (actionPtr != IntPtr.Zero)
			{
				var action = actionPtr.Cast<Action<string>>();
				action(msg);
			}
		}
	}
}
#endif