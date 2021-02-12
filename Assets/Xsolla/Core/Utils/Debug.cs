using System;

namespace Xsolla.Core
{
	public class Debug
	{
		public static void Log(object message)
		{
			if (XsollaSettings.LogLevel == LogLevel.InfoWarningsErrors)
				UnityEngine.Debug.Log(message);
		}

		public static void LogWarning(object message)
		{
			if (XsollaSettings.LogLevel <= LogLevel.WarningsErrors)
				UnityEngine.Debug.LogWarning(message);
		}

		public static void LogError(object message)
		{
			UnityEngine.Debug.LogError(message);
		}

		public static void LogAssertion(object message)
		{
			UnityEngine.Debug.LogAssertion(message);
		}

		public static void LogException(Exception exception)
		{
			UnityEngine.Debug.LogException(exception);
		}
	}
}

namespace Xsolla.Demo
{
	public class Debug : Xsolla.Core.Debug { }
}

namespace Xsolla.Login
{
	public class Debug : Xsolla.Core.Debug { }
}

namespace Xsolla.Store
{
	public class Debug : Xsolla.Core.Debug { }
}
