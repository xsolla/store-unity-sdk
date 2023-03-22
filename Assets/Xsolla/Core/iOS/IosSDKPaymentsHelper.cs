#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Xsolla.Core
{
	public class IosSDKPaymentsHelper
	{
		[DllImport("__Internal")]
		private static extern void _performPayment(string token, bool isSandbox, string redirectUrl,
			IosCallbacks.ActionStringCallbackDelegate onErrorCallback, IntPtr onErrorActionPtr,
			IosCallbacks.ActionBoolCallbackDelegate browserCallback, IntPtr browserCallbackActionPtr);

		private Action<bool> OnBrowserClosed { get; set; }

		public void PerformPayment(string token, bool isSandbox, Action<bool> onBrowserClosed = null)
		{
			OnBrowserClosed = onBrowserClosed;

			var redirectPolicy = XsollaSettings.IosRedirectPolicySettings;
			var redirectUrl = redirectPolicy.UseSettingsFromPublisherAccount
				? $"app://xpayment.{Application.identifier}"
				: redirectPolicy.ReturnUrl;

			Action<string> onErrorNative = FailHandler;
			Action<bool> onBrowserClosedNative = HandleBrowserClosed;
			_performPayment(token, isSandbox, redirectUrl, IosCallbacks.ActionStringCallback, onErrorNative.GetPointer(), IosCallbacks.ActionBoolCallback, onBrowserClosedNative.GetPointer());
		}

		private static void FailHandler(string error)
		{
			Debug.LogError($"Payments failed. Error: {error}");
		}

		private void HandleBrowserClosed(bool isManually)
		{
			OnBrowserClosed?.Invoke(isManually);
		}
	}
}
#endif