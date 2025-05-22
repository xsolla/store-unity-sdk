#if UNITY_ANDROID
using System;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Auth
{
	internal class AndroidWidgetAuthenticator : IWidgetAuthenticator
	{
		private readonly Action OnSuccessCallback;
		private readonly Action<Error> OnErrorCallback;
		private readonly Action OnCancelCallback;
		private readonly string Locale;

		public AndroidWidgetAuthenticator(Action onSuccessCallback, Action<Error> onErrorCallback, Action onCancelCallback, string locale)
		{
			OnSuccessCallback = onSuccessCallback;
			OnErrorCallback = onErrorCallback;
			OnCancelCallback = onCancelCallback;
			Locale = locale;
		}

		public void Launch()
		{
			try
			{
				var androidHelper = new AndroidHelper();

				var authCallback = new AndroidAuthCallback(
					androidHelper,
					() => androidHelper.MainThreadExecutor.Enqueue(() => OnSuccessCallback?.Invoke()),
					error => androidHelper.MainThreadExecutor.Enqueue(() => OnErrorCallback(error)),
					() => androidHelper.MainThreadExecutor.Enqueue(OnCancelCallback));

				var proxyActivity = new AndroidJavaObject($"{Application.identifier}.androidProxies.XsollaWidgetAuthProxyActivity");
				proxyActivity.CallStatic(
					"perform",
					androidHelper.CurrentActivity,
					authCallback,
					Locale);
			}
			catch (Exception e)
			{
				throw new AggregateException($"AndroidSocialAuth: {e.Message}", e);
			}
		}
	}
}
#endif