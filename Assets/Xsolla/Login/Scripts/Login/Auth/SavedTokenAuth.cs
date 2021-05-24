using System.Collections;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class SavedTokenAuth : StoreStringActionResult, ILoginAuthorization
	{
		public void TryAuth(params object[] args)
		{
			StartCoroutine(LoadToken());
		}

		private IEnumerator LoadToken()
		{
			if (XsollaSettings.AuthorizationType == AuthorizationType.OAuth2_0)
				yield return new WaitWhile(() => DemoController.Instance.LoginDemo.IsOAuthTokenRefreshInProgress);

			if (DemoController.Instance.LoginDemo.LoadToken(Constants.LAST_SUCCESS_AUTH_TOKEN, out var token))
			{
				Debug.Log("SavedTokenAuth.TryAuth: Token loaded");
				base.OnSuccess?.Invoke(token);
			}
			else
			{
				Debug.Log("SavedTokenAuth.TryAuth: No token");
				base.OnError?.Invoke(null);
			}
		}
	}
}
