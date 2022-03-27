using System.Collections;
using UnityEngine;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class SavedTokenAuth : LoginAuthorization
	{
		public override void TryAuth(params object[] args)
		{
			if (Token.Load())
			{
				Debug.Log("SavedTokenAuth.TryAuth: Token loaded, validating");
				SdkAuthLogic.Instance.ValidateToken(
					token: Token.Instance,
					onSuccess: _ => base.OnSuccess?.Invoke(Token.Instance),
					onError: HandleError);
			}
			else
			{
				Debug.Log("SavedTokenAuth.TryAuth: No token");
				base.OnError?.Invoke(null);
			}
		}

		private void HandleError(Error error)
		{
			Debug.Log($"SavedTokenAuth.TryAuth: Error occured while validating: {error.ErrorType}");

			if (error.ErrorType == ErrorType.InvalidToken && XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0)
			{
				Debug.Log($"SavedTokenAuth.TryAuth: Trying to refresh OAuth token");
				SdkAuthLogic.Instance.RefreshOAuthToken(
					onSuccess: newToken => base.OnSuccess?.Invoke(newToken),
					onError: refreshError =>
					{
						Debug.LogError(error.errorMessage);
						base.OnError?.Invoke(null);
					});
			}
			else
				base.OnError?.Invoke(null);
		}
	}
}
