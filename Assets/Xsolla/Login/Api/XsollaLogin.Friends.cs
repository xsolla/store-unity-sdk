using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.Login
{
	public partial class XsollaLogin : MonoSingleton<XsollaLogin>
	{

		[Obsolete("Use XsollaUserAccount instead")]
		public void GetUserSocialFriends(string token, SocialProvider platform = SocialProvider.None, uint offset = 0, uint limit = 500, bool withXlUid = false, Action<UserSocialFriends> onSuccess = null, Action<Error> onError = null)
			=> XsollaUserAccount.Instance.GetUserSocialFriends(token, platform, offset, limit, withXlUid, onSuccess, onError);

		[Obsolete("Use XsollaUserAccount instead")]
		public void UpdateUserSocialFriends(string token, SocialProvider platform = SocialProvider.None, Action onSuccess = null, Action<Error> onError = null)
			=> XsollaUserAccount.Instance.UpdateUserSocialFriends(token, platform, onSuccess, onError);

		[Obsolete("Use XsollaUserAccount instead")]
		public void GetUserFriends(
			string token,
			FriendsSearchType type,
			FriendsSearchResultsSort sortBy = FriendsSearchResultsSort.ByNickname, 
			FriendsSearchResultsSortOrder sortOrder = FriendsSearchResultsSortOrder.Asc,
			string after = null,
			int? limit = null,
			Action<List<UserFriendEntity>> onSuccess = null, Action<Error> onError = null)
			=> XsollaUserAccount.Instance.GetUserFriends(token, type, sortBy, sortOrder, after, limit, onSuccess, onError);

		[Obsolete("Use XsollaUserAccount instead")]
		public void UpdateUserFriends(string token, FriendAction action, string user, Action onSuccess, Action<Error> onError)
			=> XsollaUserAccount.Instance.UpdateUserFriends(token, action, user, onSuccess, onError);
	}
}
