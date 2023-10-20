using System;
using UnityEngine;

namespace Xsolla.Core
{
	internal class AndroidSocialAuth
	{
		public void Perform(SocialProvider provider, Action onSuccess, Action<Error> onError, Action onCancel)
		{
			try
			{
				var helper = new AndroidHelper();

				var providerName = provider.ToString().ToUpper();
				var socialNetworkClass = new AndroidJavaClass("com.xsolla.android.login.social.SocialNetwork");
				var socialNetworkObject = socialNetworkClass.GetStatic<AndroidJavaObject>(providerName);

				var callback = new AndroidAuthCallback(
					token => helper.MainThreadExecutor.Enqueue(() =>
					{
						XsollaToken.Create(token);
						onSuccess?.Invoke();
					}),
					error => helper.MainThreadExecutor.Enqueue(() => onError(error)),
					() => helper.MainThreadExecutor.Enqueue(onCancel));

				var proxyActivity = new AndroidJavaObject($"{Application.identifier}.androidProxies.SocialAuthProxyActivity");
				proxyActivity.CallStatic(
					"perform",
					helper.CurrentActivity,
					socialNetworkObject,
					callback);
			}
			catch (Exception e)
			{
				throw new AggregateException($"AndroidSocialAuth: {e.Message}", e);
			}
		}
	}
}