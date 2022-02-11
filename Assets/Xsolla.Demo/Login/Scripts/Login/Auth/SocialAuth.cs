using System;
using System.Collections;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class SocialAuth : LoginAuthorization
	{
		public override void TryAuth(params object[] args)
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

				string url = SdkAuthLogic.Instance.GetSocialNetworkAuthUrl(provider);
				Debug.Log($"Social url: {url}");

				var browser = BrowserHelper.Instance.InAppBrowser;
				browser.Open(url);
				browser.AddCloseHandler(BrowserCloseHandler);
				browser.AddUrlChangeHandler(UrlChangedHandler);
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

		private void UrlChangedHandler(string newUrl)
		{
			if (XsollaSettings.AuthorizationType == AuthorizationType.JWT && ParseUtils.TryGetValueFromUrl(newUrl, ParseParameter.token, out var token))
			{
				Debug.Log($"We take{Environment.NewLine}from URL:{newUrl}{Environment.NewLine}token = {token}");
				StartCoroutine(SuccessAuthCoroutine(token));
			}
			else if (XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0 && ParseUtils.TryGetValueFromUrl(newUrl, ParseParameter.code, out var code))
			{
				Debug.Log($"We take{Environment.NewLine}from URL:{newUrl}{Environment.NewLine}code = {code}");
				SdkAuthLogic.Instance.ExchangeCodeToToken(
					code,
					onSuccessExchange: socialToken => StartCoroutine(SuccessAuthCoroutine(socialToken)),
					onError: error => { Debug.LogError(error.errorMessage); base.OnError?.Invoke(error); }
					);
			}
		}

		private IEnumerator SuccessAuthCoroutine(string token)
		{
			yield return new WaitForEndOfFrame();

			if (EnvironmentDefiner.IsStandaloneOrEditor)
			{
				BrowserHelper.Instance.Close();
				HotkeyCoroutine.Unlock();
			}

			base.OnSuccess?.Invoke(token);
		}
	}
}
