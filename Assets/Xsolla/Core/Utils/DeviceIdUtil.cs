using System;
using UnityEngine;

namespace Xsolla.Core
{
	public static class DeviceIdUtil
	{
		public static Exception GetDeviceInfo(DeviceType deviceType, out string deviceId, out string deviceName, out string deviceModel)
		{
			deviceName = SystemInfo.deviceName;
			deviceModel = SystemInfo.deviceModel;
			var exception = GetDeviceID(deviceType, out deviceId);
			return exception;
		}

		public static Exception GetDeviceID(DeviceType deviceType, out string deviceID)
		{
			deviceID = default(string);
			Exception exception = null;
#if UNITY_ANDROID
			try
			{
				AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject currentActivity = player.GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
				AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
				deviceID = secure.CallStatic<string>("getString", contentResolver, "android_id");
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				exception = ex;
			}
#else
			deviceID = SystemInfo.deviceUniqueIdentifier;
#endif
			return exception;
		}
	}
}
