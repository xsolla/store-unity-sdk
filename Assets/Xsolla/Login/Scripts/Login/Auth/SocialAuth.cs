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
				if (base.OnError != null)
					base.OnError.Invoke(null);
				return;
			}

			SocialProvider provider;
			if (TryExtractProvider(args, out provider))
			{
				HotkeyCoroutine.Lock();

				string url = SdkLoginLogic.Instance.GetSocialNetworkAuthUrl(provider);
				Debug.Log(string.Format("Social url: {0}", url));

				var browser = BrowserHelper.Instance.InAppBrowser;
				browser.Open(url);
				browser.AddCloseHandler(BrowserCloseHandler);
				browser.AddUrlChangeHandler(UrlChangedHandler);
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

		private void UrlChangedHandler(string newUrl)
		{
			string token;
			string code;
			if (XsollaSettings.AuthorizationType == AuthorizationType.JWT && ParseUtils.TryGetValueFromUrl(newUrl, ParseParameter.token, out token))
			{
				Debug.Log(string.Format("We take{0}from URL:{1}{0}token = {2}", Environment.NewLine, newUrl, token));
				StartCoroutine(SuccessAuthCoroutine(token));
			}
			else if (XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0 && ParseUtils.TryGetValueFromUrl(newUrl, ParseParameter.code, out code))
			{
				Debug.Log(string.Format("We take{0}from URL:{1}{0}code = {2}", Environment.NewLine, newUrl, code));
				SdkLoginLogic.Instance.ExchangeCodeToToken(
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

			if (EnvironmentDefiner.IsStandaloneOrEditor)
			{
				BrowserHelper.Instance.Close();
				HotkeyCoroutine.Unlock();
			}

			if (base.OnSuccess != null)
				base.OnSuccess.Invoke(token);
		}
	}
}
