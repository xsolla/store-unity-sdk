using System;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class BasicAuth : LoginAuthorization
	{
		private const string DEMO_USER_NAME = "XSOLLA";
		private bool _isDemoUser;
		private bool _isJwtInvalidationEnabled;

		public override void TryAuth(params object[] args)
		{
			string username; string password; bool rememberMe;
			if (TryExtractArgs(args, out username, out password, out rememberMe))
			{
				_isDemoUser = (username.ToUpper() == DEMO_USER_NAME && password.ToUpper() == DEMO_USER_NAME);
				_isJwtInvalidationEnabled = XsollaSettings.JwtTokenInvalidationEnabled;

				if(_isDemoUser && _isJwtInvalidationEnabled)
					XsollaSettings.JwtTokenInvalidationEnabled = false;

				SdkLoginLogic.Instance.SignIn(username, password, rememberMe, BasicAuthSuccess, BasicAuthFailed);
			}
			else
			{
				Debug.LogError("BasicAuth.TryAuth: Could not extract arguments for SignIn");
				if (base.OnError != null)
					base.OnError.Invoke(new Error(errorMessage: "Basic auth failed"));
			}
		}

		private bool TryExtractArgs(object[] args, out string username, out string password, out bool rememberMe)
		{
			username = default(string);
			password = default(string);
			rememberMe = default(bool);

			if (args == null)
			{
				Debug.LogError("BasicAuth.TryExtractArgs: 'object[] args' was null");
				return false;
			}

			if (args.Length != 3)
			{
				Debug.LogError(string.Format("BasicAuth.TryExtractArgs: args.Length expected 3, was {}", args.Length));
				return false;
			}

			try
			{
				username = (string)args[0];
				password = (string)args[1];
				rememberMe = (bool)args[2];
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("BasicAuth.TryExtractArgs: Error during argument extraction: {}", ex.Message));
				return false;
			}

			return true;
		}

		private void BasicAuthSuccess(string token)
		{
			RestoreJwtInvalidationIfNeeded();
			if (base.OnSuccess != null)
				base.OnSuccess.Invoke(Token.Instance);
		}

		private void BasicAuthFailed(Error error)
		{
			RestoreJwtInvalidationIfNeeded();
			Debug.LogWarning(string.Format("BasicAuth: auth failed. Error: {0}", error.errorMessage));
			if (base.OnError != null)
				base.OnError.Invoke(error);
		}

		private void RestoreJwtInvalidationIfNeeded()
		{
			if(_isDemoUser && _isJwtInvalidationEnabled)
				XsollaSettings.JwtTokenInvalidationEnabled = true;
		}
	}
}
