using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;

public abstract class SocialAuth : MonoBehaviour, ILoginAuthorization
{
	public Action<string> OnSuccess { get; set; }
	public Action OnFailed { get; set; }

	protected abstract SocialProvider GetSocialProvider();
	protected virtual bool InvalidateUserOldJwt() => false;
	
	private void Start()
	{
		var provider = GetSocialProvider();
		var invalidate = InvalidateUserOldJwt();
		SocialNetworkAuth(provider, invalidate);
	}

	private void SocialNetworkAuth(SocialProvider socialProvider, bool invalidateOtherTokens = false)
	{
		int port = GetFreePort();
		string url = XsollaLogin.Instance.GetSocialNetworkAuthUrl(socialProvider, port, invalidateOtherTokens);
		
		BrowserHelper.Instance.Open(url, true);
		BrowserHelper.Instance.GetLastBrowser().GetComponent<XsollaBrowser>().Navigate.UrlChangedEvent +=
			(browser, newUrl) =>
			{
				if (!IsLocalhostRedirect(newUrl) || !IsRedirectWithToken(newUrl, out var token))
				{
					OnFailed?.Invoke();
					return;
				}
				if (!newUrl.Contains(":" + port))
				{
					// At this place we not return and we not use onError, because it is Demo project
					Debug.LogError($"Redirect to URL = {newUrl}, but port must {port}!");
				}
				OnSuccess?.Invoke(token);
			};
	}

	private static bool IsLocalhostRedirect(string newUrl)
	{
		return newUrl.Contains("localhost");
	}

	private static bool IsRedirectWithToken(string newUrl, out string token)
	{
		token = ParseUtils.ParseToken(newUrl);
		return string.IsNullOrEmpty(token);
	}

	private static int GetFreePort()
	{
		var c = new UdpClient(0);
		var port = ((IPEndPoint) c.Client.LocalEndPoint).Port;
		c.Dispose();
		return port;
	}
}
