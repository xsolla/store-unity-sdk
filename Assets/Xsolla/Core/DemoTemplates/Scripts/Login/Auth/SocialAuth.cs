using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Login;

public class SocialAuth : StoreStringActionResult, ILoginAuthorization
{
	public void TryAuth(params object[] args)
	{
		if (HotkeyCoroutine.IsLocked())
		{
			base.OnError?.Invoke(null);
			return;
		}

		if (TryExtractProvider(args, out SocialProvider provider))
		{
			HotkeyCoroutine.Lock();

			string url = DemoController.Instance.GetImplementation().GetSocialNetworkAuthUrl(provider);
			Debug.Log($"Social url: {url}");
			BrowserHelper.Instance.Open(url, true);

			var singlePageBrowser = BrowserHelper.Instance.GetLastBrowser();

			Debug.Log($"BROWSER IS NULL: {singlePageBrowser == null}");

			singlePageBrowser.BrowserClosedEvent += _ => BrowserCloseHandler();
			singlePageBrowser.GetComponent<XsollaBrowser>().Navigate.UrlChangedEvent +=
				(browser, newUrl) =>
				{
					if (IsRedirectWithToken(newUrl, out var token))
					{
						Debug.Log($"We take{Environment.NewLine}from URL:{newUrl}{Environment.NewLine}token = {token}");
						StartCoroutine(SuccessAuthCoroutine(token));
					}
				};
		}
		else
		{
			Debug.LogError("SocialAuth.TryAuth: Could not extract argument");
			base.OnError?.Invoke(new Error(errorMessage: "Social auth failed"));
		}
	}

	private bool TryExtractProvider(object[] args, out SocialProvider provider)
	{
		provider = default(SocialProvider);

		if (args == null)
		{
			Debug.LogError("SocialAuth.TryExtractProvider: 'object[] args' was null");
			return false;
		}

		if (args.Length != 1)
		{
			Debug.LogError($"SocialAuth.TryExtractProvider: args.Length expected 1, was {args.Length}");
			return false;
		}

		try
		{
			provider = (SocialProvider)args[0];
		}
		catch (Exception ex)
		{
			Debug.LogError($"SocialAuth.TryExtractProvider: Error during argument extraction: {ex.Message}");
			return false;
		}

		return true;
	}

	private void BrowserCloseHandler()
	{
		HotkeyCoroutine.Unlock();
		base.OnError?.Invoke(null);
	}

	private IEnumerator SuccessAuthCoroutine(string token)
	{
		yield return new WaitForEndOfFrame();
		Destroy(BrowserHelper.Instance.gameObject);
		BrowserCloseHandler();
		base.OnSuccess?.Invoke(token);
	}

	private static bool IsRedirectWithToken(string newUrl, out string token)
	{
		var regex = new Regex(@"[&?]token=\S*");
		token = regex.Match(newUrl).Value.Replace("?token=", string.Empty).Replace("&token=", string.Empty);
		return !string.IsNullOrEmpty(token);
	}
}
