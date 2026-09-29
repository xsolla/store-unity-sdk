using System;

namespace Xsolla.Demo
{
	public interface IAuthService
	{
		void ClearSavedData();

		void AuthBuySavedData(Action onSuccess, Action onError);
		
		void SignInTouristMode(Action onSuccess, Action<string> onError);

		void SignIn(string username, string password, Action onSuccess, Action<string> onError);

		void Register(string username, string email, string password, Action onSuccess, Action<string> onError);

		void AuthViaSocialNetwork(string socialNetwork, Action onSuccess, Action<string> onError, Action onCancel);
	}
}