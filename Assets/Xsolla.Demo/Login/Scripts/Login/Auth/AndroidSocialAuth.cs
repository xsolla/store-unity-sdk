using System;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class AndroidSocialAuth : LoginAuthorization
	{
		public override void TryAuth(params object[] args)
		{
			if (TryExtractProvider(args, out SocialProvider socialProvider))
			{
				try
				{
					using (var sdkHelper = new AndroidSDKSocialAuthHelper())
					{
						sdkHelper.PerformSocialAuth(socialProvider,
						onSuccess: token => OnSocialAuthResult(socialProvider, SocialAuthResult.SUCCESS, token: token),
						onCancelled: () => OnSocialAuthResult(socialProvider, SocialAuthResult.CANCELLED),
						onError: error => OnSocialAuthResult(socialProvider, SocialAuthResult.ERROR, error: error));
					}

					Debug.Log("AndroidSocialAuth.SocialNetworkAuth: auth request was sent");
				}
				catch (Exception ex)
				{
					Debug.LogError($"AndroidSocialAuth.SocialNetworkAuth: {ex.Message}");
					base.OnError?.Invoke(new Error(errorMessage: "Social auth failed"));
				}
			}
			else
			{
				Debug.LogWarning("AndroidSocialAuth.TryAuth: Could not extract argument");
				base.OnError?.Invoke(new Error(errorMessage: "Social auth failed"));
			}
		}

		private bool TryExtractProvider(object[] args, out SocialProvider provider)
		{
			provider = default(SocialProvider);

			if (args == null)
			{
				Debug.LogError("AndroidSocialAuth.TryExtractProvider: 'object[] args' was null");
				return false;
			}

			if (args.Length != 1)
			{
				Debug.LogError($"AndroidSocialAuth.TryExtractProvider: args.Length expected 1, was {args.Length}");
				return false;
			}

			try
			{
				provider = (SocialProvider)args[0];
			}
			catch (Exception ex)
			{
				Debug.LogError($"AndroidSocialAuth.TryExtractProvider: Error during argument extraction: {ex.Message}");
				return false;
			}

			return true;
		}


		private void OnSocialAuthResult(SocialProvider socialProvider, SocialAuthResult authResult, string token = null, Error error = null)
		{
			Debug.Log($"AndroidSocialAuth.OnSocialAuthResult: processing auth result for {socialProvider}");

			var logHeader = $"AndroidSocialAuth.OnSocialAuthResult: authResult for {socialProvider} returned";

			switch (authResult)
			{
				case SocialAuthResult.SUCCESS:
					Debug.Log($"{logHeader} SUCCESS. Token: {token}");
					base.OnSuccess?.Invoke(token);
					break;
				case SocialAuthResult.CANCELLED:
					Debug.Log($"{logHeader} CANCELLED.");
					base.OnError?.Invoke(null);
					break;
				case SocialAuthResult.ERROR:
					Debug.LogError($"{logHeader} ERROR. Error message: {error.errorMessage}");
					base.OnError?.Invoke(error);
					break;
				default:
					Debug.LogError($"{logHeader} unexpected authResult.");
					base.OnError?.Invoke(new Error(errorMessage: "Social auth failed"));
					break;
			}
		}

		private enum SocialAuthResult
		{
			SUCCESS, CANCELLED, ERROR
		}
	}
}