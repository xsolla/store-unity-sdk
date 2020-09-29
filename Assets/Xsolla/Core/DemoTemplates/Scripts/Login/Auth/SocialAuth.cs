using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using Xsolla.Core;

public class SocialAuth : StoreStringActionResult, ILoginAuthorization
{
	public void TryAuth(params object[] args)
	{
#if UNITY_EDITOR || UNITY_STANDALONE
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

			if (singlePageBrowser == null)
			{
				Debug.LogError("Browser is null");
				base.OnError?.Invoke(Error.UnknownError);
				return;
			}

			singlePageBrowser.BrowserClosedEvent += _ => BrowserCloseHandler();
			singlePageBrowser.GetComponent<XsollaBrowser>().Navigate.UrlChangedEvent += UrlChangedHandler;
		}
		else
		{
			Debug.LogError("SocialAuth.TryAuth: Could not extract argument");
			base.OnError?.Invoke(new Error(errorMessage: "Social auth failed"));
		}
#endif
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

	private void UrlChangedHandler(IXsollaBrowser browser, string newUrl)
	{
		if (XsollaSettings.AuthorizationType == AuthorizationType.JWT && ParseUtils.TryGetValueFromUrl(newUrl, ParseParameter.token, out var token))
		{
			Debug.Log($"We take{Environment.NewLine}from URL:{newUrl}{Environment.NewLine}token = {token}");
			StartCoroutine(SuccessAuthCoroutine(token));
		}
		else if (XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0 && ParseUtils.TryGetValueFromUrl(newUrl, ParseParameter.code, out var code))
		{
			Debug.Log($"We take{Environment.NewLine}from URL:{newUrl}{Environment.NewLine}code = {code}");
			DemoController.Instance.GetImplementation().ExchangeCodeToToken(
				code,
				onSuccessExchange: socialToken => StartCoroutine(SuccessAuthCoroutine(socialToken)),
				onError: error => { Debug.LogError(error.errorMessage); base.OnError?.Invoke(error); }
				);
		}
	}

	private IEnumerator SuccessAuthCoroutine(string token)
	{
		yield return new WaitForEndOfFrame();
#if UNITY_EDITOR || UNITY_STANDALONE
		Destroy(BrowserHelper.Instance.gameObject);
		HotkeyCoroutine.Unlock();
#endif
		base.OnSuccess?.Invoke(token);
	}
}
