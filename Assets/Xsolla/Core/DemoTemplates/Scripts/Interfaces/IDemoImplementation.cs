using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xsolla.Core;
using Xsolla.Login;

public interface IDemoImplementation
{
	Token GetUserToken();
	
	void GetVirtualCurrencies([NotNull] Action<List<VirtualCurrencyModel>> onSuccess,
		[CanBeNull] Action<Error> onError = null);
	
	void GetCatalogVirtualItems([NotNull] Action<List<CatalogVirtualItemModel>> onSuccess,
		[CanBeNull] Action<Error> onError = null);

	void GetCatalogVirtualCurrencyPackages([NotNull] Action<List<CatalogVirtualCurrencyModel>> onSuccess,
		[CanBeNull] Action<Error> onError = null);
	
	void GetCatalogSubscriptions([NotNull] Action<List<CatalogSubscriptionItemModel>> onSuccess,
		[CanBeNull] Action<Error> onError = null);

	List<string> GetCatalogGroupsByItem(ItemModel item);

	void GetInventoryItems([NotNull] Action<List<InventoryItemModel>> onSuccess,
		[CanBeNull] Action<Error> onError = null);

	void GetVirtualCurrencyBalance([NotNull] Action<List<VirtualCurrencyBalanceModel>> onSuccess,
		[CanBeNull] Action<Error> onError = null);

	void GetUserSubscriptions([NotNull] Action<List<UserSubscriptionModel>> onSuccess,
		[CanBeNull] Action<Error> onError = null);

	void ConsumeVirtualCurrency(InventoryItemModel currency, uint count, [NotNull] Action onSuccess,
		[CanBeNull] Action onFailed = null);

	void ConsumeInventoryItem(InventoryItemModel item, uint count, [NotNull] Action<InventoryItemModel> onSuccess,
		[CanBeNull] Action<InventoryItemModel> onFailed = null);

	void PurchaseForRealMoney(CatalogItemModel item, [CanBeNull] Action<CatalogItemModel> onSuccess = null,
		[CanBeNull] Action<Error> onError = null);

	void PurchaseForVirtualCurrency(CatalogItemModel item, [CanBeNull] Action<CatalogItemModel> onSuccess = null,
		[CanBeNull] Action<Error> onError = null);

	void PurchaseCart(List<UserCartItem> items, [NotNull] Action<List<UserCartItem>> onSuccess, 
		[CanBeNull] Action<Error> onError = null);

	void GetUserFriends([CanBeNull] Action<List<FriendModel>> onSuccess = null, 
		[CanBeNull] Action<Error> onError = null);
	
	void GetBlockedUsers([CanBeNull] Action<List<FriendModel>> onSuccess = null, 
		[CanBeNull] Action<Error> onError = null);
	
	void GetPendingUsers([CanBeNull] Action<List<FriendModel>> onSuccess = null, 
		[CanBeNull] Action<Error> onError = null);
	
	void GetRequestedUsers([CanBeNull] Action<List<FriendModel>> onSuccess = null, 
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
	
	void GetFriendsFromSocialNetworks([CanBeNull] Action<List<FriendModel>> onSuccess = null,
		[CanBeNull] Action<Error> onError = null);
	
	void SearchUsersByNickname(string nickname, [CanBeNull] Action<List<FriendModel>> onSuccess = null,
		[CanBeNull] Action<Error> onError = null);

	Token Token { get; set; }

	Token GetDemoUserToken();
	
	void SaveToken(string key, string token);

	bool LoadToken(string key, out string token);

	void DeleteToken(string key);

	void ValidateToken(string token, [CanBeNull] Action<string> onSuccess = null, 
		[CanBeNull] Action<Error> onError = null);

	void GetUserInfo(string token, [NotNull] Action<UserInfo> onSuccess, [CanBeNull] Action<Error> onError = null);

	void UpdateUserInfo(string token, UserInfoUpdate info, Action<UserInfo> onSuccess, Action<Error> onError = null);

	void ChangeUserPhoneNumber(string token, string phoneNumber, Action onSuccess, Action<Error> onError);

	void DeleteUserPhoneNumber(string token, string phoneNumber, Action onSuccess, Action<Error> onError);

	void Registration(string username, string password, string email, [NotNull] Action onSuccess,
		[CanBeNull] Action<Error> onError = null);

	void SignIn(string username, string password, bool rememberUser, [NotNull] Action onSuccess,
		[CanBeNull] Action<Error> onError = null);

	void ResetPassword(string username, [NotNull] Action onSuccess,
		[CanBeNull] Action<Error> onError = null);

	void SteamAuth(string appId, string sessionTicket, [CanBeNull] Action<string> onSuccess = null,
		[CanBeNull] Action<Error> onError = null);

	string GetSocialNetworkAuthUrl(SocialProvider socialProvider);

	void LinkSocialProvider(SocialProvider socialProvider);

	void SignInConsoleAccount(string userId, string platform, Action<string> successCase, Action<Error> failedCase);

	void RequestLinkingCode(Action<LinkingCode> onSuccess, Action<Error> onError);

	void LinkConsoleAccount(string userId, string platform, string confirmationCode, Action onSuccess, Action<Error> onError);

	void GetUserAttributes(string token, string projectId, UserAttributeType attributeType,
		List<string> attributeKeys, string userId, Action<List<UserAttribute>> onSuccess, Action<Error> onError);

	void UpdateUserAttributes(string token, string projectId, List<UserAttribute> attributes, Action onSuccess, Action<Error> onError);

	void RemoveUserAttributes(string token, string projectId, List<string> attributeKeys, Action onSuccess, Action<Error> onError);

#region OAuth2.0
	bool IsOAuthTokenRefreshInProgress { get; }
	void ExchangeCodeToToken(string code, Action<string> onSuccessExchange = null, Action<Error> onError = null);
	#endregion

#region Picture
	void UploadUserPicture(string token, byte[] pictureData, string boundary, Action<string> onSuccess, Action<Error> onError);
	void DeleteUserPicture(string token, Action onSuccess, Action<Error> onError);
#endregion
}