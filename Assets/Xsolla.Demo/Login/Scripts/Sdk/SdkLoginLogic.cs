using System;
using System.Collections.Generic;
using Xsolla.Core;
using Xsolla.Login;

namespace Xsolla.Demo
{
	public class SdkLoginLogic : MonoSingleton<SdkLoginLogic>
	{
		private const int NETWORKS_CACHE_TIMEOUT = 5;

		[Obsolete("Use SdkAuthLogic instead")]
		public event Action RegistrationEvent
		{
			add => SdkAuthLogic.Instance.RegistrationEvent += value;
			remove => SdkAuthLogic.Instance.RegistrationEvent -= value;
		}

		[Obsolete("Use SdkAuthLogic instead")]
		public event Action LoginEvent
		{
			add => SdkAuthLogic.Instance.LoginEvent += value;
			remove => SdkAuthLogic.Instance.LoginEvent -= value;
		}

		[Obsolete("Use SdkAuthLogic instead")]
		public event Action UpdateUserInfoEvent
		{
			add => SdkAuthLogic.Instance.UpdateUserInfoEvent += value;
			remove => SdkAuthLogic.Instance.UpdateUserInfoEvent -= value;
		}

		[Obsolete("Use SdkUserAccountLogic instead")]
		public event Action UpdateUserAttributesEvent
		{
			add => SdkUserAccountLogic.Instance.UpdateUserAttributesEvent += value;
			remove => SdkUserAccountLogic.Instance.UpdateUserAttributesEvent -= value;
		}

		[Obsolete("Use SdkUserAccountLogic instead")]
		public event Action RemoveUserAttributesEvent
		{
			add => SdkUserAccountLogic.Instance.RemoveUserAttributesEvent += value;
			remove => SdkUserAccountLogic.Instance.RemoveUserAttributesEvent -= value;
		}

		#region Token
		[Obsolete("Use SdkAuthLogic instead")]
		public void ValidateToken(string token, Action<string> onSuccess = null, Action<Error> onError = null)
			=> SdkAuthLogic.Instance.ValidateToken(token, onSuccess, onError);
		#endregion

		#region User
		[Obsolete("Use SdkAuthLogic instead")]
		public void GetUserInfo(string token, Action<UserInfo> onSuccess = null, Action<Error> onError = null)
			=> SdkAuthLogic.Instance.GetUserInfo(token, onSuccess, onError);

		[Obsolete("Use SdkAuthLogic instead")]
		public void GetUserInfo(string token, bool useCache, Action<UserInfo> onSuccess, Action<Error> onError = null)
			=> SdkAuthLogic.Instance.GetUserInfo(token, useCache, onSuccess, onError);

		[Obsolete("Use SdkAuthLogic instead")]
		public void UpdateUserInfo(string token, UserInfoUpdate info, Action<UserInfo> onSuccess, Action<Error> onError = null)
			=> SdkAuthLogic.Instance.UpdateUserInfo(token, info, onSuccess, onError);

		[Obsolete("Use SdkUserAccountLogic instead")]
		public void GetPublicInfo(string token, string user, Action<UserPublicInfo> onSuccess, Action<Error> onError = null)
			=> SdkUserAccountLogic.Instance.GetPublicInfo(token, user, onSuccess, onError);

		[Obsolete("Use SdkAuthLogic instead")]
		public void Registration(string username, string password, string email, string state = null, Action onSuccess = null, Action<Error> onError = null)
			=> SdkAuthLogic.Instance.Registration(username, password, email, state, onSuccess, onError);

		[Obsolete("Use SdkAuthLogic instead")]
		public void Registration(string username, string password, string email, string state = null, Action<int> onSuccess = null, Action<Error> onError = null)
			=> SdkAuthLogic.Instance.Registration(username, password, email, state, onSuccess, onError);

		[Obsolete("Use SdkAuthLogic instead")]
		public void Registration(string username, string password, string email, string state = null, Action<LoginUrlResponse> onSuccess = null, Action<Error> onError = null)
			=> SdkAuthLogic.Instance.Registration(username, password, email, state, onSuccess, onError);

