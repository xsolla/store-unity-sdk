using UnityEngine;

namespace Xsolla.Core
{
	public static class XDebug
	{
		private const string TAG = "[Xsolla SDK]";

		public static void Log(object message, bool ignoreLogLevel = false)
		{
			Log(TAG, message, ignoreLogLevel);
		}

		public static void Log(string tag, object message, bool ignoreLogLevel = false)
		{
			if (XsollaSettings.LogLevel == LogLevel.InfoWarningsErrors || ignoreLogLevel)
				Debug.Log($"{tag} {message}");
		}

		public static void LogWarning(object message, bool ignoreLogLevel = false)
		{
			LogWarning(TAG, message, ignoreLogLevel);
		}

		public static void LogWarning(string tag, object message, bool ignoreLogLevel = false)
		{
			if (XsollaSettings.LogLevel <= LogLevel.WarningsErrors || ignoreLogLevel)
				Debug.LogWarning($"{tag} {message}");
		}

		public static void LogError(object message)
		{
			LogError(TAG, message);
		}

		public static void LogError(string tag, object message)
		{
			Debug.LogError($"{tag} {message}");
		}
	}
}