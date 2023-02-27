using System;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class IosSocialAuth : LoginAuthorization
	{
		public override void TryAuth(params object[] args)
		{
			
#if UNITY_IOS
			if (TryExtractProvider(args, out SocialProvider provider))
			{
				try
				{
					new IosSDKSocialAuthHelper().PerformSocialAuth(provider, SuccessHandler, CancelHandler, FailHandler);
					Debug.Log("IosSocialAuth.SocialNetworkAuth: auth request was sent");
				}
				catch (Exception ex)
				{
					Debug.LogError($"IosSocialAuth.SocialNetworkAuth: {ex.Message}");
					base.OnError?.Invoke(new Error(errorMessage: "Social auth failed"));
				}
			}
			else
			{
				Debug.LogError("IosSocialAuth.TryAuth: Could not extract argument");
				base.OnError?.Invoke(new Error(errorMessage: "Social auth failed"));
			}
#else
			Debug.Log("iOS social network auth is not supported for this platform");
#endif
		}

		private bool TryExtractProvider(object[] args, out SocialProvider provider)
		{
			provider = default(SocialProvider);

			if (args == null)
			{
				Debug.LogError("IosSocialAuth.TryExtractProvider: 'object[] args' was null");
				return false;
			}

			if (args.Length != 1)
			{
				Debug.LogError($"IosSocialAuth.TryExtractProvider: args.Length expected 1, was {args.Length}");
				return false;
			}

			try
			{
				provider = (SocialProvider) args[0];
			}
			catch (Exception ex)
			{
				Debug.LogError($"IosSocialAuth.TryExtractProvider: Error during argument extraction: {ex.Message}");
				return false;
			}

			return true;
		}

		private void SuccessHandler(LoginOAuthJsonResponse response)
		{
			Debug.Log($"IosSocialAuth.SuccessHandler: Token received: {response.access_token}");
			base.OnSuccess?.Invoke(response.access_token);
		}

		private void FailHandler(Error error)
		{
			Debug.Log("IosSocialAuth.SuccessHandler: Social auth failed");
			base.OnError?.Invoke(error);
		}

		private void CancelHandler()
		{
			Debug.Log("IosSocialAuth.SuccessHandler: Social auth cancelled");
			base.OnError?.Invoke(null);
		}
	}
}