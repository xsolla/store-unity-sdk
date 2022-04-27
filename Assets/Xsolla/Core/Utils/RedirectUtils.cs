namespace Xsolla.Core
{
	public static class RedirectUtils
	{
		public static string GetRedirectUrl()
		{
			if (!string.IsNullOrEmpty(XsollaSettings.CallbackUrl))
				return XsollaSettings.CallbackUrl;
			else
				return Constants.DEFAULT_REDIRECT_URL;
		}

		public static string GetRedirectUrl(string redirectArg)
		{
			if (!string.IsNullOrEmpty(redirectArg))
				return redirectArg;
			else
				return GetRedirectUrl();
		}
	}
}