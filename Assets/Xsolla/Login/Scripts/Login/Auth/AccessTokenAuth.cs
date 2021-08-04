using System;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class AccessTokenAuth : StoreStringActionResult, ILoginAuthorization
	{
		public void TryAuth(params object[] args)
		{
			if (TryExtractArgs(args, out string email))
			{
				DemoController.Instance.LoginDemo.AccessTokenAuth(email, AccessTokenAuthSuccess, AccessTokenAuthFailed);
			}
			else
			{
				Debug.LogError("AccessTokenAuth.TryAuth: Could not extract arguments for AccessTokenAuth");
				base.OnError?.Invoke(new Error(errorMessage: "Basic auth failed"));
			}
		}

		private bool TryExtractArgs(object[] args, out string email)
		{
			email = default(string);

			if (args == null)
			{
				Debug.LogError("AccessTokenAuth.TryExtractArgs: 'object[] args' was null");
				return false;
			}

			if (args.Length != 1)
			{
				Debug.LogError($"AccessTokenAuth.TryExtractArgs: args.Length expected 1, was {args.Length}");
				return false;
			}

			try
			{
				email = (string)args[0];
			}
			catch (Exception ex)
			{
				Debug.LogError($"AccessTokenAuth.TryExtractArgs: Error during argument extraction: {ex.Message}");
				return false;
			}

			return true;
		}

		private void AccessTokenAuthSuccess()
		{
			base.OnSuccess?.Invoke(Token.Instance);
		}

		private void AccessTokenAuthFailed(Error error)
		{
			Debug.LogWarning($"AccessTokenAuth: auth failed. Error: {error.errorMessage}");
			base.OnError?.Invoke(error);
		}
	}
}
