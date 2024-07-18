#if UNITY_ANDROID

namespace Xsolla.DevTools
{
	public static class ApiLevelProcessor
	{
		public static void Process()
		{
#if UNITY_6000_0_OR_NEWER
			UnityEditor.PlayerSettings.Android.minSdkVersion = UnityEditor.AndroidSdkVersions.AndroidApiLevel24;
			UnityEditor.PlayerSettings.Android.targetSdkVersion = UnityEditor.AndroidSdkVersions.AndroidApiLevel34;
#endif
		}
	}
}
#endif