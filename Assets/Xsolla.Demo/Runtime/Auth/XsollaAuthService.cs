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

		public void SignInTouristMode(Action onSuccess, Action<string> onError)
		{
			WebRequestHelper.Instance.GetRequest<TouristAuthResponse>(
				SdkType.Login,
				"https://us-central1-xsolla-sdk-demo.cloudfunctions.net/generateDemoUserToken",
				response => {
					XsollaToken.Create(response.access_token, response.refresh_token);
					onSuccess?.Invoke();
				},
				error => onError?.Invoke(error.errorMessage));
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
				"discord"  => SocialProvider.Discord,
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