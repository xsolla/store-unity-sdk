using System;
using UnityEngine;

namespace Xsolla.Core
{
	public class AndroidXsollaWidgetAuth
	{
		public void Perform(Action onSuccess, Action<Error> onError, Action onCancel)
		{
			try
			{
				var helper = new AndroidHelper();
				
				var callback = new AndroidAuthCallback(
					token => helper.MainThreadExecutor.Enqueue(() =>
					{
						XsollaToken.Create(token);
						onSuccess?.Invoke();
					}),
					error => helper.MainThreadExecutor.Enqueue(() => onError(error)),
					() => helper.MainThreadExecutor.Enqueue(onCancel));

				var proxyActivity = new AndroidJavaObject($"{Application.identifier}.androidProxies.XsollaWidgetAuthProxyActivity");
				proxyActivity.CallStatic(
					"perform",
					helper.CurrentActivity,
					callback);
			}
			catch (Exception e)
			{
				throw new AggregateException($"AndroidSocialAuth: {e.Message}", e);
			}
		}
	}
}