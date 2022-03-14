#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Xsolla.Core.iOS;

namespace Xsolla.Core
{
	public class IosSDKPaymentsHelper 
	{
		[DllImport("__Internal")]
		private static extern void _performPayment(string token, bool isSandbox, string redirectUrl,
			IosCallbacks.ActionStringCallbackDelegate onErrorCallback, IntPtr onErrorActionPtr);

		public void PerformPayment(string token, bool isSandbox)
		{
			var redirectPolicy = XsollaSettings.IosRedirectPolicySettings;
			var redirectUrl = redirectPolicy.UseSettingsFromPublisherAccount
				? $"app://xpayment.{Application.identifier}"
				: redirectPolicy.ReturnUrl;

			Action<string> onErrorNative = FailHandler;
			_performPayment(token, isSandbox, redirectUrl, IosCallbacks.ActionStringCallback, onErrorNative.GetPointer());
		}
		
		private void FailHandler(string error)
		{
			Debug.LogError($"Payments failed. Error: {error}");
		}
	}
}
#endif