		[Obsolete("Use SdkAuthLogic instead")]
		public void SignIn(string username, string password, bool rememberUser, Action<string> onSuccess, Action<Error> onError = null)
			=> SdkAuthLogic.Instance.SignIn(username, password, rememberUser, onSuccess, onError);

		[Obsolete("Use SdkAuthLogic instead")]
		public void AccessTokenAuth(string email, Action onSuccess, Action<Error> onError = null)
			=> SdkAuthLogic.Instance.AccessTokenAuth(email, onSuccess, onError);

		[Obsolete("Use SdkAuthLogic instead")]
		public void ResetPassword(string username, Action onSuccess = null, Action<Error> onError = null)
			=> SdkAuthLogic.Instance.ResetPassword(username, onSuccess, onError);

		[Obsolete("Use SdkAuthLogic instead")]
		public void ResendConfirmationLink(string username, Action onSuccess = null, Action<Error> onError = null)
			=> SdkAuthLogic.Instance.ResendConfirmationLink(username, onSuccess, onError);

		[Obsolete("Use SdkUserAccountLogic instead")]
		public void ChangeUserPhoneNumber(string token, string phoneNumber, Action onSuccess, Action<Error> onError)
			=> SdkUserAccountLogic.Instance.ChangeUserPhoneNumber(token, phoneNumber, onSuccess, onError);

		[Obsolete("Use SdkUserAccountLogic instead")]
		public void DeleteUserPhoneNumber(string token, string phoneNumber, Action onSuccess, Action<Error> onError)
			=> SdkUserAccountLogic.Instance.DeleteUserPhoneNumber(token, phoneNumber, onSuccess, onError);

		[Obsolete("Use SdkUserAccountLogic instead")]
		public void SearchUsersByNickname(string nickname, Action<List<FriendModel>> onSuccess = null, Action<Error> onError = null)
			=> SdkUserAccountLogic.Instance.SearchUsersByNickname(nickname, onSuccess, onError);
		#endregion

		#region Social
		[Obsolete("Use SdkAuthLogic instead")]
		public void SteamAuth(string appId, string sessionTicket, string state = null, Action<string> onSuccess = null, Action<Error> onError = null)
			=> SdkAuthLogic.Instance.SteamAuth(appId, sessionTicket, state, onSuccess, onError);

		[Obsolete("Use SdkAuthLogic instead")]
		public void AuthViaDeviceID(Core.DeviceType deviceType, string deviceName, string deviceId, string payload = null, string state = null, Action<string> onSuccess = null, Action<Error> onError = null)
			=> SdkAuthLogic.Instance.AuthViaDeviceID(deviceType, deviceName, deviceId, payload, state, onSuccess, onError);

		[Obsolete("Use SdkAuthLogic instead")]
		public string GetSocialNetworkAuthUrl(SocialProvider socialProvider)
			=> SdkAuthLogic.Instance.GetSocialNetworkAuthUrl(socialProvider);

		[Obsolete("Use SdkUserAccountLogic instead")]
		public void LinkSocialProvider(SocialProvider socialProvider, Action<SocialProvider> onSuccess, Action<Error> onError = null)
			=> SdkUserAccountLogic.Instance.LinkSocialProvider(socialProvider, onSuccess, onError);

		[Obsolete("Use SdkUserAccountLogic instead")]
		public void PurgeSocialProvidersCache()
			=> SdkUserAccountLogic.Instance.PurgeSocialProvidersCache();

		[Obsolete("Use SdkUserAccountLogic instead")]
		public void GetLinkedSocialProviders(Action<List<LinkedSocialNetwork>> onSuccess, Action<Error> onError = null)
			=> SdkUserAccountLogic.Instance.GetLinkedSocialProviders(onSuccess, onError);
		#endregion

		#region AccountLinking
		[Obsolete("Use SdkAuthLogic instead")]
		public void SignInConsoleAccount(string userId, string platform, Action<string> onSuccess, Action<Error> onError)
			=> SdkAuthLogic.Instance.SignInConsoleAccount(userId, platform, onSuccess, onError);

