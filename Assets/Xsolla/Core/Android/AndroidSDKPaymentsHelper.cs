using System;
using UnityEngine;
using Xsolla.Core.Android;

namespace Xsolla.Core
{
	public class AndroidSDKPaymentsHelper : IDisposable
	{
		public void PerformPayment(string token, bool isSandbox)
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

				var activity = new AndroidHelper().CurrentActivity;
				proxyClass.CallStatic("performPayment", activity, token, isSandbox, redirectScheme, redirectHost);
			}
			catch (Exception e)
			{
				throw new AggregateException($"AndroidSDKPaymentsHelper.PerformPayment: {e.Message}", e);
			}
		}

		public void Dispose()
		{
			return;
		}
	}
}