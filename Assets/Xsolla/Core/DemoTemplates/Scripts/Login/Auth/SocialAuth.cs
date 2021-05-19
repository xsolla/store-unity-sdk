using System;
using System.Collections;
using UnityEngine;
using Xsolla.Core;

public class SocialAuth : StoreStringActionResult, ILoginAuthorization
{
	public void TryAuth(params object[] args)
	{
#if UNITY_EDITOR || UNITY_STANDALONE
		if (HotkeyCoroutine.IsLocked())
		{
			if (base.OnError != null)
				base.OnError.Invoke(null);
			return;
		}

		SocialProvider provider;
		if (TryExtractProvider(args, out provider))
		{
			HotkeyCoroutine.Lock();

			string url = DemoController.Instance.GetImplementation().GetSocialNetworkAuthUrl(provider);
			Debug.Log(string.Format("Social url: {0}", url));
			BrowserHelper.Instance.Open(url, true);

			var singlePageBrowser = BrowserHelper.Instance.GetLastBrowser();

			if (singlePageBrowser == null)
			{
				Debug.LogError("Browser is null");
				if (base.OnError != null)
					base.OnError.Invoke(Error.UnknownError);
				return;
			}

			singlePageBrowser.BrowserClosedEvent += _ => BrowserCloseHandler();
			singlePageBrowser.GetComponent<XsollaBrowser>().Navigate.UrlChangedEvent += UrlChangedHandler;
		}
		else
		{
			Debug.LogError("SocialAuth.TryAuth: Could not extract argument");
			if (base.OnError != null)
				base.OnError.Invoke(new Error(errorMessage: "Social auth failed"));
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
			Debug.LogError(string.Format("SocialAuth.TryExtractProvider: args.Length expected 1, was {0}", args.Length));
			return false;
		}

		try
		{
			provider = (SocialProvider)args[0];
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("SocialAuth.TryExtractProvider: Error during argument extraction: {0}", ex.Message));
			return false;
		}

		return true;
	}


	private void BrowserCloseHandler()
	{
		HotkeyCoroutine.Unlock();
		if (base.OnError != null)
			base.OnError.Invoke(null);
	}

	private void UrlChangedHandler(IXsollaBrowser browser, string newUrl)
	{
		string token;
		string code;
		if (XsollaSettings.AuthorizationType == AuthorizationType.JWT && ParseUtils.TryGetValueFromUrl(newUrl, ParseParameter.token, out token))
		{
			Debug.Log(string.Format("We take{0}from URL:{1}{2}token = {3}", Environment.NewLine, newUrl, Environment.NewLine, token));
			StartCoroutine(SuccessAuthCoroutine(token));
		}
		else if (XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0 && ParseUtils.TryGetValueFromUrl(newUrl, ParseParameter.code, out code))
		{
			Debug.Log(string.Format("We take{0}from URL:{1}{2}code = {3}", Environment.NewLine, newUrl, Environment.NewLine, code));
			DemoController.Instance.GetImplementation().ExchangeCodeToToken(
				code,
				onSuccessExchange: socialToken => StartCoroutine(SuccessAuthCoroutine(socialToken)),
				onError: error =>
				{
					Debug.LogError(error.errorMessage);
					if (base.OnError != null)
						base.OnError.Invoke(error);
				});
		}
	}

	private IEnumerator SuccessAuthCoroutine(string token)
	{
		yield return new WaitForEndOfFrame();
#if UNITY_EDITOR || UNITY_STANDALONE
		Destroy(BrowserHelper.Instance.gameObject);
		HotkeyCoroutine.Unlock();
#endif
		if (base.OnSuccess != null)
			base.OnSuccess.Invoke(token);
	}
}
