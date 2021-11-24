using System;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class AccessTokenAuth : LoginAuthorization
	{
		public override void TryAuth(params object[] args)
		{
			string email;
			if (TryExtractArgs(args, out email))
			{
				SdkLoginLogic.Instance.AccessTokenAuth(email, AccessTokenAuthSuccess, AccessTokenAuthFailed);
			}
			else
			{
				Debug.LogError("AccessTokenAuth.TryAuth: Could not extract arguments for AccessTokenAuth");
				if (base.OnError != null)
					base.OnError.Invoke(new Error(errorMessage: "Basic auth failed"));
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
				Debug.LogError(string.Format("AccessTokenAuth.TryExtractArgs: args.Length expected 1, was {0}", args.Length));
				return false;
			}

			try
			{
				email = (string)args[0];
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("AccessTokenAuth.TryExtractArgs: Error during argument extraction: {0}", ex.Message));
				return false;
			}

			return true;
		}

		private void AccessTokenAuthSuccess()
		{
			if (base.OnSuccess != null)
				base.OnSuccess.Invoke(Token.Instance);
		}

		private void AccessTokenAuthFailed(Error error)
		{
			Debug.LogWarning(string.Format("AccessTokenAuth: auth failed. Error: {0}", error.errorMessage));
			if (base.OnError != null)
				base.OnError.Invoke(error);
		}
	}
}
