using System;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class XsollaAuthService : IAuthService
	{
		public void ClearSavedData()
		{
			XsollaToken.DeleteSavedInstance();
		}

		public void AuthBuySavedData(Action onSuccess, Action onError)
		{
			XsollaAuth.AuthBySavedToken(
				onSuccess,
				_ => onError?.Invoke());
		}

		public void SignIn(string username, string password, Action onSuccess, Action<string> onError)
		{
			XsollaAuth.SignIn(
				username,
				password,
				() => onSuccess?.Invoke(),
				error => onError?.Invoke(error.errorMessage));
		}

		public void Register(string username, string email, string password, Action onSuccess, Action<string> onError)
		{
			XsollaAuth.Register(
				username,
				password,
				email,
				_ => onSuccess?.Invoke(),
				error => onError?.Invoke(error.errorMessage));
		}

		public void AuthViaSocialNetwork(string socialNetwork, Action onSuccess, Action<string> onError, Action onCancel)
		{
			var provider = socialNetwork switch {
				"google"   => SocialProvider.Google,
				"facebook" => SocialProvider.Facebook,
				_          => throw new Exception($"Unsupported social provider: {socialNetwork}")
			};

			XsollaAuth.AuthViaSocialNetwork(
				provider,
				() => { onSuccess?.Invoke(); },
				error => { onError?.Invoke(error.errorMessage); },
				() => { onCancel?.Invoke(); });
		}
	}
}