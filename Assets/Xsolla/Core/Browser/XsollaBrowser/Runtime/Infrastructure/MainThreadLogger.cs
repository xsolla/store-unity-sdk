using Xsolla.Core;

namespace Xsolla.XsollaBrowser
{
	public class MainThreadLogger
	{
		private const string TAG = "[Xsolla BROWSER]";

		private readonly MainThreadExecutor MainThreadExecutor;

		public MainThreadLogger(MainThreadExecutor mainThreadExecutor)
		{
			MainThreadExecutor = mainThreadExecutor;
		}

		public void Log(string message)
		{
			if (MainThreadExecutor)
				MainThreadExecutor.Enqueue(() => XDebug.Log(TAG, message));
		}
	}
}