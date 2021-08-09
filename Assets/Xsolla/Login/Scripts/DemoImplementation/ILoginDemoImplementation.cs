using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Xsolla.Core;
using Xsolla.Login;

namespace Xsolla.Demo
{
	public partial interface ILoginDemoImplementation
	{
		void ValidateToken(string token, [CanBeNull] Action<string> onSuccess = null,
			[CanBeNull] Action<Error> onError = null);

		void GetUserAttributes(string token, string projectId, UserAttributeType attributeType,
			List<string> attributeKeys, string userId, Action<List<UserAttribute>> onSuccess, Action<Error> onError);

		void UpdateUserAttributes(string token, string projectId, List<UserAttribute> attributes, Action onSuccess, Action<Error> onError);

		void RemoveUserAttributes(string token, string projectId, List<string> attributeKeys, Action onSuccess, Action<Error> onError);

		void RequestLinkingCode(Action<LinkingCode> onSuccess, Action<Error> onError);

		void SignInConsoleAccount(string userId, string platform, Action<string> successCase, Action<Error> failedCase);

		void LinkConsoleAccount(string userId, string platform, string confirmationCode, Action onSuccess, Action<Error> onError);

		void SearchUsersByNickname(string nickname, [CanBeNull] Action<List<FriendModel>> onSuccess = null,
			[CanBeNull] Action<Error> onError = null);

		void GetPublicInfo(string token, string user, Action<UserPublicInfo> onSuccess, Action<Error> onError = null);

		void SignIn(string username, string password, bool rememberUser, [NotNull] Action<string> onSuccess,
			[CanBeNull] Action<Error> onError = null);

		void SteamAuth(string appId, string sessionTicket, string state = null, [CanBeNull] Action<string> onSuccess = null,
			[CanBeNull] Action<Error> onError = null);

		void AuthViaDeviceID(Core.DeviceType deviceType, string deviceName, string deviceId, string payload = null, string state = null, Action<string> onSuccess = null, Action<Error> onError = null);

		void AccessTokenAuth(string email, [NotNull] Action onSuccess,
			[CanBeNull] Action<Error> onError = null);

		string GetSocialNetworkAuthUrl(SocialProvider socialProvider);

		void Registration(string username, string password, string email, string state = null, [CanBeNull] Action onSuccess = null,
			[CanBeNull] Action<Error> onError = null);

		bool IsOAuthTokenRefreshInProgress { get; }

		void ExchangeCodeToToken(string code, Action<string> onSuccessExchange = null, Action<Error> onError = null);

		void ResetPassword(string username, [CanBeNull] Action onSuccess = null,
			[CanBeNull] Action<Error> onError = null);

		void ResendConfirmationLink(string username, [CanBeNull] Action onSuccess = null, [CanBeNull] Action<Error> onError = null);

		void GetUserInfo(string token, [NotNull] Action<UserInfo> onSuccess, [CanBeNull] Action<Error> onError = null);

		void UpdateUserInfo(string token, UserInfoUpdate info, Action<UserInfo> onSuccess, Action<Error> onError = null);

		void DeleteUserPicture(string token, Action onSuccess, Action<Error> onError);

		void UploadUserPicture(string token, byte[] pictureData, string boundary, Action<string> onSuccess, Action<Error> onError);

		void ChangeUserPhoneNumber(string token, string phoneNumber, Action onSuccess, Action<Error> onError);

		void DeleteUserPhoneNumber(string token, string phoneNumber, Action onSuccess, Action<Error> onError);

		void GetLinkedSocialProviders(Action<List<LinkedSocialNetwork>> onSuccess, Action<Error> onError = null);

		void LinkSocialProvider(SocialProvider socialProvider, Action<SocialProvider> onSuccess, Action<Error> onError = null);
	}
}
