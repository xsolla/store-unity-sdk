using System;
using Xsolla.Core;

namespace Xsolla.Auth
{
	internal static class LocalAuthServer
	{
		private static LocalAuthHttpListener HttpListener;

		public static void Start(string redirectUrl, Action successCallback, Action<Error> errorCallback)
		{
			HttpListener?.Stop();
			HttpListener = new LocalAuthHttpListener(redirectUrl, successCallback, errorCallback);
			HttpListener.Start();
		}
	}
}