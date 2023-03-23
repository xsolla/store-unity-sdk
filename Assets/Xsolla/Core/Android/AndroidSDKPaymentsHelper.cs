using System;
using UnityEngine;
using Xsolla.Core.Android;

namespace Xsolla.Core
{
	public class AndroidSDKPaymentsHelper : IDisposable
	{
		private readonly AndroidHelper androidHelper;

		public AndroidSDKPaymentsHelper()
		{
			androidHelper = new AndroidHelper();
		}

		public void PerformPayment(string token, bool isSandbox, Action<bool> browserClosedCallback)
		{
			try
			{
				var proxyClass = new AndroidJavaClass($"{Application.identifier}.androidProxies.AndroidPaymentsProxy");

				string redirectScheme = null;
				string redirectHost = null;
				var redirectPolicy = XsollaSettings.AndroidRedirectPolicySettings;
				if (!redirectPolicy.UseSettingsFromPublisherAccount && !string.IsNullOrEmpty(redirectPolicy.ReturnUrl))
				{
					var uri = new Uri(redirectPolicy.ReturnUrl);
					redirectScheme = uri.Scheme;
					redirectHost = uri.Host;
				}

				var browserCallback = new AndroidSDKBrowserCallback {
					OnBrowserClosed = isManually => androidHelper.OnMainThreadExecutor.Enqueue(() => browserClosedCallback?.Invoke(isManually))
				};

				var activity = new AndroidHelper().CurrentActivity;
				proxyClass.CallStatic("performPayment", activity, token, isSandbox, redirectScheme, redirectHost, browserCallback);
			}
			catch (Exception e)
			{
				throw new AggregateException($"AndroidSDKPaymentsHelper.PerformPayment: {e.Message}", e);
			}
		}

		public void Dispose()
		{
			androidHelper.Dispose();
		}
	}
}