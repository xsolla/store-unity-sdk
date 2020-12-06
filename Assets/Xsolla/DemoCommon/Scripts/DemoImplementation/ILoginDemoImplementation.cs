using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Xsolla.Core;
using Xsolla.Login;

namespace Xsolla.Demo
{
	public interface ILoginDemoImplementation
	{
		Token Token { get; set; }

		Token GetUserToken();

		void SaveToken(string key, string token);

		bool LoadToken(string key, out string token);

		void DeleteToken(string key);

		void ValidateToken(string token, [CanBeNull] Action<string> onSuccess = null,
			[CanBeNull] Action<Error> onError = null);

		void GetUserAttributes(string token, string projectId, UserAttributeType attributeType,
			List<string> attributeKeys, string userId, Action<List<UserAttribute>> onSuccess, Action<Error> onError);

		void UpdateUserAttributes(string token, string projectId, List<UserAttribute> attributes, Action onSuccess, Action<Error> onError);

		void RemoveUserAttributes(string token, string projectId, List<string> attributeKeys, Action onSuccess, Action<Error> onError);

		void RequestLinkingCode(Action<LinkingCode> onSuccess, Action<Error> onError);

		void SignInConsoleAccount(string userId, string platform, Action<string> successCase, Action<Error> failedCase);

		void LinkConsoleAccount(string userId, string platform, string confirmationCode, Action onSuccess, Action<Error> onError);

		void GetFriendsFromSocialNetworks([CanBeNull] Action<List<FriendModel>> onSuccess = null,
			[CanBeNull] Action<Error> onError = null);

		void ForceUpdateFriendsFromSocialNetworks([CanBeNull] Action onSuccess = null,
			[CanBeNull] Action<Error> onError = null);

		void SearchUsersByNickname(string nickname, [CanBeNull] Action<List<FriendModel>> onSuccess = null,
			[CanBeNull] Action<Error> onError = null);

		void GetPublicInfo(string token, string user, Action<UserPublicInfo> onSuccess, Action<Error> onError = null);

		void SignIn(string username, string password, bool rememberUser, [NotNull] Action onSuccess,
			[CanBeNull] Action<Error> onError = null);

		void SteamAuth(string appId, string sessionTicket, [CanBeNull] Action<string> onSuccess = null,
			[CanBeNull] Action<Error> onError = null);

		string GetSocialNetworkAuthUrl(SocialProvider socialProvider);

		void Registration(string username, string password, string email, [NotNull] Action onSuccess,
			[CanBeNull] Action<Error> onError = null);

		bool IsOAuthTokenRefreshInProgress { get; }

		void ExchangeCodeToToken(string code, Action<string> onSuccessExchange = null, Action<Error> onError = null);

		void ResetPassword(string username, [NotNull] Action onSuccess,
			[CanBeNull] Action<Error> onError = null);

		void GetUserInfo(string token, [NotNull] Action<UserInfo> onSuccess, [CanBeNull] Action<Error> onError = null);

		void UpdateUserInfo(string token, UserInfoUpdate info, Action<UserInfo> onSuccess, Action<Error> onError = null);

		void DeleteUserPicture(string token, Action onSuccess, Action<Error> onError);

		void UploadUserPicture(string token, byte[] pictureData, string boundary, Action<string> onSuccess, Action<Error> onError);

		void ChangeUserPhoneNumber(string token, string phoneNumber, Action onSuccess, Action<Error> onError);

		void DeleteUserPhoneNumber(string token, string phoneNumber, Action onSuccess, Action<Error> onError);

		void GetUserFriends([CanBeNull] Action<List<FriendModel>> onSuccess = null,
			[CanBeNull] Action<Error> onError = null);

		void BlockUser(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null,
			[CanBeNull] Action<Error> onError = null);

		void UnblockUser(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null,
			[CanBeNull] Action<Error> onError = null);

		void SendFriendshipInvite(FriendModel user, Action<FriendModel> onSuccess = null,
			Action<Error> onError = null);

		void RemoveFriend(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null,
			[CanBeNull] Action<Error> onError = null);

		void AcceptFriendship(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null,
			[CanBeNull] Action<Error> onError = null);

		void DeclineFriendship(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null,
			[CanBeNull] Action<Error> onError = null);

		void CancelFriendshipRequest(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null,
			[CanBeNull] Action<Error> onError = null);

		void GetBlockedUsers([CanBeNull] Action<List<FriendModel>> onSuccess = null,
			[CanBeNull] Action<Error> onError = null);

		void GetPendingUsers([CanBeNull] Action<List<FriendModel>> onSuccess = null,
			[CanBeNull] Action<Error> onError = null);

		void GetRequestedUsers([CanBeNull] Action<List<FriendModel>> onSuccess = null,
			[CanBeNull] Action<Error> onError = null);

		void GetLinkedSocialProviders(Action<List<LinkedSocialNetwork>> onSuccess, Action<Error> onError = null);

		void LinkSocialProvider(SocialProvider socialProvider, Action<SocialProvider> onSuccess, Action<Error> onError = null);
	}
}