		[Obsolete("Use SdkUserAccountLogic instead")]
		public void RequestLinkingCode(Action<LinkingCode> onSuccess, Action<Error> onError)
			=> SdkUserAccountLogic.Instance.RequestLinkingCode(onSuccess, onError);

		[Obsolete("Use SdkUserAccountLogic instead")]
		public void LinkConsoleAccount(string userId, string platform, string confirmationCode, Action onSuccess, Action<Error> onError)
			=> SdkUserAccountLogic.Instance.LinkConsoleAccount(userId, platform, confirmationCode, onSuccess, onError);
		#endregion

		#region Attributes
		[Obsolete("Use SdkUserAccountLogic instead")]
		public void GetUserAttributes(string token, string projectId, UserAttributeType attributeType,
			List<string> attributeKeys, string userId, Action<List<UserAttribute>> onSuccess, Action<Error> onError)
			=> SdkUserAccountLogic.Instance.GetUserAttributes(token, projectId, attributeType, attributeKeys, userId, onSuccess, onError);

		[Obsolete("Use SdkUserAccountLogic instead")]
		public void UpdateUserAttributes(string token, string projectId, List<UserAttribute> attributes, Action onSuccess, Action<Error> onError)
			=> SdkUserAccountLogic.Instance.UpdateUserAttributes(token, projectId, attributes, onSuccess, onError);

		[Obsolete("Use SdkUserAccountLogic instead")]
		public void RemoveUserAttributes(string token, string projectId, List<string> attributeKeys, Action onSuccess, Action<Error> onError)
			=> SdkUserAccountLogic.Instance.RemoveUserAttributes(token, projectId, attributeKeys, onSuccess, onError);
		#endregion

		#region OAuth2.0
		[Obsolete("Use SdkAuthLogic instead")]
		public bool IsOAuthTokenRefreshInProgress
			=> SdkAuthLogic.Instance.IsOAuthTokenRefreshInProgress;

		[Obsolete("Use SdkAuthLogic instead")]
		public void ExchangeCodeToToken(string code, Action<string> onSuccessExchange = null, Action<Error> onError = null)
			=> SdkAuthLogic.Instance.ExchangeCodeToToken(code, onSuccessExchange, onError);
		#endregion

		#region Picture
		[Obsolete("Use SdkUserAccountLogic instead")]
		public void UploadUserPicture(string token, byte[] pictureData, string boundary, Action<string> onSuccess, Action<Error> onError)
			=> SdkUserAccountLogic.Instance.UploadUserPicture(token, pictureData, boundary, onSuccess, onError);

		[Obsolete("Use SdkUserAccountLogic instead")]
		public void DeleteUserPicture(string token, Action onSuccess, Action<Error> onError)
			=> SdkUserAccountLogic.Instance.DeleteUserPicture(token, onSuccess, onError);
		#endregion

		#region Passwordless
		[Obsolete("Use SdkAuthLogic instead")]
		public void StartAuthByPhoneNumber(string phoneNumber, string linkUrl, bool sendLink, Action<string> onSuccess, Action<Error> onError = null)
			=> SdkAuthLogic.Instance.StartAuthByPhoneNumber(phoneNumber, linkUrl, sendLink, onSuccess, onError);

		[Obsolete("Use SdkAuthLogic instead")]
		public void CompleteAuthByPhoneNumber(string phoneNumber, string confirmationCode, string operationId, Action<string> onSuccess, Action<Error> onError = null)
			=> SdkAuthLogic.Instance.CompleteAuthByPhoneNumber(phoneNumber, confirmationCode, operationId, onSuccess, onError);

		[Obsolete("Use SdkAuthLogic instead")]
		public void StartAuthByEmail(string email, string linkUrl, bool sendLink, Action<string> onSuccess, Action<Error> onError = null)
			=> SdkAuthLogic.Instance.StartAuthByEmail(email, linkUrl, sendLink, onSuccess, onError);

		[Obsolete("Use SdkAuthLogic instead")]
		public void CompleteAuthByEmail(string email, string confirmationCode, string operationId, Action<string> onSuccess, Action<Error> onError = null)
			=> SdkAuthLogic.Instance.CompleteAuthByEmail(email, confirmationCode, operationId, onSuccess, onError);
		#endregion
	}
}
